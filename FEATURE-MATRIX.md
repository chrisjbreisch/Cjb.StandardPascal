# ISO 7185 Feature Matrix

Last reviewed: **2026-08-21**

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
| Character and string literals | ✅ | Single-quoted literals, doubled-quote escaping, and exact-length packed character-array assignment are implemented |
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
| Character and string constants | ✅ | Strings scan, parse, compare, print, and assign to exact-length fixed character arrays |
| Parenthesized expressions | ✅ | Grouped expressions preserve precedence and evaluate recursively |
| Unary `+`, `-`, and `not` | ✅ | Numeric signs and Boolean negation are evaluated with operand checks |
| `*`, `/`, `div`, and `mod` | ✅ | `/` returns real; `div` and `mod` require integers and detect zero divisors |
| `and`, `or` | ✅ | Boolean operands are required |
| Binary `+` and `-` | 🚧 | Numeric semantics plus integer set union and difference are implemented |
| Relational operators | 🚧 | Numeric and Boolean comparisons are implemented; other ISO types remain |
| Set membership `in` | ✅ | Integer set membership is evaluated with ordinal operand checks |
| Identifier expressions | 🚧 | `true` and `false` resolve; declared identifiers remain |
| Function calls | ✅ | User functions and predefined ordinal/numeric routines are evaluated with arity/type checks |
| Set constructors | ✅ | Integer elements and inclusive integer ranges are supported |
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
| Declaration ordering | ✅ | Label, constant, type, variable, procedure, and function sections are parsed in ISO order |
| Label declarations | ✅ | Numeric labels are supported with same-block `goto` restrictions |
| Constant declarations | ✅ | Scalar constant expressions |
| Type and variable declarations | 🚧 | Grouped variables, named ordinal/composite types, arrays, files, and pointers are supported; alias and full ISO identity rules remain |
| Procedure and function declarations | ✅ | Procedures and functions support typed signatures and function-result assignment |
| Forward declarations | ✅ | Forward procedure declarations can precede their executable definition |
| Nested lexical scopes | ✅ | Nested procedures resolve enclosing routine locals with shadowing |
| Recursive routines | ✅ | Recursive function calls isolate and restore caller activations |

## Types

| Feature | Status | Notes |
| --- | :---: | --- |
| `integer`, `real`, `boolean`, and `char` | ✅ | 64-bit signed integer (`maxint` = `9223372036854775807`), double real, Boolean, and single-character string values |
| Enumerated and subrange types | 🚧 | Enumerations and integer subranges are supported, including assignment range checks; advanced ordinal compatibility remains |
| Array types | 🚧 | Integer-bounded multidimensional arrays support indexed reads/writes and bounds diagnostics |
| Packed character arrays | ✅ | Indexed packed character arrays accept exact-length strings |
| Record types | ✅ | Fixed and scalar variant fields support direct selection and `with` |
| Set types | ✅ | Runtime integer sets support constructors, ranges, membership, union, difference, and intersection |
| File types | ✅ | Injectable in-memory typed file queues support `write(file, value)` and `read(file, variable)` |
| Pointer types | ✅ | Named pointer types, `nil`, allocation, disposal, dereference, and lifetime diagnostics are supported |
| Packed types | ✅ | `packed array` syntax and fixed character-array representation are supported |
| Type compatibility | ⬜ | Identity, ordinal/subrange, set, and string rules |
| Assignment compatibility | ✅ | Scalar identity, integer-to-real promotion, pointer `nil`, subrange checks, fixed strings, and structured reference aliasing are enforced |

## Statements

| Feature | Status | Notes |
| --- | :---: | --- |
| Empty and compound statements | ✅ | Semicolon-separated statements in `begin ... end`, including empty statements |
| Temporary `Print` statement | ✅ | Retained as a documented transitional extension for interactive migration |
| Assignment | ✅ | Scalar, indexed, field, dereference, fixed-string, and structured reference assignment are supported |
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
| Numeric functions | ✅ | `abs`, `sqr`, `sqrt`, `sin`, `cos`, `tan`, `arctan`, `exp`, and `ln` with source-correlated domain errors |
| Ordinal functions | ✅ | `ord`, `chr`, `succ`, and `pred` for integer and character ordinals |
| Real conversion functions | ✅ | `round` and `trunc` |
| Text output | 🚧 | `write` and `writeln` output scalar expressions; field widths remain |
| Text input | ✅ | Injectable `read` and `readln` assign scalar console input |
| File operations | ✅ | In-memory file queues implement one-item `write`/`read` with empty-file diagnostics |
| Packing procedures | ✅ | `pack` and `unpack` copy bounded array elements |
| Dynamic allocation | ✅ | `new`, `dispose`, dereference, `nil`, and disposed-pointer lifetime errors are supported |
| Runtime diagnostics | ✅ | Arithmetic, bounds, undefined identifier, pointer lifetime, file-empty, and assignment errors are source-correlated |
| Typed symbols and routine signatures | ✅ | Case-insensitive declaration/identifier resolution, duplicate diagnostics, variables, constants, parameters, and routine signatures are available for semantic analysis |
| Runtime values and activation records | ✅ | Typed variable bindings support lexical-parent lookup, shadowing, compatible assignment, and source-correlated runtime errors |

## Quality and conformance

| Feature | Status | Notes |
| --- | :---: | --- |
| Scanner, parser, semantic, and interpreter tests | 🚧 | Focused coverage includes expressions, structured control flow, routines, arrays, sets, files, pointers, and diagnostics |
| CLI integration tests | 🚧 | Interpreter output, scanner/parser/runtime error paths, recovery, and end-of-input are covered |
| ISO example corpus | 🚧 | Phase 4 positive and negative composite fixtures are present; broader ISO corpus remains |
| Cross-platform validation | ⬜ | Windows, Linux, and macOS CI required |
| Feature-status documentation | ✅ | This matrix establishes the baseline |
| Project overview and build instructions | ✅ | Maintained in `README.md` |
