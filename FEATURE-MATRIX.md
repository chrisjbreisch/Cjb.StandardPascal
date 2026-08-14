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
| Command-line application project | 🚧 | Entry point exists but intentionally reports that execution is unavailable |
| Source-file arguments | ⬜ | Planned final interface: `cjb-pascal file.pas [file2.pas ...]` |
| Exit-code contract | ⬜ | Success, source error, runtime error, and invocation error codes remain to be defined |
| Automated unit-test projects | ✅ | MSTest 4 projects exist for the language and CLI |
| Continuous integration | ⬜ | Build and test workflow not configured |

## Lexical analysis

| Feature | Status | Notes |
| --- | :---: | --- |
| Source text and source spans | ⬜ | File, line, column, offset, and length are required |
| Case-insensitive identifiers | ⬜ | Original spelling should remain available for diagnostics |
| Reserved words | ⬜ | Full ISO 7185 reserved-word set required |
| Integer literals | ⬜ | Decimal digits without a sign |
| Real literals | ⬜ | Decimal and scientific notation |
| Character and string literals | ⬜ | Single quotes with doubled-quote escaping |
| Operators and delimiters | ⬜ | Includes `:=`, `<>`, `<=`, `>=`, and `..` |
| Delimiter aliases | ⬜ | `(.`/`[`, `.)`/`]`, `@`/`^`, and `(*`/`{` forms |
| Whitespace | ⬜ | Spaces and line endings act as separators |
| Brace comments | ⬜ | `{ comment }` |
| Parenthesis-star comments | ⬜ | `(* comment *)` |
| Scanner diagnostics | ⬜ | Unexpected characters and unterminated constructs |

## Expressions

| Feature | Status | Notes |
| --- | :---: | --- |
| Integer and real constants | ⬜ | First expression milestone |
| Boolean constants | ⬜ | Predefined identifiers `true` and `false` |
| Character and string constants | ⬜ | Fixed-length character arrays require later type support |
| Parenthesized expressions | ⬜ | Highest-precedence grouping |
| Unary `+`, `-`, and `not` | ⬜ | Numeric signs and Boolean negation |
| `*`, `/`, `div`, and `mod` | ⬜ | `/` returns real; `div` and `mod` require integers |
| `and`, `or` | ⬜ | Boolean operands only |
| Binary `+` and `-` | ⬜ | Numeric operations first; set operations later |
| Relational operators | ⬜ | `=`, `<>`, `<`, `>`, `<=`, and `>=` |
| Set membership `in` | ⬜ | Depends on set types |
| Identifier expressions | ⬜ | Depends on declarations and name resolution |
| Function calls | ⬜ | Depends on routine declarations |
| Set constructors | ⬜ | Includes individual elements and ranges |
| Static expression type checking | ⬜ | Required before general program execution |
| Checked runtime arithmetic | ⬜ | Overflow and division-by-zero diagnostics |

## Program structure and declarations

| Feature | Status | Notes |
| --- | :---: | --- |
| Program heading | ⬜ | `program name(...);` |
| Block structure | ⬜ | Declaration part followed by a statement part |
| Declaration ordering | ⬜ | Label, constant, type, variable, and routine sections |
| Label declarations | ⬜ | Numeric labels in the ISO-defined range |
| Constant declarations | ⬜ | Integer, real, character, and named constants |
| Type and variable declarations | ⬜ | Named types, aliases, and grouped variables |
| Procedure and function declarations | ⬜ | Nested routines and function-result assignment |
| Forward declarations | ⬜ | Routine signature matching required |
| Nested lexical scopes | ⬜ | Programs, procedures, and functions |
| Recursive routines | ⬜ | Requires activation records and lexical parents |

## Types

| Feature | Status | Notes |
| --- | :---: | --- |
| `integer`, `real`, `boolean`, and `char` | ⬜ | Predefined scalar types |
| Enumerated and subrange types | ⬜ | User-defined ordinals and range checks |
| Array types | ⬜ | Any ordinal index type and multiple dimensions |
| Packed character arrays | ⬜ | Standard Pascal string representation |
| Record types | ⬜ | Fixed and variant fields |
| Set types | ⬜ | Ordinal base type |
| File types | ⬜ | Files cannot contain file components |
| Pointer types | ⬜ | Includes forward type references and `nil` |
| Packed types | ⬜ | Initially semantic metadata; physical packing is optional |
| Type compatibility | ⬜ | Identity, ordinal/subrange, set, and string rules |
| Assignment compatibility | ⬜ | Includes integer-to-real promotion and range checks |

## Statements

| Feature | Status | Notes |
| --- | :---: | --- |
| Empty and compound statements | ⬜ | Statement sequences and `begin ... end` |
| Assignment | ⬜ | Variable access on the left of `:=` |
| Procedure call | ⬜ | Value, variable, and routine parameters |
| `if` statement | ⬜ | Includes nearest-`if` binding for `else` |
| `while` and `repeat` statements | ⬜ | Boolean loop conditions |
| `for` statement | ⬜ | `to` and `downto`; ordinal local control variable |
| `case` statement | ⬜ | Ordinal selector and compatible labels |
| `goto` and labeled statements | ⬜ | Structured-statement jump restrictions |
| `with` statement | ⬜ | Record-field scope |

## Predefined routines and runtime services

| Feature | Status | Notes |
| --- | :---: | --- |
| Numeric functions | ⬜ | `abs`, `sqr`, `sqrt`, trigonometric, exponential, and logarithmic functions |
| Ordinal functions | ⬜ | `ord`, `chr`, `succ`, and `pred` |
| Real conversion functions | ⬜ | `round` and `trunc` |
| Text output | ⬜ | `write` and `writeln`, including field widths |
| Text input | ⬜ | `read` and `readln` |
| File operations | ⬜ | Standard file and text-file operations |
| Packing procedures | ⬜ | `pack` and `unpack` |
| Dynamic allocation | ⬜ | `new` and `dispose` |
| Runtime diagnostics | ⬜ | Source-correlated errors without silent fallbacks |

## Quality and conformance

| Feature | Status | Notes |
| --- | :---: | --- |
| Scanner, parser, semantic, and interpreter tests | ⬜ | Components do not exist yet |
| CLI integration tests | ⬜ | Test project exists; behavior tests remain |
| ISO example corpus | ⬜ | Positive and negative conformance programs required |
| Cross-platform validation | ⬜ | Windows, Linux, and macOS CI required |
| Feature-status documentation | ✅ | This matrix establishes the baseline |
| Project overview and build instructions | ✅ | Maintained in `README.md` |
