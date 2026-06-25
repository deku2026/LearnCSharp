# Stage 01 · 语法基础与程序结构

- 路线图源文档: `CSharp-阶段1-语法基础与程序结构-详解.md` (在 `C:/MyFile/ArcForges/ArchitectureDesign/CSharpStudy/` 下找原始中文版)
- 主路线索引: `C:/MyFile/ArcForges/ArchitectureDesign/CSharpStudy/CSharp-dotNET-完整学习路线图.md`

## 目标

把 C++ 能迁移的语法迅速对上, 并建立 「源代码 → 程序集 + 元数据 → 运行时加载 → JIT → 执行」 整体心智 (没有头文件 / 没有独立链接器 / 没有预处理器宏).

## 子目录 (sectionNN_xxx, 按文档大项顺序)

```
src/LearnCSharp/Stage01_SyntaxAndProgramStructure/
└── Section01_LanguageBasics/   ← 整个阶段单一 section, 10 个 step 在此
```

## Topic 命名约定

`stage01/section01/<item_slug>` (snake_case slug 方便 CLI 输入).

## 一个占位的学习节奏

1. 读 `learn.microsoft.com/dotnet/csharp/...` 对应页 + 路线图本节;
2. 在 `sharplab.io` 看 IL/lowered, `BenchmarkDotNet` 量化性能;
3. 写最小例子, 把空 `Run()` 填上真实代码;
4. 跑这一个 topic:
   ```
   dotnet run --project src/LearnCSharp -c Debug -- stage01/section01/<item_slug>
   ```
5. 留意 C++ / 旧 .NET 对照, 用 NRT / Analyzer 复查.
