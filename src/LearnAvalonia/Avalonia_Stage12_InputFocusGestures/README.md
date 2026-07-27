# Avalonia_Stage12_InputFocusGestures

对应文档：`ArchitectureDesign/AvaloniaStudy/第5部分-阶段12-输入焦点手势与IME.md`

Stage 12 · 输入、焦点、手势与 IME —— Avalonia 12 路由事件（tunnel/bubble/direct）、指针（mouse/touch/pen、Capture、CaptureLost）、InputElement 手势事件（Tapped/DoubleTapped/Holding）、键盘与热键（KeyGesture/HotKey）、焦点（FocusScope/FocusManager/GotFocus NavigationMethod）、Access Keys 按符号匹配、IME 与 TextInput、拖放 DnD（IAsyncDataTransfer/DoDragDropAsync）、剪贴板（IClipboard SetTextAsync/TryGetTextAsync）、PlatformFeedback、浏览器限制、命令集成、调试方法。本应用以 nav-shell 方式承载 18 个知识点 demo，左侧 ListBox 切换章节，右侧 ScrollViewer 渲染对应 demo，底部显示当前章节的文档路径与验收标准。

## 运行

```pwsh
dotnet run --project src/LearnAvalonia/Avalonia_Stage12_InputFocusGestures -c Debug
```

## 知识点（18）

- §1 为什么输入是专家分水岭
- §2.1 路由事件：direct / tunnel / bubble
- §2.2 指针：mouse / touch / pen
- §2.3 手势事件在 InputElement
- §2.4 键盘与热键
- §2.5 焦点：scopes、Tab、FocusManager
- §2.6 Access Keys（按符号）
- §2.7 IME 与文本输入
- §2.8 拖放 DnD 与 IAsyncDataTransfer
- §2.9 剪贴板（IAsyncDataTransfer）
- §2.10 PlatformFeedback（触觉/声音）
- §2.11 浏览器限制
- §2.12 与命令系统集成
- §2.13 调试输入的方法
- §3 跨平台差异
- §4 Avalonia 12 / 12.1 变化
- §5 过时与禁止
- §6 最小实验清单

详见 `Docs/Stage12.md`。
