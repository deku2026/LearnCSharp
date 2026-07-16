# Fill All C# Learn Topics — Design Spec

**Date:** 2026-07-16  
**Branch / worktree:** `fill-all-csharp-examples` @ `.worktree/fill-all-csharp-examples`  
**Status:** Approved

## Goal

Fill all **271** empty `Run()` placeholders under `src/LearnCSharp/Stage0*`–`Stage13*` with rich, document-aligned, compile-clean example code (beginner → expert coverage per topic). Map each `.cs` to its `// Doc :` markdown under `C:\MyFile\ArcForges\ArchitectureDesign\CSharpStudy`. Commit and push the branch; **no PR**.

## Non-goals

- Stage 14 Unity appendix (not scaffolded).
- New test frameworks.
- Changing topic registry / CLI semantics (args unused; keep existing shell).
- Opening a PR.

## Constraints (from product / user)

| Item | Value |
|------|--------|
| Runtime | .NET 10 / C# 14 (`Directory.Build.props`) |
| Density | Document-full multi-demo per topic (main path + edges + pitfalls) |
| Args | Ignore `args`; no interactive `ReadLine` |
| Style | Keep `[LearnTopic]` / `static int Run(string[] args)` / header comments |
| Quality | `dotnet build` must pass; Debug RunAll should not hang or throw uncaught |
| Git | Local commits per stage batch; final `push`; no PR |

## Architecture

Existing single exe + reflection registry stays:

```
Program.cs → TopicRegistry → [LearnTopic] static int Run(string[])
```

Each topic file becomes a self-contained mini-lesson:

1. Preserve header (`Doc` / `Stage` / `Section` / `Item` / `Topic id`).
2. `Run` orchestrates ordered demos.
3. Private static helpers or local functions hold each demo block.
4. Output via `Console.WriteLine` section headers; self-check via `System.Diagnostics.Debug.Assert`.
5. Intentional exception demos only inside `try/catch` so `RunAll` stays green.

### Canonical skeleton

```csharp
// LearnCSharp example (filled)
// Doc      : <existing>
// Stage    : <existing>
// Section  : <existing>
// Item     : <existing>
// Topic id : <existing>

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
        DemoMainPath();
        DemoEdges();
        DemoPitfalls();
        return 0;
    }

    private static void DemoMainPath() { /* ... */ }
    private static void DemoEdges() { /* ... */ }
    private static void DemoPitfalls() { /* ... */ }
}
```

Nested types needed for OOP demos live in the same file as `private`/`file` types when possible to avoid cross-topic collisions.

## Content sources (priority)

1. Matching markdown in `ArchitectureDesign/CSharpStudy` (primary syllabus).
2. Microsoft Learn C# language reference / .NET API docs.
3. Well-known teaching patterns (Skeet / Albahari-style minimal contrasts) when docs are conceptual-only.
4. SharpLab-style “what it lowers to” explained in comments where relevant (no runtime dependency on SharpLab).

## Inventory

| Stage | Count | Doc pattern |
|-------|------:|-------------|
| 01 Syntax | 10 | 阶段1 详解 |
| 02 Type system | 21 | 阶段2 第1–4 部分 |
| 03 Members/OOP | 23 | 阶段3 第1–4 部分 |
| 04 Control/patterns | 12 | 阶段4 |
| 05 Collections/LINQ | 17 | 阶段5 |
| 06 Exceptions | 9 | 阶段6 |
| 07 Async | 10 | 阶段7 |
| 08 Keywords/C#14 | 15 | 阶段8 |
| 09 BCL | 34 | 阶段9 |
| 10 Engineering | 28 | 阶段10 |
| 11 Runtime expert | 52 | 阶段11 |
| 12 Performance | 20 | 阶段12 |
| 13 Metaprogramming | 20 | 阶段13 |
| **Total** | **271** | |

## Execution approach (approved)

**Approach A:** Unified template + fill by stage batches (01→13). Each batch:

1. Read corresponding study markdown(s).
2. Fill every `.cs` in that stage with multi-demo content.
3. `dotnet build LearnCSharp.slnx -c Debug`.
4. Smoke: `dotnet run --project src/LearnCSharp -c Debug -- <one topic>` and optionally short RunAll.
5. Commit: `feat(stageNN): fill learn topic examples`.

Parallel subagents may own different stages only when they do not share files; merge by sequential build gates.

## Special handling

| Area | Rule |
|------|------|
| `unsafe` / pointers | Stay inside Stage13 Section04; enable only if project already allows or use safe demos + comments |
| P/Invoke | Prefer documented safe patterns; skip requiring missing native DLLs at runtime (guard with try or `OperatingSystem`) |
| HTTP | Prefer local/mock or `HttpClient` against well-known public endpoints with short timeout; catch network failures so RunAll continues |
| BenchmarkDotNet | Demo setup/API shapes without long benchmark runs in default path |
| Source generators | Explain pipeline with sample Roslyn-oriented code that compiles as educational snippets (not a second project unless needed) |
| Async | Use `.GetAwaiter().GetResult()` only when necessary; prefer `async` local helpers invoked via `Run` returning after completion; avoid deadlocks |

## Success criteria

- [ ] All 271 topics have non-empty educational demos (not `_ = args; return 0;` only).
- [ ] `dotnet build` Debug succeeds with zero errors.
- [ ] Debug RunAll completes without hang; failures only if intentionally unfixed (target: 0).
- [ ] Branch pushed to `origin/fill-all-csharp-examples`.
- [ ] No PR opened.

## Out of scope follow-ups

- Adding unit tests per topic.
- Publishing docs site.
- Stage14 Unity examples in this repo.
