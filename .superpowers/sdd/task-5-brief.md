## Task 5: Stage10–14 fill

**Projects + counts:**
- Stage10 WindowsDialogs — 19
- Stage11 PagesNavigation — 18
- Stage12 InputFocusGestures — 18
- Stage13 GraphicsAnimation — 21
- Stage14 CustomControls — 20

Total: 96 demo pages.

Docs:
- `第5部分-阶段10-桌面窗口对话框与系统集成.md` → 19
- `第5部分-阶段11-Pages与导航架构.md` → 18
- `第5部分-阶段12-输入焦点手势与IME.md` → 18
- `第5部分-阶段13-图形动画与效果.md` → 21
- `第6部分-阶段14-自定义控件.md` → 20

Apply nav shell, one demo per section.

- [ ] **Step 1: Apply template + create demos for all 5 projects**

- [ ] **Step 2: Build green**

Run: `dotnet build LearnCSharp.CI.slnx -c Release --nologo`
Expected: "0 个警告 0 个错误".

- [ ] **Step 3: pre-commit + commit**

```pwsh
pre-commit run --files src/LearnAvalonia/Avalonia_Stage10_WindowsDialogs src/LearnAvalonia/Avalonia_Stage11_PagesNavigation src/LearnAvalonia/Avalonia_Stage12_InputFocusGestures src/LearnAvalonia/Avalonia_Stage13_GraphicsAnimation src/LearnAvalonia/Avalonia_Stage14_CustomControls
git add <those dirs>
git commit -m "feat: fill Stage10-14 demos (96 knowledge-point pages)"
```

---

