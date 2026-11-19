# GF_X 补丁说明（MUDV587/UnityGameFramework fork）

> 本分支 `feature-upm` = 上游 master `e7eb3ef` + GF_X 本地补丁 + UPM 化脚手架。
> `master` 分支与上游 EllanJiang/UnityGameFramework 保持逐字节镜像，**禁止直接提交**。
> 上游近年未更新（最后推送 2023-05-21）。

## 上游

- 仓库：https://github.com/EllanJiang/UnityGameFramework
- 基线：master `e7eb3ef11393df0ac504204c480cdb839752b05f`（2021-10-28，当前上游最新）

## 一、兼容性/功能补丁（10 处，均条件编译，非对应平台行为不变）

| # | 文件 | 改动 | 来源提交 |
|---|---|---|---|
| 1 | `Scripts/Runtime/Sound/SoundComponent.cs` | `#if UNITY_6000_0_OR_NEWER`：`FindObjectsByType<AudioListener>(FindObjectsSortMode.None)` | `9ad4421` 2025-02-01 |
| 2 | `Scripts/Editor/Misc/ScriptingDefineSymbols.cs` | `#if UNITY_6000_0_OR_NEWER`：`NamedBuildTarget` 版 `PlayerSettings.Get/SetScriptingDefineSymbols` | `e95e0a0` 2025-02-09 |
| 3 | `Scripts/Editor/ResourceBuilder/ResourceBuilderController.cs` | `#if UNITY_2022_3_OR_NEWER`：不再显式传 `DeterministicAssetBundle`（新版本默认） | `e95e0a0`，`fd50732` 2025-05-01 |
| 4 | `Scripts/Runtime/Resource/DefaultLoadResourceAgentHelper.cs` | WebGL：`persistentDataPath` 下直接 `File.ReadAllBytes` 读取（绕开 UnityWebRequest） | `6105626` 2025-06-29，`bbf38a8` |
| 5 | `Scripts/Runtime/Resource/DefaultResourceHelper.cs` | 同上（WebGL 直读分支）+ `using Utility = GameFramework.Utility` | `6105626`，`bbf38a8` |
| 6 | `Scripts/Editor/Misc/Type.cs` | 程序集扫描名单新增 `"GF_X.Runtime"`（原 `"Builtin.Runtime"`，`fcaeff9` 2023-07-20）、`"GF_X.Editor"`（`c155658` 2026-10-16，M3 修复） | `fcaeff9`，`c155658` |
| 7 | `Scripts/Runtime/Debugger/DebuggerComponent.cs` | `#if UNITY_6000_0_OR_NEWER`：`TextEditor` 初始化分支 | `f7b08a4` 2024-07-02 |
| 8 | `Scripts/Runtime/Debugger/DebuggerComponent.QualityInformationWindow.cs` | `#if UNITY_2022_1_OR_NEWER`：`QualitySettings.globalTextureMipmapLimit` | `fcaeff9` |
| 9 | `Scripts/Runtime/Debugger/DebuggerComponent.ScreenInformationWindow.cs` | `#if UNITY_2022_1_OR_NEWER`：`refreshRateRatio` | `fcaeff9` |
| 10 | `Scripts/Runtime/WebRequest/UnityWebRequestAgentHelper.cs` | `#if UNITY_2022_1_OR_NEWER`：`UnityWebRequest.Post` 新重载（contentType 参数） | `fcaeff9` |
| 11 | `Scripts/Runtime/UI/UIComponent.cs` | ✨🆕 `public Transform InstanceRoot => m_InstanceRoot;`（懒创建字段只读公开，供 ugfx `AppSettings` Inspector 使用） | `ebc2837` 2026-11-19 |
| 12 | `Scripts/Editor/Misc/Type.cs` | `internal static class Type` → `public static class Type`；`internal static string GetConfigurationPath<T>()` → `public`（供 ugfx 构建管线直接调用，替代反射私有 API） | `ebc2837` 2026-11-19 |
| 13 | `Scripts/Editor/ResourceEditor/ResourceEditor.cs` | `private static void Open()` → `internal static void Open()`；🆕 配套新增 `ResourceEditorWindow.cs`（public 门面 `ResourceEditorWindow.Open()`） | `ebc2837` 2026-11-19 |

## 二、集成/脚手架变更

| # | 文件 | 改动 | 来源 |
|---|---|---|---|
| 11 | 🆕 `Prefabs/GameFramework.prefab` | 新序列化格式重写（`m_CorrespondingSourceObject` 等），组件/脚本引用内容与上游一致；随包提供 | `fcaeff9` 2023-07-20 |
| 12 | `Scripts/Runtime/UnityGameFramework.Runtime.asmdef` | 完整序列化（`rootNamespace`/`allowUnsafeCode` 等）+ 以 **GUID** 引用 `GameFramework`（`282a0a72b3151df4394127349e04dbbe`） | GF_X 工程提交 `b87fda5` 等 2024-03~08 |
| 13 | `Scripts/Editor/UnityGameFramework.Editor.asmdef` | 同上，`references` 另加 `GameFramework`（**GUID** 引用，与 runtime 风格统一） | 同上 |
| 14 | `package.json` | 依赖重写为核包模型（`com.jiangyin.gameframework 2025.7.10` + 7 modules）；版本 `2025.7.10`；`repository.url` 指本 fork；`revision` 更正为 `e7eb3ef…`（原 `9e04764c…` 为 master 祖先，已过时） | `3077133` + T4/T4.5 |
| 15 | `package.json.meta`、`LICENSE.md.meta` | 仅 `guid:` 行不同（Unity 实例标识） | — |

已知问题：`Type.cs` 扫描名单硬编码 GF_X 专有程序集名（`GF_X.Runtime`/`GF_X.Editor`）——功能依赖（Inspector Helper 下拉、ResourceBuilder IBuildEventHandler 发现），保留现状，见主仓库 `Docs/T10-WorkingTree-Review.md`。asmdef 引用风格（runtime GUID / editor 名字）已于 2026-11-18 统一为 GUID 引用。

## 三、相对上游移除的文件

- `Libraries/GameFramework.dll(+.meta)`、`Libraries/GameFramework.xml(+.meta)` —— 预编译 DLL 弃用，改源码 asmdef 引用
- `Libraries/ICSharpCode.SharpZipLib.dll(+.meta)` —— 改走 NuGetForUnity（`SharpZipLib 0.86.0`，与上游 DLL 位级一致）
- `GameFramework.prefab(+.meta)`（根目录）—— 被 `Prefabs/GameFramework.prefab`（新序列化格式）取代
- `README.md` —— 与核包 README 内容一致，不重复引入
- `README.md.meta` —— 孤立元数据

`LICENSE.md`（2021 版）**未**移除，随包保留（两上游仓内容一致；核包侧仅保留 2013-2020 旧版授权）。

## 四、UPM 化新增

- `Scripts/**/*.meta`、`Prefabs/GameFramework.prefab.meta`、`Scripts.meta`、`Libraries.meta`、`Prefabs.meta`、`LICENSE.md.meta` —— 随包固定 GUID
- `package.json`（+.meta）
- `Libraries/link.xml`（保留上游原件，**与上游逐字节一致**）—— `GameFramework`、`UnityGameFramework.Runtime` preserve=all，IL2CPP 裁剪保护

## 五、合并上游时注意

1. 先按 `--ff-only` 同步 `master`（fork 默认分支，禁止人工提交），再 `master` → `feature-upm` 合并。
2. 补丁重灾区：
   - `Editor/Misc/Type.cs`（程序集扫描名单——上游若改动需与 `GF_X.Runtime`/`GF_X.Editor` 名单合并；另含 `Type` 改为 public 类、`GetConfigurationPath<T>()` 改为 public，上游若改内部类名或方法签名将编译失败，合并时必须评估）
   - `UI/UIComponent.cs`（含 🆕 public `InstanceRoot` 属性）、`ResourceEditor/ResourceEditor.cs`（`Open()` 改 internal）与 🆕 `ResourceEditor/ResourceEditorWindow.cs`（public 门面，上游无此文件，git 不会冲突）
   - 兼容性补丁全部是条件编译块（`#if UNITY_*`），冲突概率低
   - WebGL 直读两处（`DefaultLoadResourceAgentHelper.cs`、`DefaultResourceHelper.cs`）
   - `package.json`（依赖模型已重写，上游改动需整体评估）
   - `Scripts/**.asmdef`（引用方式）
   - `Prefabs/GameFramework.prefab`（新格式；上游若动旧 prefab，手动合并后重新序列化）
3. 上游若**新增文件**，需补 `.meta`（新 GUID）。
4. 验证工具：GF_X 主仓库 `Tools/UpstreamDiff/Compare-Upstream.ps1`。
5. 勿变锚点 GUID：`UnityGameFramework.Runtime.asmdef` = `363c5eb08ff8e6a439b85e37b8c20d96`；`UnityGameFramework.Editor.asmdef` = `a2d8a19598eca814496b089021d08d60`；`Prefabs/GameFramework.prefab` = `adb3eb1c35fcff14f89fba7b05c9d71c`。

## 来源归属

补丁清单由 GF_X 主仓库 `Docs/UpstreamDiff.md` §3.2/§3.3/§3.4 定稿（2026-10-16）并于 2026-11-18 对上游 master 复核一致。
文档同步位置：主仓库 `Docs/UpstreamForks.md`。
