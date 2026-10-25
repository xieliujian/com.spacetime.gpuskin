---
name: git-commit-push
description: Commit and push changes in SpaceTime Unity package repositories, with explicit manual file selection or automatic dirty-repository detection. 用于提交、推送、同步 Packages/com.spacetime.* 下各独立包的改动；用户说“提交并推送”“批量提交”“git commit push”时使用。
---

# Git Commit Push

这个技能处理 Packages/com.spacetime.* 这类独立仓库的提交与推送，支持两种模式：

- 手动模式：明确指定仓库、文件路径和提交说明，只提交这些文件。
- 自动模式：扫描所有独立包，提交已跟踪文件的修改并推送。

## 选择模式

当只改了少数文件、仓库不在 Packages 下，或需要排除无关改动时，用手动模式。

当用户要求“把所有改动提交并推送”，且改动都在已跟踪文件中时，用自动模式。

自动模式默认不包含未跟踪文件；需要包含时加 -IncludeUntracked，或改用手动模式指定路径。

在 Packages 目录或某个包目录中运行时，可以省略 -PackagesRoot，脚本会自动推断。

## 执行

脚本路径：scripts/commit_push.ps1。使用 Windows PowerShell 运行：

~~~powershell
powershell -NoProfile -ExecutionPolicy Bypass -File "<skill-dir>\scripts\commit_push.ps1" ...
~~~

先查看计划：

~~~powershell
powershell -NoProfile -ExecutionPolicy Bypass -File "<skill-dir>\scripts\commit_push.ps1" -PackagesRoot "<Packages>" -AllPackages -Auto -DryRun
~~~

手动提交当前仓库：

~~~powershell
powershell -NoProfile -ExecutionPolicy Bypass -File "<skill-dir>\scripts\commit_push.ps1" -RepoPath "<repo>" -Paths "Runtime/Foo.cs" -Message "Fix foo initialization"
~~~

批量提交各仓库中同一个相对路径：

~~~powershell
powershell -NoProfile -ExecutionPolicy Bypass -File "<skill-dir>\scripts\commit_push.ps1" -PackagesRoot "<Packages>" -AllPackages -Paths ".agents/skills/example" -Message "Add example skill"
~~~

自动提交所有已跟踪改动：

~~~powershell
powershell -NoProfile -ExecutionPolicy Bypass -File "<skill-dir>\scripts\commit_push.ps1" -PackagesRoot "<Packages>" -AllPackages -Auto -Message "Update package code"
~~~

推送失败且确认是 GitHub SSH 认证问题时，加 -HttpsFallback。只想本地提交时加 -NoPush。

## Pageant 推送

如果 Pageant 已运行并加载了 GitHub 密钥，在命令中加 -Pageant：

~~~powershell
powershell -NoProfile -ExecutionPolicy Bypass -File "<skill-dir>\scripts\commit_push.ps1" -PackagesRoot "<Packages>" -AllPackages -Auto -Message "Update package code" -Pageant
~~~

如果 Pageant 尚未运行，可以把 .ppk 路径交给 -PageantKey，脚本会启动 Pageant 并加载该密钥：

~~~powershell
powershell -NoProfile -ExecutionPolicy Bypass -File "<skill-dir>\scripts\commit_push.ps1" -PackagesRoot "<Packages>" -AllPackages -Auto -Message "Update package code" -Pageant -PageantKey "D:\xieliujian\TortoiseGit_github_private_key.ppk"
~~~

Pageant 模式通过 TortoiseGit 的 TortoiseGitPlink 连接。环境设置只作用于本次脚本，不修改全局 Git 配置。密钥由 Pageant 读取，脚本不会把密钥内容或路径写入仓库。Pageant 已运行时，-PageantKey 不会强制重新加载密钥；请先在 Pageant 中确认目标密钥已经加载。

## 提交说明

用户给了提交说明就原样使用。没给时，先看 git diff --stat 和实际改动，写一条能说明目的的简短说明；不要用“update files”这类空话。

## 安全规则

- 手动模式只暂存并提交 -Paths 指定的文件。
- 自动模式只暂存已跟踪文件；未跟踪文件必须显式包含。
- 不执行 force push、amend、reset、clean 或历史重写。
- detached HEAD、合并/变基/拣选/回滚进行中、存在冲突时跳过该仓库。
- 推送失败保留本地提交，查清原因再重试。
- -HttpsFallback 只对 GitHub SSH 地址临时改用 HTTPS，不修改 origin。
- 不提交被 Git 忽略的文件。

## 汇报

完成后逐仓库报告：仓库名、分支、提交哈希、推送结果、跳过原因。推送后用 git ls-remote 核对远端分支与本地 HEAD 一致。任一仓库失败时继续处理其余仓库，最后以非零退出码结束。
