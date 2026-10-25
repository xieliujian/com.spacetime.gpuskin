[CmdletBinding()]
param(
    [string]$RepoPath = "",
    [string]$PackagesRoot = "",
    [switch]$AllPackages,
    [string[]]$RepoName = @(),
    [switch]$Auto,
    [string[]]$Paths = @(),
    [string]$Message = "",
    [switch]$IncludeUntracked,
    [switch]$NoPush,
    [switch]$DryRun,
    [switch]$HttpsFallback,
    [switch]$Pageant,
    [string]$PageantKey = ""
)

$ErrorActionPreference = "Stop"
$results = New-Object System.Collections.Generic.List[object]

function Invoke-Git {
    param(
        [string]$Repo,
        [string[]]$GitArgs
    )

    $oldErrorAction = $ErrorActionPreference
    $ErrorActionPreference = "Continue"
    $raw = & git -C $Repo @GitArgs 2>&1
    $code = $LASTEXITCODE
    $ErrorActionPreference = $oldErrorAction
    $lines = @()

    foreach ($line in $raw) {
        $lines += $line.ToString()
    }

    return [pscustomobject]@{
        ExitCode = $code
        Output = ($lines -join [Environment]::NewLine)
    }
}

function Add-Result {
    param(
        [string]$Repo,
        [string]$Branch,
        [string]$Status,
        [string]$Commit,
        [string]$Detail
    )

    $script:results.Add([pscustomobject]@{
        Repo = $Repo
        Branch = $Branch
        Status = $Status
        Commit = $Commit
        Detail = $Detail
    })
}

function Test-GitMarker {
    param(
        [string]$Repo,
        [string]$Name
    )

    $result = Invoke-Git -Repo $Repo -GitArgs @("rev-parse", "--git-path", $Name)
    if ($result.ExitCode -ne 0) {
        return $false
    }

    $marker = $result.Output.Trim()
    if ([string]::IsNullOrWhiteSpace($marker)) {
        return $false
    }

    if (-not [System.IO.Path]::IsPathRooted($marker)) {
        $marker = Join-Path $Repo $marker
    }

    return (Test-Path -LiteralPath $marker)
}

function Get-RepoState {
    param([string]$Repo)

    $inside = Invoke-Git -Repo $Repo -GitArgs @("rev-parse", "--is-inside-work-tree")
    if ($inside.ExitCode -ne 0 -or $inside.Output.Trim() -ne "true") {
        return [pscustomobject]@{
            IsRepo = $false
            Root = $Repo
            Branch = ""
            Detached = $false
            Operation = ""
            HasConflicts = $false
        }
    }

    $rootResult = Invoke-Git -Repo $Repo -GitArgs @("rev-parse", "--show-toplevel")
    $branchResult = Invoke-Git -Repo $Repo -GitArgs @("symbolic-ref", "--quiet", "--short", "HEAD")
    $detached = $branchResult.ExitCode -ne 0
    $branch = ""
    if (-not $detached) {
        $branch = $branchResult.Output.Trim()
    }

    $operation = ""
    foreach ($name in @("MERGE_HEAD", "CHERRY_PICK_HEAD", "REVERT_HEAD")) {
        if (Test-GitMarker -Repo $rootResult.Output.Trim() -Name $name) {
            $operation = $name
            break
        }
    }

    if ([string]::IsNullOrWhiteSpace($operation)) {
        foreach ($name in @("rebase-merge", "rebase-apply")) {
            if (Test-GitMarker -Repo $rootResult.Output.Trim() -Name $name) {
                $operation = $name
                break
            }
        }
    }

    $conflicts = Invoke-Git -Repo $rootResult.Output.Trim() -GitArgs @("ls-files", "-u")
    $hasConflicts = $conflicts.ExitCode -eq 0 -and -not [string]::IsNullOrWhiteSpace($conflicts.Output)

    return [pscustomobject]@{
        IsRepo = $true
        Root = $rootResult.Output.Trim()
        Branch = $branch
        Detached = $detached
        Operation = $operation
        HasConflicts = $hasConflicts
    }
}

function Get-ChangedPaths {
    param(
        [string]$Repo,
        [bool]$WithUntracked
    )

    $paths = New-Object System.Collections.Generic.HashSet[string]
    $commands = @(
        @("diff", "--name-only"),
        @("diff", "--cached", "--name-only")
    )

    foreach ($command in $commands) {
        $result = Invoke-Git -Repo $Repo -GitArgs $command
        if ($result.ExitCode -ne 0) {
            continue
        }

        foreach ($line in ($result.Output -split "\r?\n")) {
            if (-not [string]::IsNullOrWhiteSpace($line)) {
                [void]$paths.Add($line.Trim())
            }
        }
    }

    if ($WithUntracked) {
        $untracked = Invoke-Git -Repo $Repo -GitArgs @("ls-files", "--others", "--exclude-standard")
        if ($untracked.ExitCode -eq 0) {
            foreach ($line in ($untracked.Output -split "\r?\n")) {
                if (-not [string]::IsNullOrWhiteSpace($line)) {
                    [void]$paths.Add($line.Trim())
                }
            }
        }
    }

    return @($paths)
}

function Resolve-PackagesRoot {
    if (-not [string]::IsNullOrWhiteSpace($PackagesRoot)) {
        return (Resolve-Path -LiteralPath $PackagesRoot).Path
    }

    $current = (Get-Location).Path
    if ((Split-Path -Leaf $current) -eq "Packages") {
        return $current
    }

    $parent = Split-Path -Parent $current
    if ((Split-Path -Leaf $parent) -eq "Packages") {
        return $parent
    }

    throw "Packages root not found. Pass -PackagesRoot."
}

function ConvertTo-HttpsUrl {
    param([string]$Url)

    if ($Url.StartsWith("git@github.com:")) {
        return "https://github.com/" + $Url.Substring("git@github.com:".Length)
    }

    if ($Url.StartsWith("ssh://git@github.com/")) {
        return "https://github.com/" + $Url.Substring("ssh://git@github.com/".Length)
    }

    return ""
}

function Resolve-Executable {
    param(
        [string]$Name,
        [string[]]$Candidates
    )

    $command = Get-Command $Name -ErrorAction SilentlyContinue
    if ($command -and -not [string]::IsNullOrWhiteSpace($command.Source)) {
        return $command.Source
    }

    foreach ($candidate in $Candidates) {
        if (Test-Path -LiteralPath $candidate) {
            return $candidate
        }
    }

    return ""
}

function Enable-Pageant {
    param([string]$KeyPath)

    $plink = Resolve-Executable -Name "TortoiseGitPlink" -Candidates @(
        "D:\Program Files\TortoiseGit\bin\TortoiseGitPlink.exe",
        "D:\Program Files\TortoiseGit\bin\TortoisePlink.exe"
    )
    if ([string]::IsNullOrWhiteSpace($plink)) {
        throw "TortoiseGitPlink.exe not found. Install TortoiseGit or add it to PATH."
    }

    $pageantPath = Resolve-Executable -Name "pageant" -Candidates @(
        "D:\Program Files\TortoiseGit\bin\pageant.exe"
    )

    $running = @(Get-Process pageant -ErrorAction SilentlyContinue)
    if ($running.Count -eq 0) {
        if ([string]::IsNullOrWhiteSpace($KeyPath)) {
            throw "Pageant is not running. Pass -PageantKey <ppk> or start Pageant with the key loaded."
        }

        if ([string]::IsNullOrWhiteSpace($pageantPath)) {
            throw "pageant.exe not found. Install Pageant or add it to PATH."
        }

        $fullKeyPath = (Resolve-Path -LiteralPath $KeyPath).Path
        Start-Process -FilePath $pageantPath -ArgumentList ('"' + $fullKeyPath + '"') -WindowStyle Hidden
        Start-Sleep -Seconds 2
        $running = @(Get-Process pageant -ErrorAction SilentlyContinue)
        if ($running.Count -eq 0) {
            throw "Pageant did not start. Check the key file and Pageant installation."
        }
    }

    $env:GIT_SSH_COMMAND = '"' + $plink + '" -batch'
    $env:GIT_SSH_VARIANT = "plink"
}

if (-not $Auto -and $Paths.Count -eq 0) {
    throw "Specify -Auto or -Paths."
}

if ($AllPackages) {
    $root = Resolve-PackagesRoot
    $repos = @()
    foreach ($directory in (Get-ChildItem -LiteralPath $root -Directory)) {
        if (-not (Test-Path -LiteralPath (Join-Path $directory.FullName ".git"))) {
            continue
        }

        if ($RepoName.Count -gt 0 -and $RepoName -notcontains $directory.Name) {
            continue
        }

        $repos += $directory.FullName
    }
} else {
    if ([string]::IsNullOrWhiteSpace($RepoPath)) {
        $RepoPath = (Get-Location).Path
    }

    $repos = @((Resolve-Path -LiteralPath $RepoPath).Path)
}

if ($repos.Count -eq 0) {
    throw "No Git repositories found."
}

$usePageant = $Pageant -and -not $DryRun -and -not $NoPush
$oldSshCommand = $env:GIT_SSH_COMMAND
$oldSshVariant = $env:GIT_SSH_VARIANT

if ($usePageant) {
    Enable-Pageant -KeyPath $PageantKey
}

foreach ($repo in $repos) {
    $state = Get-RepoState -Repo $repo
    $name = Split-Path -Leaf $state.Root

    if (-not $state.IsRepo) {
        Add-Result -Repo $repo -Branch "" -Status "failed" -Commit "" -Detail "not a git repository"
        continue
    }

    if ($state.Detached) {
        Add-Result -Repo $name -Branch "" -Status "failed" -Commit "" -Detail "detached HEAD"
        continue
    }

    if (-not [string]::IsNullOrWhiteSpace($state.Operation)) {
        Add-Result -Repo $name -Branch $state.Branch -Status "failed" -Commit "" -Detail ("in progress: " + $state.Operation)
        continue
    }

    if ($state.HasConflicts) {
        Add-Result -Repo $name -Branch $state.Branch -Status "failed" -Commit "" -Detail "unresolved conflicts"
        continue
    }

    $candidatePaths = @()
    if (-not $Auto) {
        foreach ($path in $Paths) {
            $fullPath = $path
            if (-not [System.IO.Path]::IsPathRooted($path)) {
                $fullPath = Join-Path $state.Root $path
            }

            if (Test-Path -LiteralPath $fullPath) {
                $candidatePaths += $path
                continue
            }

            $tracked = Invoke-Git -Repo $state.Root -GitArgs (@("ls-files", "--error-unmatch", "--") + @($path))
            if ($tracked.ExitCode -eq 0) {
                $candidatePaths += $path
            }
        }

        if ($candidatePaths.Count -eq 0) {
            Add-Result -Repo $name -Branch $state.Branch -Status "skipped" -Commit "" -Detail "no matching paths"
            continue
        }
    }

    if ($DryRun) {
        $includeUntracked = $IncludeUntracked.IsPresent
        if ($Auto) {
            $preview = Get-ChangedPaths -Repo $state.Root -WithUntracked $includeUntracked
        } else {
            $preview = $candidatePaths
        }

        if ($preview.Count -eq 0) {
            Add-Result -Repo $name -Branch $state.Branch -Status "skipped" -Commit "" -Detail "no changes"
            continue
        }

        $detail = ($preview | Select-Object -First 20) -join ", "
        if ($preview.Count -gt 20) {
            $detail += " ..."
        }

        Add-Result -Repo $name -Branch $state.Branch -Status "dry-run" -Commit "" -Detail $detail
        continue
    }

    if ($Auto) {
        $add = Invoke-Git -Repo $state.Root -GitArgs @("add", "-u")
        if ($add.ExitCode -ne 0) {
            Add-Result -Repo $name -Branch $state.Branch -Status "failed" -Commit "" -Detail $add.Output
            continue
        }

        if ($IncludeUntracked) {
            $addAll = Invoke-Git -Repo $state.Root -GitArgs @("add", "--all", "--", ".")
            if ($addAll.ExitCode -ne 0) {
                Add-Result -Repo $name -Branch $state.Branch -Status "failed" -Commit "" -Detail $addAll.Output
                continue
            }
        }
    } else {
        $addArgs = @("add", "--") + $candidatePaths
        $add = Invoke-Git -Repo $state.Root -GitArgs $addArgs
        if ($add.ExitCode -ne 0) {
            Add-Result -Repo $name -Branch $state.Branch -Status "failed" -Commit "" -Detail $add.Output
            continue
        }
    }

    $diffArgs = @("diff", "--cached", "--quiet")
    if (-not $Auto) {
        $diffArgs += @("--") + $candidatePaths
    }

    $diff = Invoke-Git -Repo $state.Root -GitArgs $diffArgs
    if ($diff.ExitCode -eq 0) {
        Add-Result -Repo $name -Branch $state.Branch -Status "skipped" -Commit "" -Detail "no staged changes"
        continue
    }

    if ($diff.ExitCode -ne 1) {
        Add-Result -Repo $name -Branch $state.Branch -Status "failed" -Commit "" -Detail $diff.Output
        continue
    }

    $commitMessage = $Message
    if ([string]::IsNullOrWhiteSpace($commitMessage)) {
        $commitMessage = "Update " + $name + " package"
    }

    $commitArgs = @("commit", "-m", $commitMessage)
    if (-not $Auto) {
        $commitArgs += @("--") + $candidatePaths
    }

    $commit = Invoke-Git -Repo $state.Root -GitArgs $commitArgs
    if ($commit.ExitCode -ne 0) {
        Add-Result -Repo $name -Branch $state.Branch -Status "failed" -Commit "" -Detail $commit.Output
        continue
    }

    $hashResult = Invoke-Git -Repo $state.Root -GitArgs @("rev-parse", "--short", "HEAD")
    $hash = $hashResult.Output.Trim()

    if ($NoPush) {
        Add-Result -Repo $name -Branch $state.Branch -Status "committed" -Commit $hash -Detail "push skipped"
        continue
    }

    $push = Invoke-Git -Repo $state.Root -GitArgs @("push", "origin", $state.Branch)
    $pushDetail = $push.Output

    if ($push.ExitCode -ne 0 -and $HttpsFallback) {
        $remote = Invoke-Git -Repo $state.Root -GitArgs @("remote", "get-url", "origin")
        if ($remote.ExitCode -eq 0) {
            $httpsUrl = ConvertTo-HttpsUrl -Url $remote.Output.Trim()
            if (-not [string]::IsNullOrWhiteSpace($httpsUrl)) {
                $oldPrompt = $env:GIT_TERMINAL_PROMPT
                $env:GIT_TERMINAL_PROMPT = "0"
                $retry = Invoke-Git -Repo $state.Root -GitArgs @("push", $httpsUrl, $state.Branch)
                $env:GIT_TERMINAL_PROMPT = $oldPrompt
                if ($retry.ExitCode -eq 0) {
                    [void](Invoke-Git -Repo $state.Root -GitArgs @("update-ref", ("refs/remotes/origin/" + $state.Branch), "HEAD"))
                    $push = $retry
                    $pushDetail = $retry.Output + " (HTTPS fallback)"
                }
            }
        }
    }

    if ($push.ExitCode -ne 0) {
        Add-Result -Repo $name -Branch $state.Branch -Status "failed" -Commit $hash -Detail $pushDetail
        continue
    }

    $detail = "origin/" + $state.Branch
    if ($usePageant) {
        $detail += " via Pageant"
    }

    Add-Result -Repo $name -Branch $state.Branch -Status "pushed" -Commit $hash -Detail $detail
}

if ($usePageant) {
    $env:GIT_SSH_COMMAND = $oldSshCommand
    $env:GIT_SSH_VARIANT = $oldSshVariant
}

$results | Format-Table -AutoSize | Out-String -Width 300 | Write-Output

$failed = @($results | Where-Object { $_.Status -eq "failed" })
if ($failed.Count -gt 0) {
    exit 1
}
