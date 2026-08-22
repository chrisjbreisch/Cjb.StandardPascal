# Cjb.StandardPascal Development Instructions

## Project Context

- This repository is a C#/.NET implementation of a Standard Pascal scanner, parser, interpreter, CLI, and application bootstrapper.
- Keep responsibilities separated across the existing projects:
  - `Cjb.StandardPascal.Language`: scanning, parsing, expressions/statements, interpretation, and language exceptions.
  - `Cjb.StandardPascal.Application`: application composition and console-facing abstractions.
  - `Cjb.StandardPascal.Cli`: process entry point and CLI wiring.
  - `tests`: MSTest coverage for the language and CLI/application layers.

## Implementation Guidelines

- Follow the existing C# style, public APIs, naming, and project boundaries before introducing new abstractions.
- Prefer small, focused changes that preserve behavior outside the requested feature.
- Keep parsing and interpretation behavior explicit and deterministic; use the existing token, source span, parse exception, and runtime exception types for diagnostics.
- Preserve source locations when creating or propagating syntax and runtime errors.
- Avoid changing generated files, `bin/`, or `obj/` contents.
- Do not add dependencies unless the existing .NET libraries and project patterns cannot reasonably support the change.
- After each change, ensure that the solution builds successfully and that all relevant tests pass.
- After each change, commit with a meaningful message that describes the intent and scope of the change and push it to the remote repository.
- Do not end a turn with uncommitted or failing work unless specifcally asked to stop.
- If a task cannot be completed, ask one concrete blocking question instead of reporting partial completion.
- Continue automatically until the user’s requested roadmap phase is fully complete. Never stop merely because work is broad or crosses roadmap phases.
- Before ending, all changes must be validated, committed, pushed, and `git status --short` must be empty. Otherwise continue working or ask a single specific blocking question.


## Testing and Validation

- Add or update focused MSTest cases for behavior changes, especially scanner, parser, and interpreter edge cases.
- Run the narrowest relevant test project first, then run the full solution test suite when the change crosses project boundaries.
- Before finishing, run `dotnet build Cjb.StandardPascal.sln` and relevant tests, unless the user requests otherwise.
- Treat compiler warnings and test failures introduced by the change as issues to resolve; do not alter unrelated failing tests.


## Documentation

- Update `README.md` or `FEATURE-MATRIX.md` when a user-visible language feature, CLI behavior, or supported capability changes.
- Keep `ROADMAP.md` current as implementation progresses; update its phase checklists, dependencies, and architecture decisions when the plan changes.
- Keep documentation concise and consistent with the current implementation.
- Maintain a live checklist of every requested roadmap item. Mark an item complete only after its implementation, tests, documentation, commit, and push are complete.