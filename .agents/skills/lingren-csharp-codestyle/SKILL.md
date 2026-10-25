---
name: lingren-csharp-codestyle
description: Apply LingRen company C# coding standards (公司代码规范) whenever writing, generating, editing, moving, reviewing, or formatting C# / Unity C# code. 写 C# 代码、改代码、代码评审、代码规范检查时默认使用。Also use when the user mentions LingRen code style, C# 代码规范, codestyle, DokuWiki codestyle, m_ prefix, s_ prefix, 4-space indentation, 120-character line limit, or asks to make code conform to http://192.168.0.211/dokuwiki/doku.php?id=wiki:codestyle.
when_to_use: Use for any task that produces or modifies C# or Unity C# code in the LingRen project, including creating new files, editing existing code, moving or renaming code, code review, and reformatting. Also use when asked whether existing code follows the company standard. Unless the user explicitly requests a different code style, apply this skill by default.
---

# LingRen C# Code Style

Source of truth: `http://192.168.0.211/dokuwiki/doku.php?id=wiki:codestyle` (DokuWiki page `wiki:codestyle`).
The full numbered rule list lives in `references/csharp-rules.md`; read it when a detailed rule check is needed.

## Workflow

When this skill is used for C# code changes:

1. Read the target C# files before editing and preserve existing project patterns unless they conflict with the rules.
2. Load `references/csharp-rules.md` when a detailed rule check is needed.
3. Apply the rules only to the requested or touched code. Avoid unrelated formatting churn.
4. Keep source files UTF-8 with BOM when creating or rewriting C# files.
5. After editing, check for these common violations:
   - Lines longer than 120 characters.
   - Tabs instead of 4 spaces.
   - Constants not named with uppercase and underscores.
   - Static fields not named `s_Name` with `s_` and uppercase first letter after the prefix.
   - Private or protected fields not named `m_Name` with `m_` and uppercase first letter after the prefix.
   - `if`, `for`, and `while` statements whose braces are not on the next line.
   - Missing null checks before using references that may be null.
   - Unnecessary explicit `private` keywords.
   - Non-trivial boolean logic returned directly instead of being assigned to a local variable with clear semantics first, when that would improve breakpoint debugging, value inspection, or later logging.
   - Missing blank line after a multi-line object or collection initializer before the next statement.
   - WorldObj object-reflection parameters without the required `_lowerCamelCase` name.
6. Summarize which code-style rules were applied and mention any rules that were not verifiable locally, such as Unity compilation.

## Naming Quick Reference

- Classes, methods, enum types, and enum values: mixed case, first letter uppercase.
- Properties and public fields: mixed case, first letter lowercase.
- Private and protected fields: `m_` prefix, mixed case, first letter uppercase after the prefix.
- Static fields: `s_` prefix, mixed case, first letter uppercase after the prefix.
- Constants: uppercase words separated by underscores.
- Source file names: mixed case, first letter uppercase; related class files may use underscores.

## Formatting Quick Reference

- Use 4 spaces for indentation, never tabs.
- Put each statement on its own line.
- Add blank lines between functions and between unrelated logic blocks.
- Put spaces around operators and no spaces immediately inside parentheses.
- Use explicit parentheses for expression precedence.
- Keep each line at or under 120 characters.
- For multiline function arguments, align continuation lines clearly with the first argument style.
- For `if`, `for`, and `while`, use a space before `(` and put `{` on the following line.
- For non-trivial boolean logic, prefer assigning the result to a clearly named local variable before returning it, rather than directly returning a long compound condition. Very simple single-condition returns may still return directly.

## Class Layout

Order class contents as:

1. Private fields
2. Protected fields
3. Public fields
4. Public properties
5. Public functions
6. Protected functions
7. System callback functions
8. Custom callback functions
9. Private functions

## Logging And Runtime Safety

- Record error information to logs.
- Prefer project logging APIs such as `Debugger.LogDebug`, `Debugger.LogWarning`, `Debugger.LogError`, and `Debugger.LogInfo` where available.
- Avoid string-concatenation logging that creates unnecessary GC; prefer formatted logging APIs such as `LogDebugF` where available.
- Replace `Action`, `Func`, and `UnityEvent` usage with the project's protected alternatives when the surrounding project already uses them: `Run` with `ERunner.Run`, `RFunc` with `EFuncer.Run`, and `RunEvent`.
