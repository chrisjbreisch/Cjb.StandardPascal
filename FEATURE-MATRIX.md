# ISO 7185 Feature Matrix

Last reviewed: **2026-08-14**

This matrix is the authoritative implementation-status summary for
Cjb.StandardPascal. It must be updated in the same change as any feature,
behavior, or conformance change.

**Status:** ✅ Implemented · 🚧 Partial · ⬜ Not implemented

## Foundation

| Feature | Status | Notes |
| --- | :---: | --- |
| Cross-platform .NET solution | ✅ | .NET 10 solution with separate language, CLI, and test projects |
| Strict build configuration | ✅ | Compiler warnings are treated as errors |
| Application composition root | ✅ | Bootstrapper configures JSON/environment settings, logging, DI, and application scope |
| Execution contract | ✅ | `IScanner.ScanTokens(SourceText)` feeds `IParser.Parse`/`ParseStatement`; `IInterpreter.Evaluate`/`Interpret` execute the resulting expression or temporary statement; the application hosts the interactive loop and the CLI delegates process execution to it |
| Command-line application project | 🚧 | Interactive expression scanning is available; parsing and execution remain |
| Interactive expression input | ✅ | Reads expressions until a blank line or end-of-input and prints interpreted values or positioned errors |
| Source-file arguments | ⬜ | Planned final interface: `cjb-pascal file.pas [file2.pas ...]` |
| Exit-code contract | 🚧 | Interactive scanner sessions return success; source-file and runtime codes remain undefined |
| Automated unit-test projects | ✅ | MSTest 4 projects exist for the language and CLI |
| Continuous integration | ⬜ | Build and test workflow not configured |

## Lexical analysis

| Feature | Status | Notes |
| --- | :---: | --- |
| Source text and source spans | ✅ | Tokens and scan errors carry file, line, column, offset, and length |
| Case-insensitive identifiers | ✅ | Keyword matching is case-insensitive; original spelling is preserved |
| Reserved words | 🚧 | Program, declaration, scalar-type, output, expression, and control-flow keywords are recognized; routine and composite grammar support remains |
| Integer literals | ✅ | Decimal digits are parsed as 64-bit integers pending the `maxint` policy |
| Real literals | ✅ | Decimal and scientific notation use invariant culture |
| Character and string literals | 🚧 | Single-quoted literals and doubled-quote escaping are implemented; ISO character/fixed-array typing remains |
| Operators and delimiters | 🚧 | Expression tokens plus program punctuation, assignment, and block delimiters are implemented |
| Delimiter aliases | ✅ | `(.`/`[`, `.)`/`]`, and `@`/`^` normalize to canonical tokens |
| Whitespace | ✅ | Whitespace is ignored while source locations remain accurate |
| Brace comments | ✅ | `{ comment }`, including source-correlated unterminated-comment diagnostics |
| Parenthesis-star comments | ✅ | `(* comment *)`, including source-correlated unterminated-comment diagnostics |
| Scanner diagnostics | ✅ | Unexpected characters, invalid numeric syntax, overflow, and unterminated comments/literals throw source-correlated `ScanException` errors |

## Expressions

| Feature | Status | Notes |
| --- | :---: | --- |
| Integer and real constants | ✅ | 64-bit integer and double-precision real values are scanned, parsed, and evaluated |
| Boolean constants | ✅ | Predefined identifiers `true` and `false` are evaluated case-insensitively |
| Character and string constants | 🚧 | String values scan, parse, compare, and print; character/fixed-array type semantics remain |
| Parenthesized expressions | ✅ | Grouped expressions preserve precedence and evaluate recursively |
| Unary `+`, `-`, and `not` | ✅ | Numeric signs and Boolean negation are evaluated with operand checks |
| `*`, `/`, `div`, and `mod` | ✅ | `/` returns real; `div` and `mod` require integers and detect zero divisors |
| `and`, `or` | ✅ | Boolean operands are required |
| Binary `+` and `-` | 🚧 | Numeric semantics are implemented; set operations remain |
| Relational operators | 🚧 | Numeric and Boolean comparisons are implemented; other ISO types remain |
| Set membership `in` | 🚧 | Syntax is parsed; set types and evaluation remain |
| Identifier expressions | 🚧 | `true` and `false` resolve; declared identifiers remain |
| Function calls | ⬜ | Depends on routine declarations |
| Set constructors | ⬜ | Includes individual elements and ranges |
| Static expression type checking | 🚧 | Program execution validates current expression operators and identifiers before interpretation; declared and structured types remain |
| Program semantic analysis | ✅ | Program execution performs source-correlated expression type and identifier validation before interpretation |
| Checked runtime arithmetic | ✅ | Integer overflow and division by zero produce source-positioned runtime errors |

## Program structure and declarations

| Feature | Status | Notes |
| --- | :---: | --- |
| Source-spanned AST abstractions | ✅ | Common AST bases cover programs, blocks, declarations, types, expressions, l-values, statements, and routines; concrete grammar support follows in later phases |
| Program AST and execution boundary | ✅ | `Program` retains a source-spanned statement body and is parsed/executed through `IParser.ParseProgram` and `IInterpreter.Execute`; expression and temporary-statement APIs remain available during migration |
| Program heading | ✅ | `program name(...);` with optional file parameters |
| Block structure | ✅ | Constant/variable declarations followed by `begin ... end` statements |
| Declaration ordering | 🚧 | Constant and variable sections are supported; label, type, and routine sections follow later |
| Label declarations | ⬜ | Numeric labels in the ISO-defined range |
| Constant declarations | ✅ | Scalar constant expressions |
| Type and variable declarations | 🚧 | Grouped scalar variables are supported; named types and aliases follow later |
| Procedure and function declarations | ⬜ | Nested routines and function-result assignment |
| Forward declarations | ⬜ | Routine signature matching required |
| Nested lexical scopes | ⬜ | Programs, procedures, and functions |
| Recursive routines | ⬜ | Requires activation records and lexical parents |

## Types

| Feature | Status | Notes |
| --- | :---: | --- |
| `integer`, `real`, `boolean`, and `char` | ✅ | 64-bit signed integer (`maxint` = `9223372036854775807`), double real, Boolean, and single-character string values |
| Enumerated and subrange types | 🚧 | Enumerations and integer subranges are supported, including assignment range checks; advanced ordinal compatibility remains |
| Array types | ⬜ | Any ordinal index type and multiple dimensions |
| Packed character arrays | ⬜ | Standard Pascal string representation |
| Record types | 🚧 | Fixed scalar fields support record variables and `with`; variant fields and nested selectors remain |
| Set types | ⬜ | Ordinal base type |
| File types | ⬜ | Files cannot contain file components |
| Pointer types | ⬜ | Includes forward type references and `nil` |
| Packed types | ⬜ | Initially semantic metadata; physical packing is optional |
| Type compatibility | ⬜ | Identity, ordinal/subrange, set, and string rules |
| Assignment compatibility | 🚧 | Scalar identity and integer-to-real promotion are enforced; structured and range rules remain |

## Statements

| Feature | Status | Notes |
| --- | :---: | --- |
| Empty and compound statements | ✅ | Semicolon-separated statements in `begin ... end`, including empty statements |
| Temporary `Print` statement | ✅ | Retained as a documented transitional extension for interactive migration |
| Assignment | ✅ | Scalar identifier assignment with compatibility checks |
| Procedure call | ⬜ | Value, variable, and routine parameters |
| `if` statement | ✅ | Includes nearest-`if` binding for `else` |
| `while` and `repeat` statements | ✅ | Boolean conditions are semantically validated |
| `for` statement | ✅ | Integer `to` and `downto`; active control assignment is rejected |
| `case` statement | ✅ | Scalar ordinal selectors, multi-label branches, `else`, and compatibility checks |
| `goto` and labeled statements | ✅ | Numeric labels with same-compound-block target restriction |
| `with` statement | ✅ | Fixed record-field scope |

## Predefined routines and runtime services

| Feature | Status | Notes |
| --- | :---: | --- |
| Numeric functions | ⬜ | `abs`, `sqr`, `sqrt`, trigonometric, exponential, and logarithmic functions |
| Ordinal functions | ✅ | `ord`, `chr`, `succ`, and `pred` for integer and character ordinals |
| Real conversion functions | ✅ | `round` and `trunc` |
| Text output | 🚧 | `write` and `writeln` output scalar expressions; field widths remain |
| Text input | ⬜ | `read` and `readln` |
| File operations | ⬜ | Standard file and text-file operations |
| Packing procedures | ⬜ | `pack` and `unpack` |
| Dynamic allocation | ⬜ | `new` and `dispose` |
| Runtime diagnostics | 🚧 | Arithmetic, undefined identifier, and scalar assignment errors are source-correlated; advanced semantic diagnostics follow |
| Typed symbols and routine signatures | ✅ | Case-insensitive declaration/identifier resolution, duplicate diagnostics, variables, constants, parameters, and routine signatures are available for semantic analysis |
| Runtime values and activation records | ✅ | Typed variable bindings support lexical-parent lookup, shadowing, compatible assignment, and source-correlated runtime errors |

## Quality and conformance

| Feature | Status | Notes |
| --- | :---: | --- |
| Scanner, parser, semantic, and interpreter tests | 🚧 | Scanner, parser, and interpreter tests cover the implemented expression subset |
| CLI integration tests | 🚧 | Interpreter output, scanner/parser/runtime error paths, recovery, and end-of-input are covered |
| ISO example corpus | ⬜ | Positive and negative conformance programs required |
| Cross-platform validation | ⬜ | Windows, Linux, and macOS CI required |
| Feature-status documentation | ✅ | This matrix establishes the baseline |
| Project overview and build instructions | ✅ | Maintained in `README.md` |
