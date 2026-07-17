# Fill All C# Learn Topics — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans. Steps use checkbox syntax.

**Goal:** Fill all 271 empty LearnCSharp topic `Run()` methods with document-aligned multi-demo examples; build green; commit; push; no PR.

**Architecture:** Keep existing `[LearnTopic]` + `TopicRegistry` shell. Each file becomes a self-contained mini-lesson with `Run` orchestrating private demos. Primary syllabus: `C:\MyFile\ArcForges\ArchitectureDesign\CSharpStudy\*.md`.

**Tech Stack:** C# 14 / .NET 10, single console project `src/LearnCSharp`, no new test framework.

## Global Constraints

- Work only in worktree: `C:\MyFile\ArcForges\LearnCSharp\.worktree\fill-all-csharp-examples`
- Ignore `args`; no `Console.ReadLine`
- Multi-demo density (main + edges + pitfalls)
- Preserve header comments and topic ids
- `dotnet build LearnCSharp.slnx -c Debug` must pass after each stage
- Packages only via existing CPM unless absolutely required
- Intentional exceptions inside try/catch
- Chinese comments OK; English identifiers

## File map

- Modify: every `src/LearnCSharp/Stage*/**/*.cs` placeholder (271)
- Create: none required for topics (optional helper only if unavoidable)
- Docs: `docs/superpowers/specs/2026-07-16-fill-csharp-examples-design.md` (done)

## Template (every topic)

```csharp
// LearnCSharp example (filled)
// Doc / Stage / Section / Item / Topic id — keep existing values

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.StageNN.SectionMM;

internal static class ItemName
{
    [LearnTopic("stageNN/sectionMM/item_slug")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ItemName ===");
        DemoMain();
        DemoEdges();
        return 0;
    }

    private static void DemoMain() { /* document main path */ }
    private static void DemoEdges() { /* boundaries + Debug.Assert */ }
}
```

## Verification commands

```pwsh
dotnet build LearnCSharp.slnx -c Debug
dotnet run --project src/LearnCSharp -c Debug -- stage01/section01/hello_world_dissection
# optional full:
dotnet run --project src/LearnCSharp -c Debug
```

---

### Task 0: Spec committed

- [ ] Ensure design spec exists under `docs/superpowers/`
- [ ] Commit: `docs: add fill-csharp-examples design and plan`

### Task 1: Stage01 (10 files)

**Files:** `src/LearnCSharp/Stage01_SyntaxAndProgramStructure/Section01_LanguageBasics/*.cs`
**Doc:** `CSharp-阶段1-语法基础与程序结构-详解.md`

- [ ] Fill all 10 topics from doc steps 1–10
- [ ] Build Debug
- [ ] Commit: `feat(stage01): fill syntax and program structure examples`

### Task 2: Stage02 (21 files)

**Docs:** 阶段2 第1–4 部分
**Sections:** Foundations, Aggregates, Generics, Memory-oriented

- [ ] Fill all 21
- [ ] Build + commit `feat(stage02): fill type system examples`

### Task 3: Stage03 (23 files)

**Docs:** 阶段3 第1–4 部分

- [ ] Fill all 23
- [ ] Build + commit `feat(stage03): fill members and OOP examples`

### Task 4: Stage04 (12 files)

**Docs:** 阶段4 第1–2 部分

- [ ] Fill all 12
- [ ] Build + commit `feat(stage04): fill control flow and pattern matching examples`

### Task 5: Stage05 (17 files)

**Docs:** 阶段5 第1–3 部分

- [ ] Fill all 17
- [ ] Build + commit `feat(stage05): fill collections LINQ iterator examples`

### Task 6: Stage06 (9 files)

**Docs:** 阶段6 第1–2 部分

- [ ] Fill all 9
- [ ] Build + commit `feat(stage06): fill exceptions and diagnostics examples`

### Task 7: Stage07 (10 files)

**Docs:** 阶段7 第1–2 部分

- [ ] Fill all 10 (async safe for RunAll)
- [ ] Build + commit `feat(stage07): fill async basics examples`

### Task 8: Stage08 (15 files)

**Docs:** 阶段8 第1–2 部分

- [ ] Fill all 15
- [ ] Build + commit `feat(stage08): fill keywords and C#14 examples`

### Task 9: Stage09 (34 files)

**Docs:** 阶段9 第1–6 部分

- [ ] Fill all 34 (HTTP resilient)
- [ ] Build + commit `feat(stage09): fill BCL examples`

### Task 10: Stage10 (28 files)

**Docs:** 阶段10 第1–5 部分

- [ ] Fill all 28 (engineering demos as code commentary + runnable snippets)
- [ ] Build + commit `feat(stage10): fill engineering system examples`

### Task 11: Stage11 (52 files)

**Docs:** 阶段11 第1–10 部分

- [ ] Fill all 52 (runtime concepts via observable demos + comments)
- [ ] Build + commit `feat(stage11): fill runtime expert examples`

### Task 12: Stage12 (20 files)

**Docs:** 阶段12 第1–4 部分

- [ ] Fill all 20 (no long BenchmarkDotNet runs by default)
- [ ] Build + commit `feat(stage12): fill performance line examples`

### Task 13: Stage13 (20 files)

**Docs:** 阶段13 第1–4 部分

- [ ] Fill all 20 (unsafe/PInvoke guarded)
- [ ] Build + commit `feat(stage13): fill metaprogramming and interop examples`

### Task 14: Final verification + push

- [ ] Full `dotnet build -c Debug` and `dotnet build -c Release`
- [ ] Debug RunAll (or sample sweep) — 0 failures target
- [ ] Push `fill-all-csharp-examples` to origin
- [ ] Do **not** open PR

## Self-review checklist

1. Spec coverage: all 13 stages have tasks → yes
2. No TBD placeholders in process → yes
3. Topic ids unchanged across fills → enforce in review
