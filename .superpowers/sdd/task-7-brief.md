## Task 7: Stage21–22 + Capstone2 fill

**Projects + counts:**
- Stage21 AppArchitecture — 20
- Stage22 DataNetworkAuth — 19
- Capstone2 ComplexDesktop — 15

Total: 54 demo pages.

Docs:
- `第9部分-阶段21-应用架构与工程结构.md` → 20
- `第9部分-阶段22-数据存储网络与认证.md` → 19
- `第9部分-Capstone2-复杂桌面生产应用.md` → 15

Stage21 csproj adds `Microsoft.Extensions.Hosting` (in CPM) — actually already referenced via existing CPM entries; just add PackageReference.
Stage22 csproj adds `Microsoft.Data.Sqlite` + `Microsoft.EntityFrameworkCore.Sqlite` + `Microsoft.Extensions.Http`.

- [ ] **Step 1: Apply template + create demos for all 3 projects**

- [ ] **Step 2: Build green**

Run: `dotnet build LearnCSharp.CI.slnx -c Release --nologo`
Expected: "0 个警告 0 个错误".

- [ ] **Step 3: pre-commit + commit**

```pwsh
pre-commit run --files src/LearnAvalonia/Avalonia_Stage21_AppArchitecture src/LearnAvalonia/Avalonia_Stage22_DataNetworkAuth src/LearnAvalonia/Avalonia_Capstone2_ComplexDesktop
git add <those dirs>
git commit -m "feat: fill Stage21-22 + Capstone2 demos (54 knowledge-point pages)"
```

---

