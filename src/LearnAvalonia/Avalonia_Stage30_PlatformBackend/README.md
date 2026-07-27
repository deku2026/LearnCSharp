# Avalonia_Stage30_PlatformBackend

对应文档：`ArchitectureDesign/AvaloniaStudy/第11部分-阶段30-平台后端选读.md`

Stage 30 · 平台后端选读 —— Avalonia 12.1 桌面后端。本应用以 nav-shell 方式承载 11 个知识点 demo，覆盖桌面 5 后端（Win32 / WinUI Composition / macOS / X11 / Headless）；Android / iOS / Browser-WASM 等 mobile/WASM 分支按桌面策略不在此 build/run，标记为"见 roadmap"。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Stage30_PlatformBackend -c Debug
```

## 知识点（11）

- §1 概览
- §2 Win32
- §3 WinUI Composition
- §4 macOS (Avalonia.Native / AppKit)
- §5 X11 (Linux 默认)
- §6 Wayland (实验 🧪)
- §7 Android (非桌面 — 见 roadmap)
- §8 iOS (非桌面 — 见 roadmap)
- §9 Browser/WASM (非桌面 — 见 roadmap)
- §10 Headless (桌面/测试)
- §11 平台总览

详见 `Docs/Stage30.md`。