# CLAUDE.md — com.spacetime.gpuskin

## 项目概述

**SpaceTime GPUSkin** 是一个面向 Unity（2020.3+）的 GPU 蒙皮与大批量角色渲染 UPM 包。核心思路：

1. **烘焙阶段（Editor）**：用 `Animator.StartRecording/StopRecording` 按帧采集骨骼矩阵或顶点位置，写入贴图（Texture2D）并持久化为 Asset。
2. **运行时（Runtime）**：Shader 按「动画像素偏移 + 帧索引 + 骨骼/顶点索引」采样贴图，不再依赖 `Animator.Update` 逐帧解算。
3. **批量渲染**：通过 GPU Instancing + `MaterialPropertyBlock` 合批，少量 Draw Call 渲染大量同材质角色。
4. **全局帧驱动**：`GPUSkinMgr` 单例以 30 FPS 刷新 Shader 全局属性 `g_GpuSkinFrameIndex`，统一驱动所有实例，避免每实例脚本 Update 放大开销。

命名空间：`ST.GPUSkin`

---

## 目录结构

```
Packages/com.spacetime.gpuskin/
├── package.json                          # UPM 包元数据
├── README.md                             # 详细设计文档（含图）
├── CLAUDE.md                             # 本文件
├── readme/tex/                           # 文档配图（仅文档用，不运行时加载）
│
├── Runtime/
│   ├── com.spacetime.gpuskin.runtime.asmdef
│   └── Scripts/
│       ├── Common/
│       │   └── GPUSkinDefine.cs          # Shader PropertyID 常量、编译宏名
│       ├── DB/
│       │   ├── GPUSkinBoneInfoDB.cs      # 骨骼信息 ScriptableObject DB
│       │   ├── GPUSkinVertexInfoDB.cs    # 顶点信息 ScriptableObject DB
│       │   ├── GPUSkinInfoDB.cs          # 通用信息 DB 基类
│       │   └── GPUSkinGenConfigDB.cs     # 烘焙生成配置 DB
│       └── Logic/
│           ├── GPUSkinPlayerBase.cs      # Player 基类（运行时）
│           ├── GPUSkinPlayerBase_EditMode.cs  # Player 基类（编辑器模式扩展）
│           ├── GPUSkinBonePlayer.cs      # 骨骼动画播放组件
│           ├── GPUSkinBonePlayer_EditMode.cs
│           ├── GPUSkinVertexPlayer.cs    # 顶点动画播放组件
│           ├── GPUSkinVertexPlayer_EditMode.cs
│           └── GPUSkinMgr.cs            # 全局帧管理单例
│
├── Editor/
│   ├── com.spacetime.gpuskin.editor.asmdef
│   └── Scripts/
│       ├── Inspector/
│       │   ├── GPUSkinBonePlayerInspector.cs
│       │   └── GPUSkinVertexPlayerInspector.cs
│       ├── Tools/
│       │   ├── GPUSkinTool.cs            # 烘焙工具入口（Editor 菜单）
│       │   ├── GPUSkinBoneTool.cs        # 骨骼动画烘焙逻辑
│       │   └── GPUSkinVertexTool.cs      # 顶点动画烘焙逻辑
│       └── Utils/
│           └── GPUSkinEditorUtils.cs     # Editor 工具辅助方法
│
└── Shaders/
    ├── com.spacetime.gpuskin.shaders.asmdef
    ├── GPUSkinShaders.cs                 # Shader 路径/引用常量
    └── GPUSkin/
        ├── GPUSkinBone.shader            # 骨骼蒙皮 Shader
        └── GPUSkinVertex.shader          # 顶点动画 Shader
```

---

## 关键设计约定

### 编译宏 `ST_GAME_MODE`

`GPUSkinMgr.OnUpdate()` 中的全局帧刷新逻辑被 `#if ST_GAME_MODE` 包裹。
- **未定义**（默认）：编辑器预览模式，不驱动全局帧，Player 组件自行管理帧状态。
- **已定义**：正式游戏模式，由 `GPUSkinMgr.S.OnUpdate()` 统一刷新 `g_GpuSkinFrameIndex`。

需在 **Player Settings → Other Settings → Scripting Define Symbols** 中添加 `ST_GAME_MODE` 才能激活。

### Shader 属性 ID（`GPUSkinDefine`）

所有常量均为 `public static readonly`，以 `s_GpuSkin_` 开头。

| 常量名 | Shader 属性 | 说明 |
|--------|-------------|------|
| `s_GpuSkin_Shader_Common_CurFrame` | `_CurFrame` | 当前帧索引（实例级） |
| `s_GpuSkin_Shader_Common_CurFramePixelIndex` | `_CurFramePixelIndex` | 当前帧在贴图中的像素偏移 |
| `s_GpuSkin_Shader_Common_CurFrameCount` | `_CurFrameCount` | 当前动画总帧数 |
| `s_GpuSkin_Shader_Common_GlobalFrameIndex` | `g_GpuSkinFrameIndex` | **全局帧**（`Shader.SetGlobalInt`） |
| `s_GpuSkin_Shader_Bone_AnimTex` | `_BoneAnimTex` | 骨骼动画贴图 |
| `s_GpuSkin_Shader_Bone_AnimTexWidth` | `_BoneAnimTexWidth` | 骨骼贴图宽度 |
| `s_GpuSkin_Shader_Bone_AnimTexHeight` | `_BoneAnimTexHeight` | 骨骼贴图高度 |
| `s_GpuSkin_Shader_Bone_Count` | `_BoneCount` | 骨骼数量 |
| `s_GpuSkin_Shader_Vertex_AnimTex` | `_VertexAnimTex` | 顶点动画贴图 |
| `s_GpuSkin_Shader_Vertex_AnimTexWidth` | `_VertexAnimTexWidth` | 顶点贴图宽度 |
| `s_GpuSkin_Shader_Vertex_AnimTexHeight` | `_VertexAnimTexHeight` | 顶点贴图高度 |
| `s_GpuSkin_Shader_Vertex_Count` | `_VertexCount` | 顶点数量 |
| `s_GpuSkin_GameModeDefine` | — | 编译宏名 `"ST_GAME_MODE"`（`string`） |

### 骨骼 vs 顶点烘焙选择

| | 骨骼烘焙 | 顶点烘焙 |
|---|---|---|
| 贴图大小 | 相对小 | 逐顶点存位置，较大 |
| Shader 复杂度 | 矩阵采样 + 蒙皮加权 | 直接采样顶点位置，更简单 |
| Mesh 要求 | 需 uv2（骨骼索引）、uv3（权重） | 无额外 uv 要求 |
| 适用场景 | 骨骼数少、顶点数多 | 顶点数有上限（≈1500 级）、动画短 |

---

## 开发规范

### 代码风格
- **命名空间**：所有 Runtime 与 Editor 脚本统一使用 `namespace ST.GPUSkin`。
- **类命名**：`GPUSkin` 前缀 + 模块名，如 `GPUSkinBonePlayer`、`GPUSkinMgr`。
- **单例**：使用静态属性 `S`（`GPUSkinMgr.S`），懒初始化，不继承 `MonoBehaviour`，由外部 MonoBehaviour 驱动 `OnUpdate()`。
- **Shader PropertyID**：所有 `Shader.PropertyToID` 缓存在 `GPUSkinDefine` 的 `static readonly int` 字段，禁止在其他地方直接传字符串。

### 文件组织
- **DB 类**（ScriptableObject）放 `Runtime/Scripts/DB/`，对应运行时数据资产。
- **Player 组件**放 `Runtime/Scripts/Logic/`；EditMode 扩展方法用 `partial class` 或单独 `_EditMode.cs` 文件，加 `#if UNITY_EDITOR` 守卫。
- **烘焙工具**放 `Editor/Scripts/Tools/`；Inspector 扩展放 `Editor/Scripts/Inspector/`。
- **Shader** 放 `Shaders/GPUSkin/`；每个 Shader 对应一种烘焙类型。

### .meta 文件
Unity UPM 包中所有文件均有对应 `.meta`，**提交时必须同步维护**，不得遗漏或手工删除。

### 不要做的事
- 不要在 `Shaders/` 或 `readme/tex/` 目录下的资产中添加 `Resources.Load` 路径依赖——readme 图片仅供文档展示。
- 不要在 Runtime 代码中直接 `using UnityEditor`；Editor 专属逻辑只能放在 `Editor/` 程序集或 `#if UNITY_EDITOR` 块内。
- 不要将 `GPUSkinMgr` 改为 `MonoBehaviour`；保持纯 C# 单例，由宿主工程的 MonoBehaviour 调用其 `OnUpdate()`。

---

## 常见任务快速参考

### 新增一种动画烘焙类型
1. 在 `Runtime/Scripts/DB/` 添加对应 `GPUSkin<Type>InfoDB.cs`（ScriptableObject）。
2. 在 `Runtime/Scripts/Logic/` 添加 `GPUSkin<Type>Player.cs` 与 `GPUSkin<Type>Player_EditMode.cs`。
3. 在 `Editor/Scripts/Tools/` 添加 `GPUSkin<Type>Tool.cs`，实现烘焙逻辑，并在 `GPUSkinTool.cs` 中注册菜单项。
4. 在 `Editor/Scripts/Inspector/` 添加对应 Inspector。
5. 在 `Shaders/GPUSkin/` 添加对应 `.shader` 文件。
6. 在 `GPUSkinDefine.cs` 补充新的 Shader PropertyID 常量。

### 调试全局帧不更新
- 确认 `ST_GAME_MODE` 宏已定义。
- 确认宿主 MonoBehaviour 每帧调用了 `GPUSkinMgr.S.OnUpdate()`。
- 在 Shader 中检查 `g_GpuSkinFrameIndex` 是否被正确读取（全局属性，无需 `UNITY_ACCESS_INSTANCED_PROP`）。

### 添加新 Shader 属性
1. 在 `GPUSkinDefine.cs` 中添加 `public static readonly int s_GpuSkin_Shader_<Category>_<Name> = Shader.PropertyToID("_Xxx");`。
2. 在 Player 组件中通过 `MaterialPropertyBlock.SetXxx(GPUSkinDefine.s_GpuSkin_Shader_<Category>_<Name>, value)` 赋值。
3. 在 Shader 的 `UNITY_INSTANCING_BUFFER` 中声明对应属性（如果是实例级属性）。
