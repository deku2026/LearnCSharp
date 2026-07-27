# Avalonia_Stage20_EmbeddedLinux (skipped)

本仓库只做 **Avalonia 12 桌面主线**。Embedded Linux 需 `Avalonia.LinuxFramebuffer` + framebuffer/DRM + libinput + 目标硬件（树莓派 Tier 1），超出本仓库 `LearnCSharp.CI.slnx` 范围（仅编桌面 + 测试项目）。

完整知识点见 `ArchitectureDesign/AvaloniaStudy/第8部分-阶段20-Embedded-Linux与工业设备.md`（覆盖：包与后端选择、Framebuffer/DRM/显示管线、libinput 与输入、树莓派与 ARM RID、Kiosk 模式、EGL/软件渲染、触摸校准、屏幕键盘（核心/Pro VirtualKeyboard 分界）、串口/GPIO/FFI、资源约束、AOT、systemd、只读根、watchdog、OTA/回滚）。

如需补完，新建独立 worktree 分支并在目标硬件（树莓派 OS Tier 1）上验证；依赖包含 `libgbm1`、`libgl1-mesa-dri`、`libegl1-mesa`、`libinput10` 等。
