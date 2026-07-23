# Merge LearnCpp into LearnCSharp — Design

- **Date:** 2026-07-23
- **Branch / worktree:** `merge-learncpp` at `LearnCSharp/.worktree/merge-learncpp`
- **Status:** Approved by user ("没问题 执行")

## Goal

Fold the entire `LearnCpp` repository (C++23 self-registering topic examples, CMake + VS build, per-platform CI, tooling configs) into `LearnCSharp`, living under `src/learncpp/`, so the two learning tracks share one repo while each keeps its own build system.

## Non-goals

- No CodeQL analysis for C++ (too slow). CodeQL keeps analyzing only `csharp`.
- No Windows VS/MSBuild slnx CI job for C++ (dropped). C++ CI builds with CMake on every platform.
- `LearnCSharp.CI.slnx` is untouched (dotnet-only CI solution).
- We do **not** build the `.slnx` ourselves; the user opens it locally, builds, then opens the PR.

## Decisions

### Workspace
Git worktree fallback (`git worktree add .worktree/merge-learncpp -b merge-learncpp`), matching the repo's existing `.worktree/<branch>` convention (gitignored). Native `EnterWorktree` is not usable because the session cwd is the separate `LearnCpp` repo.

### File layout — nest the whole LearnCpp tree under `src/learncpp/`
Copy everything from LearnCpp except `.git`, `build/`, `.vs/`, `.worktree/`:

| LearnCpp | → LearnCSharp |
|---|---|
| `CMakeLists.txt`, `CMakePresets.json`, `cmake/*.cmake` | `src/learncpp/…` (unchanged; globs `${CMAKE_SOURCE_DIR}/src/*.cpp` + `include/*.hpp`, `binaryDir=${sourceDir}/build/…` still resolve) |
| `include/`, `src/` (876 `.cpp`, 874 topics) | `src/learncpp/include/`, `src/learncpp/src/` |
| `vs/learn_cpp.vcxproj{,.filters,.user}` | `src/learncpp/vs/cpp_learn.vcxproj{,.filters,.user}` (renamed) |
| `scripts/`, `docs/`, `README.md` | `src/learncpp/scripts|docs|README.md` (`%~dp0..` / `$PSScriptRoot/..` still resolve to `src/learncpp`) |
| `.clang-format`, `.clang-format-ignore`, `.clang-tidy`, `.clangd`, `.editorconfig` | `src/learncpp/…` (tools walk up from cpp files; editorconfig `root=true` stripped) |
| `.gitattributes`, `.pre-commit-config.yaml`, `.codespell-ignore` | merged into repo-root files |
| `LICENSE` | skipped (byte-identical) |

Repo-root `build/` gitignore already covers `src/learncpp/build/` and `build/vs/`.

### vcxproj → `cpp_learn`
Rename the three `vs/` files; `<RootNamespace>cpp_learn</RootNamespace>`; keep ProjectGuid. Update `scripts/generate-vs-filters.ps1` targets and re-run it to regenerate items+filters (`..\src\…` paths unchanged → stable output). Keep the CMake target / exe name `learn_cpp` (CI smoke asserts and `main.cpp` help reference it).

### `LearnCSharp.slnx`
Add `<Folder Name="/src/learncpp/">` with `<Project Path="src/learncpp/vs/cpp_learn.vcxproj" Id="44b4c0d0-…" Type="Native">` plus config/platform mapping so the x64-only vcxproj builds as `Debug|x64`/`Release|x64` and maps cleanly under the `Any CPU` solution config. Exact VS2026 slnx schema verified during implementation; user's local build is the final check.

### CI — add a cpp CMake job to each OS workflow
- `windows-ci.yml`: `cpp-cmake` job on `windows-2025-vs2026` (msvc-dev-cmd x64 VS18 → get-cmake → VS2026 bundled clang-cl on PATH → sccache+cache → `cmake --preset windows-ci` build in `src/learncpp` → smoke-run asserting 874 topics). **No slnx-msbuild job.**
- `linux-ci.yml`: `cpp-cmake` job (apt.llvm.org clang 22, libstdc++/libc++ matrix, `--preset linux-ci`, 874-topic smoke).
- `macos-ci.yml`: `cpp-cmake` job (Homebrew `llvm@22`, `--preset macos-ci`, 874-topic smoke).
- Adaptations: `working-directory: src/learncpp`; sccache `hashFiles('src/learncpp/CMakeLists.txt','src/learncpp/cmake/**','src/learncpp/CMakePresets.json')`; cpp jobs run pre-commit with `SKIP: dotnet-format` (no dotnet on cpp runners). Existing dotnet jobs unchanged.

### Root config merges
- `.pre-commit-config.yaml`: union of both — generic hooks (add `check-toml`, `--allow-multiple-documents`, `vcxproj|filters` in `mixed-line-ending` exclude), codespell (v2.4.3), clang-format via `mirrors-clang-format` v22.1.4 (self-contained, c/cpp extensions), existing local `dotnet format LearnCSharp.slnx` hook unchanged. clang-tidy stays out.
- `.gitattributes`: add `*.sln/*.vcxproj/*.filters → crlf`, C++ `linguist-language` for h/hpp/cpp/etc., `*.obj/*.o` binary.
- `.codespell-ignore`: union of word lists.
- `codeql.yml`: no cpp language added → cpp never analyzed; add `src/learncpp/**` to `paths-ignore` to make it explicit.

### Local verification (vcvars64 env)
1. `cmake --preset windows-ci` + build in `src/learncpp` → compiles & links.
2. `learn_cpp.exe --list` → `learn_cpp: 874 topics registered`.
3. `msbuild src/learncpp/vs/cpp_learn.vcxproj -p:Configuration=Release -p:Platform=x64 -m` → vcxproj compiles directly (what the user's slnx build invokes).
4. `pre-commit run --all-files` (`SKIP=dotnet-format`) + `dotnet format LearnCSharp.slnx --verify-no-changes` (confirm format hook tolerates the vcxproj).

### Commit & push
One commit on `merge-learncpp`; `git push -u origin merge-learncpp`. No PR (user opens it after local slnx build).

## Risks & mitigations
- **`dotnet format` + vcxproj in slnx** may error → test locally; fallback: point hook at `LearnCSharp.CI.slnx`.
- **slnx config mapping** for x64-only vcxproj under `Any CPU` → research VS2026 schema; user's local build validates.
- **Runner label `windows-2025-vs2026`** carried from LearnCpp's working CI; adjust if unavailable.
