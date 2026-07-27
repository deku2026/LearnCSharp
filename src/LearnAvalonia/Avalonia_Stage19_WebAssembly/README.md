# Avalonia_Stage19_WebAssembly (skipped)

本仓库只做 **Avalonia 12 桌面主线**。WebAssembly 需 `Avalonia.Browser` + `net10.0-browser` TFM，超出本仓库 `LearnCSharp.CI.slnx` 范围（仅编桌面 + 测试项目）。

完整知识点见 `ArchitectureDesign/AvaloniaStudy/第8部分-阶段19-WebAssembly专项.md`（覆盖：Browser 项目、bootstrap、CanvasKit/WebGL、事件循环、线程现状、存储、CORS、OAuth、文件/剪贴板限制、路由与历史、PWA、压缩/CDN、启动载荷、trim、调试、JS interop、a11y）。

如需补完，新建独立 worktree 分支并配置 wasm workload，按路线图 §1.1 Browser 最低 .NET 8.0 的约束执行；第三方库必须查 WASM 兼容性。