# Avalonia_Stage18_Mobile (skipped)

本仓库只做 **Avalonia 12 桌面主线**。移动端（Android/iOS）需 `net10.0-android/ios` workload，超出本仓库 `LearnCSharp.CI.slnx` 范围（仅编桌面 + 测试项目）。

完整知识点见 `ArchitectureDesign/AvaloniaStudy/第8部分-阶段18-移动端专项.md`（覆盖：工具链与模板、Single-view、Pages 全家、safe area、触摸/手势/Haptics、前后台、进程死亡与恢复、权限、通知、深链、相机/定位、secure storage、Manifest/Info.plist、包体与启动、TalkBack/VoiceOver、商店）。

如需补完，新建独立 worktree 分支并配置对应 workload（`dotnet workload install android ios`），按路线图 §1.1 移动仅 .NET 10 的约束执行。