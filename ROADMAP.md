# Standard Pascal Interpreter Roadmap

This roadmap describes the staged completion of the ISO 7185 Standard Pascal interpreter. The current implementation supports scanning, parsing, and evaluating expressions plus the temporary `Print` statement. Each phase should preserve existing regression coverage and update `FEATURE-MATRIX.md` in the same change as the implementation.

## Phase 0: Architecture Foundation

- [ ] Confirm the execution contract across `IScanner`, `IParser`, `IInterpreter`, the application layer, and the CLI.
- [ ] Replace the expression/temporary-statement-only boundary with a program AST and execution boundary while retaining expression APIs during migration.
- [ ] Add source-spanned AST abstractions for programs, blocks, declarations, types, expressions, l-values, statements, and routines.
- [ ] Introduce a typed symbol and type model with duplicate-declaration checks, identifier resolution, assignment compatibility, and routine signatures.
- [ ] Introduce scoped environments, lexical-parent lookup, runtime values, and activation records.
- [ ] Add semantic analysis between parsing and interpretation with source-correlated diagnostics.

**Primary areas:** `Parser/`, `Interpreter/`, AST visitor contracts, and language tests.

## Phase 1: Program Shell and Scalar Execution

- [ ] Add Pascal reserved words and punctuation needed for program headings, declarations, blocks, assignments, and control flow.
- [ ] Add brace comments, parenthesis-star comments, supported delimiter aliases, and diagnostics for unterminated comments/literals and invalid tokens.
- [ ] Parse `program name(...);`, declaration sections in ISO order, and `begin ... end` blocks.
- [ ] Implement `integer`, `real`, `boolean`, and `char` types, constants, variables, scalar literals, and assignments.
- [ ] Implement empty and compound statements and statement sequencing.
- [ ] Define integer width/`maxint`, integer-to-real promotion, character/string representation, range checks, and type errors.
- [ ] Add `write` and `writeln`; decide whether the temporary `Print` extension remains supported.
- [ ] Add source-file execution, input/output abstractions, and deterministic CLI exit codes.
- [ ] Add a minimal complete-program test covering declarations, assignment, expressions, output, malformed input, and runtime errors.

**Primary areas:** `Scanner/`, `Parser/`, `Interpreter/`, `ConsoleApp.cs`, `Program.cs`, and focused language/CLI tests.

## Phase 2: Structured Statements and Ordinal Semantics

- [ ] Implement `if`, `while`, `repeat ... until`, and `for ... to/downto`.
- [ ] Implement `case` with ordinal selectors and compatible labels.
- [ ] Implement labeled statements and `goto` where permitted by the selected ISO restrictions.
- [ ] Implement `with` after record values and field lookup exist.
- [ ] Add enumerated and subrange types with ordinal operations and range checks.
- [ ] Add `ord`, `chr`, `succ`, `pred`, `round`, and `trunc`.
- [ ] Enforce Boolean conditions, loop-control rules, selector compatibility, and structured-statement restrictions during semantic analysis.
- [ ] Add control-flow integration programs and source-correlated diagnostics.

## Phase 3: Procedures, Functions, and Lexical Scopes

- [ ] Implement procedure and function declarations and signatures.
- [ ] Implement value parameters, `var` parameters, function-result assignment, calls, and arity/type validation.
- [ ] Implement forward declarations, nested lexical scopes, activation records, lexical parents, and recursion.
- [ ] Define routine compatibility and overload behavior explicitly; do not substitute approximate handlers for unsupported routines.
- [ ] Add predefined numeric routines: `abs`, `sqr`, `sqrt`, trigonometric, exponential, and logarithmic functions.
- [ ] Define numeric conversion, domain-error, and output policies.
- [ ] Test calls, parameter passing, shadowing, recursion, forward references, and call diagnostics.

## Phase 4: Composite, Set, String, File, and Pointer Types

- [ ] Implement arrays, multidimensional arrays, packed character arrays, records, and variant fields.
- [ ] Implement sets, set constructors, ranges, membership, and set operations.
- [ ] Implement file and text-file types with injectable runtime services.
- [ ] Implement pointer types, `nil`, `new`, `dispose`, `pack`, and `unpack`.
- [ ] Complete ISO fixed-array character/string semantics and compatibility rules.
- [ ] Implement record-field access, array indexing, pointer dereference, and file operations including `read` and `readln`.
- [ ] Define structured runtime values, copy/reference semantics, aliasing, bounds checks, set limits, file errors, and pointer lifetime errors.
- [ ] Add positive and negative ISO program fixtures.

## Phase 5: Conformance and Delivery

- [ ] Add an ISO example corpus with expected output and error metadata.
- [ ] Add a conformance harness that executes programs through the same application/CLI path.
- [ ] Ensure cross-platform-safe paths, newlines, input, and output behavior.
- [ ] Add CI for restore, warnings-as-errors builds, language tests, CLI tests, and conformance tests.
- [ ] Keep generated `bin/` and `obj/` contents out of source changes.
- [ ] Update `README.md` claims and examples as capabilities become available.
- [ ] Mark completed work in this roadmap and `FEATURE-MATRIX.md` together.

## Implementation Order

The program shell and declarations precede general statements because identifier resolution and storage are prerequisites for assignment and control flow. Blocks and environments precede procedures because nested routines require lexical scope and activation-record behavior. Composite types follow scalar and routine foundations because their compatibility, aliasing, and runtime-value rules depend on the semantic model.

Each implementation slice should begin with a focused failing MSTest, followed by the smallest implementation that makes it pass. Existing expression tests remain regression coverage throughout the migration.

## Architecture Decisions to Resolve Early

- Integer width and the ISO `maxint` policy.
- Representation and compatibility rules for `char` and fixed-array strings.
- Type identity, subrange compatibility, integer-to-real assignment, and range checking.
- Runtime representation and copy/reference behavior for arrays, records, sets, files, and pointers.
- Input/output service abstractions and deterministic CLI exit codes.
- Whether `Print` remains as a documented transitional extension after `write`/`writeln` are implemented.
- The exact ISO restrictions selected for labels, `goto`, forward declarations, files, and pointers.

## Relevant Files

- `src/Cjb.StandardPascal.Language/Scanner/TokenType.cs` and `Scanner.cs`: tokens, keywords, comments, literals, and source spans.
- `src/Cjb.StandardPascal.Language/Parser/Parser.cs`: recursive-descent program, declaration, block, expression, and statement parsing.
- `src/Cjb.StandardPascal.Language/Parser/Expressions/` and `Parser/Statements/`: AST nodes and visitor contracts.
- `src/Cjb.StandardPascal.Language/Interpreter/`: semantic analysis, symbols, types, scopes, runtime values, execution, and diagnostics.
- `src/Cjb.StandardPascal.Application/ConsoleApp.cs`, `IConsoleApp.cs`, and `Bootstrapper.cs`: application execution and I/O composition.
- `src/Cjb.StandardPascal.Cli/Program.cs`: process arguments and exit codes.
- `tests/Cjb.StandardPascal.Language.Tests/`: scanner, parser, semantic, interpreter, and conformance tests.
- `tests/Cjb.StandardPascal.Cli.Tests/`: source-file, output, diagnostics, recovery, and exit-code tests.
- `FEATURE-MATRIX.md` and `README.md`: synchronized feature status and user-facing documentation.

## Validation Gates

- Language-only changes: `dotnet test tests/Cjb.StandardPascal.Language.Tests/Cjb.StandardPascal.Language.Tests.csproj`.
- Application/CLI changes: `dotnet test tests/Cjb.StandardPascal.Cli.Tests/Cjb.StandardPascal.Cli.Tests.csproj`.
- Cross-project changes and completed phases: `dotnet build Cjb.StandardPascal.sln` followed by `dotnet test Cjb.StandardPascal.sln`.
- Once source-file execution exists, run a minimal `.pas` file through `dotnet run --project src/Cjb.StandardPascal.Cli -- <file>` and verify output, diagnostics, and exit codes.
- Before declaring a feature family complete, run its positive and negative conformance fixtures and verify that scanner, parser, semantic, and runtime errors preserve file, line, column, offset, and length.
