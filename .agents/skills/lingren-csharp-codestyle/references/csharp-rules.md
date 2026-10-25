# LingRen C# Code Style Rules

Source: `http://192.168.0.211/dokuwiki/doku.php?id=wiki:codestyle`, DokuWiki page `wiki:codestyle`.

1. Source files use UTF-8 with BOM.
2. Source file names use mixed case with an uppercase first letter. Related files of the same class may use underscores, for example `Body.cs` and `Body_Action.cs`.
3. Indentation is 4 spaces. Use spaces only.
4. Classes and functions use mixed case with an uppercase first letter.
5. Enum types and enum members use mixed case.
6. Function names and each statement should occupy their own line. Empty functions may use one line.
7. Private and protected fields start with `m_`, use mixed case, and the first letter after the prefix is uppercase.
8. Properties and public fields use mixed case with a lowercase first letter.
9. Property `get` and `set` bodies may be one line when short; otherwise braces must be on separate lines.
10. Use public fields with discretion.
11. Separate functions, and unrelated logic within functions, with blank lines.
12. Put one space between operators and operands, for example `x + y`, not `x+y`.
13. Do not put spaces immediately inside parentheses, for example `(x + y)`, not `( x + y )`.
14. Use parentheses to make expression precedence explicit, for example `(x * y) + 1`.
15. Add appropriate comments when helpful. Comments may be Chinese or English.
16. Error information must be recorded to log files.
17. Keep each line at or under 120 characters.
18. When long function parameter lists wrap, align according to the first parameter. Avoid excessive parameters.
19. Keep function bodies reasonably short; a single function should not exceed 100 lines.
20. `if`, `for`, and `while` require a space before `(`, and `{` begins on the next line.
21. Class member order is private fields, protected fields, public fields, public properties, public functions, protected functions, system callbacks, custom callbacks, private functions.
22. String objects should remain non-null throughout their lifecycle.
23. References that may be null during their lifecycle must be null-checked before use.
24. Do not write the `private` keyword unless necessary.
25. Internal access to public properties should use `this`.
26. When there are many data members, use `#region` to group them.
27. Public fields should generally appear only in data-only classes or structs.
28. If an inner class is not just a data structure, move it to a separate `.cs` file.
29. Constants use uppercase words separated by underscores, for example `INVALID_REQUEST_ID`.
30. Internal fields that use default values should generally omit explicit default assignments.
31. Static fields start with `s_`, use mixed case, and the first letter after the prefix is uppercase, for example `s_AttachListHead`.
32. `Action`, `Func`, and `UnityEvent` have exception-safety and GC concerns. Use project alternatives when applicable: `Run` with `ERunner.Run`, `RFunc` with `EFuncer.Run`, and `RunEvent`.
33. Logging API guidance: use `Debugger.LogDebug` for debug information, `Debugger.LogWarning` for general configuration/null/server data problems, `Debugger.LogError` for severe blocking errors, and `Debugger.LogInfo` for critical flow points. Prefer formatted logging such as `Debugger.LogDebugF("ABC {0}", m_Level)` to avoid string concatenation GC.
34. A call that returns a value must not be used directly as an argument to another call, and must not be called directly inside an `if` / `else if` / `while` condition. Assign the result to a semantically named local variable first, then pass or test that variable.
    Reason: the value stays visible at the call site, so it can be inspected in a breakpoint, logged, or asserted without stepping into the callee.
    Example:
    - Wrong: `block.SetLODNum(TreeUtils.CalcRealLODNum(_res.lodNum));`
    - Right: `int realLODNum = TreeUtils.CalcRealLODNum(_res.lodNum);` then `block.SetLODNum(realLODNum);`
    - Wrong: `return calcJob.Schedule(_group.count, CalcInnerLoopCount(_group.count));`
    - Right: `int innerLoopCount = CalcInnerLoopCount(_group.count);` then `return calcJob.Schedule(_group.count, innerLoopCount);`
    - Wrong: `if (FindGroup(id) != null)`
    - Right: `var group = FindGroup(id);` then `if (group != null)`
35. A return value taken from a property chain or a non-trivial expression, for example `m_Renderer.bounds.center.y` or `transform.position.y`, must be assigned to a clearly named local variable before it is returned. A direct field read may be returned directly. Apply the same pattern when a getter delegates to a helper method.
36. After a multi-line object or collection initializer assignment ends with `};`, leave one blank line before the next statement.

37. For WorldObj object-reflection code under `Assets/LingRen/Script/WaterReflection/**/WorldObj/**`, method parameters, including `out` and `ref` parameters, use a leading underscore followed by lower camel case. For example, use `Camera _sourceCamera` instead of `Camera sourceCamera`. XML `<param name="...">` entries must use the same name.

    ```csharp
    WaterReflLayerDisScale layerCullDisScale = new WaterReflLayerDisScale
    {
        layer = source.layer,
        disScale = source.disScale
    };

    m_LayerCullDisScaleList.Add(layerCullDisScale);
    ```


