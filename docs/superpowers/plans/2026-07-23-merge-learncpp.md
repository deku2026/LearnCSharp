# Merge LearnCpp into LearnCSharp — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Fold the entire LearnCpp C++23 repo into LearnCSharp under `src/learncpp/`, wired into `LearnCSharp.slnx` and per-platform CMake CI, without disturbing the .NET build.

**Architecture:** Nest the whole LearnCpp tree (CMake + VS + sources + tooling) under `src/learncpp/` so its internal relative paths (`vs/..`, `src/*.cpp`, `%~dp0..`) keep working unchanged. Rename the VS project to `cpp_learn`. Add one CMake job to each OS CI workflow. Merge root dotfile configs.

**Tech Stack:** C++23 (clang-cl/clang), CMake 3.28 + presets + Ninja + sccache, MSBuild vcxproj (ClangCl), slnx, GitHub Actions, pre-commit.

## Global Constraints

- Working dir = worktree `C:/MyFile/ArcForges/LearnCSharp/.worktree/merge-learncpp` (branch `merge-learncpp`). Source = `C:/MyFile/ArcForges/LearnCpp`.
- Do NOT modify `LearnCSharp.CI.slnx`. Do NOT add CodeQL cpp language. Do NOT build the `.slnx`. Do NOT open a PR.
- VS project identity → `cpp_learn`; CMake target/exe stays `learn_cpp` (CI smoke + `main.cpp` reference it).
- C++ CI = CMake only (drop LearnCpp's `slnx-msbuild` VS job). Smoke asserts `learn_cpp: 874 topics registered`.
- Local verify env: `%comspec% /k "C:\Program Files\Microsoft Visual Studio\18\Community\VC\Auxiliary\Build\vcvars64.bat"`.

---

### Task 1: Copy LearnCpp tree into src/learncpp

**Files:**
- Create: `src/learncpp/**` (from LearnCpp tracked files: CMakeLists.txt, CMakePresets.json, README.md, cmake/, docs/, include/, scripts/, src/, vs/, .clang-format, .clang-format-ignore, .clang-tidy, .clangd, .editorconfig)

- [ ] **Step 1:** Export LearnCpp tracked files into `src/learncpp` via `git archive` (excludes build/.vs/.worktree and root dotfiles handled elsewhere):
```bash
DST="C:/MyFile/ArcForges/LearnCSharp/.worktree/merge-learncpp/src/learncpp"
mkdir -p "$DST"
git -C "C:/MyFile/ArcForges/LearnCpp" archive --format=tar HEAD \
  CMakeLists.txt CMakePresets.json README.md cmake docs include scripts src vs \
  .clang-format .clang-format-ignore .clang-tidy .clangd .editorconfig | tar -x -C "$DST"
```
- [ ] **Step 2:** Strip `root = true` from `src/learncpp/.editorconfig` so it cascades with the repo root.
- [ ] **Step 3:** Verify: `find "$DST" -name '*.cpp' | wc -l` → 876; `ls "$DST"` shows the expected entries; no `.git` inside.

---

### Task 2: Rename vcxproj → cpp_learn and regenerate

**Files:**
- Modify: `src/learncpp/vs/learn_cpp.vcxproj` → `cpp_learn.vcxproj` (+ `.filters`, `.user`)
- Modify: `src/learncpp/scripts/generate-vs-filters.ps1` (`$projectFile`, `$filtersFile`)

- [ ] **Step 1:** `git mv` (or mv) the three `vs/learn_cpp.vcxproj*` files to `cpp_learn.vcxproj*`.
- [ ] **Step 2:** In `cpp_learn.vcxproj`, set `<RootNamespace>cpp_learn</RootNamespace>` (keep ProjectGuid).
- [ ] **Step 3:** In `scripts/generate-vs-filters.ps1`, change `$projectFile = Join-Path $RepoRoot 'vs\cpp_learn.vcxproj'` and `$filtersFile = ...'vs\cpp_learn.vcxproj.filters'`.
- [ ] **Step 4:** Run `pwsh src/learncpp/scripts/generate-vs-filters.ps1` → "ClCompile: 876  ClInclude: 2"; confirm vcxproj/filters regenerated cleanly.

---

### Task 3: Add cpp_learn to LearnCSharp.slnx

**Files:**
- Modify: `LearnCSharp.slnx` (add `/src/learncpp/` folder + project)

- [ ] **Step 1:** Confirm VS2026 slnx schema for a native vcxproj + per-project platform mapping (research/inspect).
- [ ] **Step 2:** Insert before `</Solution>`:
```xml
  <Folder Name="/src/learncpp/">
    <Project Path="src/learncpp/vs/cpp_learn.vcxproj" Id="44b4c0d0-2a55-4a73-b803-cc04213d600b" Type="Native">
      <Platform Solution="Any CPU" Project="x64" />
    </Project>
  </Folder>
```
(adjust `Type`/mapping to the verified schema so the x64-only vcxproj builds under Debug/Release × Any CPU/x64).
- [ ] **Step 3:** Validate XML well-formedness.

---

### Task 4: CI — windows-ci.yml cpp-cmake job

**Files:**
- Modify: `.github/workflows/windows-ci.yml`

- [ ] **Step 1:** Append a `cpp-cmake` job adapted from LearnCpp's windows `build` job: `runs-on: windows-2025-vs2026`, msvc-dev-cmd (x64, VS18), get-cmake, put VS2026 bundled clang-cl on PATH, sccache + cache keyed on `src/learncpp/…`, `cmake --preset windows-ci` build with `working-directory: src/learncpp`, smoke-run `build/windows-ci/bin/learn_cpp.exe` asserting 874 topics, pre-commit with `SKIP: dotnet-format`. No slnx-msbuild job.

---

### Task 5: CI — linux-ci.yml cpp-cmake job

**Files:**
- Modify: `.github/workflows/linux-ci.yml`

- [ ] **Step 1:** Append `cpp-cmake` job: `ubuntu-24.04`, apt.llvm.org clang 22 + ninja, get-cmake, sccache, libstdc++/libc++ matrix, `cmake --preset linux-ci` (`working-directory: src/learncpp`), 874-topic smoke, pre-commit `SKIP: dotnet-format` on libstdc++ leg. No services.

---

### Task 6: CI — macos-ci.yml cpp-cmake job

**Files:**
- Modify: `.github/workflows/macos-ci.yml`

- [ ] **Step 1:** Append `cpp-cmake` job: `macos-14`, get-cmake, Homebrew `llvm@22`, sccache, `cmake --preset macos-ci` (`working-directory: src/learncpp`), 874-topic smoke, pre-commit (pipx) `SKIP: dotnet-format`.

---

### Task 7: Merge root configs

**Files:**
- Modify: `.pre-commit-config.yaml`, `.gitattributes`, `.codespell-ignore`, `.gitignore`, `.github/workflows/codeql.yml`

- [ ] **Step 1:** `.pre-commit-config.yaml` → union: exclude adds `src/.*/Generated/.*` + `\.cache/.*`; generic hooks add `check-toml`, `check-yaml --allow-multiple-documents`, `mixed-line-ending` exclude `\.(bat|cmd|ps1|vcxproj|filters)$`, union `check-executables-have-shebangs` exclude; codespell rev v2.4.3; add `mirrors-clang-format` v22.1.4 (c/cpp extensions); keep local `dotnet format LearnCSharp.slnx` hook.
- [ ] **Step 2:** `.gitattributes` → add `*.sln/*.vcxproj/*.filters` crlf; C++ `linguist-language` for h/hh/hpp/hxx/inc/cc/cpp/cxx/ipp/tpp; `*.obj`/`*.o` binary.
- [ ] **Step 3:** `.codespell-ignore` → append LearnCpp words (crate, abd, inout, nd, ot, te, fo, fpr, helo, unexpect; statics already present).
- [ ] **Step 4:** `.gitignore` → add CMake/cache patterns (CMakeCache.txt, CMakeFiles/, CMakeUserPresets.json, cmake-build-*/, Testing/, .cache/, .sccache/, .ccache/, *.gcov/gcda/gcno) not already covered.
- [ ] **Step 5:** `codeql.yml` → add `'src/learncpp/**'` to `paths-ignore` (no cpp language added).

---

### Task 8: Local verification (vcvars64)

- [ ] **Step 1:** In vcvars64 env: `cmake --preset windows-ci` then `cmake --build --preset windows-ci` in `src/learncpp` → compiles & links.
- [ ] **Step 2:** Run `build/windows-ci/bin/learn_cpp.exe --list` → header `learn_cpp: 874 topics registered`.
- [ ] **Step 3:** `msbuild src/learncpp/vs/cpp_learn.vcxproj -p:Configuration=Release -p:Platform=x64 -m` (clang-cl on PATH) → vcxproj builds.
- [ ] **Step 4:** `pre-commit run --all-files` (`SKIP=dotnet-format`); then `dotnet format LearnCSharp.slnx --verify-no-changes --no-restore` (fallback → `LearnCSharp.CI.slnx` if it errors on the vcxproj).

---

### Task 9: Commit and push

- [ ] **Step 1:** `git add -A`, review `git status`, commit with descriptive message on `merge-learncpp`.
- [ ] **Step 2:** `git push -u origin merge-learncpp`. Do NOT open a PR.
