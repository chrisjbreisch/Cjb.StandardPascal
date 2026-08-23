# ISO 7185 Compatibility Ledger

Last reviewed: **2026-08-22**

This document is the clause-level compatibility ledger for the ISO 7185:1990
standard used by Cjb.StandardPascal. It is intentionally more precise than the
feature matrix: a check means the named clause and its listed requirements are
implemented for the supported language surface, while a partial mark means
that some requirements are implemented but at least one requirement remains.

This is an implementation ledger, not a claim of ISO compliance. The processor
must not claim ISO 7185 conformance until all required clauses and their
implementation-defined policies have been verified by the Phase 5 conformance
harness.

**Status:** ✅ Implemented · 🚧 Partial · ⬜ Not implemented

## How to Read This

- Parent clauses are marked ✅ only when every listed child clause is ✅.
- A child clause can be ✅ even when its parent is 🚧 because sibling clauses remain incomplete.
- Notes identify the exact boundary of the current implementation; “partial” does not mean that the entire clause is absent.
- Informative annexes are tracked separately from the normative requirements in clause 6.

## Sections 1-5

| ISO clause | Status | Current implementation |
| --- | :---: | --- |
| 1 Scope | ✅ | The project targets a processor for ISO 7185 programs and documents implementation limits. |
| 1.1 | ✅ | Language processor scope is established. |
| 1.2 | ✅ | Activation, transformation, error-reporting, and host-environment choices are treated as implementation concerns. |
| 2 Normative reference | ⬜ | ISO 646 dependency is not formally modeled or verified. |
| 3 Definitions | 🚧 | Core processor, error, extension, and implementation terminology is used; no clause-specific conformance audit exists. |
| 3.1 Error | ✅ | Scanner, parser, semantic, and runtime error paths are distinguished. |
| 3.2 Extension | 🚧 | The temporary `Print` extension exists and is documented; extension reporting is not yet a formal processor facility. |
| 3.3 Implementation-defined | 🚧 | Selected policies such as `maxint`, character range, output spacing, and numeric formatting are implemented; the complete required policy document is not yet complete. |
| 3.4 Implementation-dependent | 🚧 | Host bindings and evaluation choices exist, but the complete policy inventory is not yet documented. |
| 3.5 Processor | ✅ | The CLI, application composition, language pipeline, and runtime together form an executable processor. |
| 4 Definitional conventions | 🚧 | The implementation follows the standard's syntax/semantics distinction; no automated notation audit is applicable. |
| 5 Compliance | 🚧 | The project explicitly does not claim ISO compliance; a complete exception statement and level selection remain Phase 5 work. |
| 5.1 Processors | 🚧 | Programs can be prepared and executed with diagnostics, but full required-feature acceptance, error treatment, and implementation-policy documentation are incomplete. |
| 5.2 Programs | 🚧 | A substantial subset of level 0 syntax executes; unsupported ISO constructs and conformance boundaries remain. |

## Section 6.1 Lexical Tokens

| ISO clause | Status | Current implementation |
| --- | :---: | --- |
| 6.1 Lexical tokens | 🚧 | Scanner coverage is broad for the implemented subset but does not cover every ISO token and separator rule. |
| 6.1.1 General | ✅ | ASCII letters/digits and token scanning are implemented. |
| 6.1.2 Special-symbols | ✅ | The scanner recognizes the ISO non-word symbols (`+ - * / = <> < <= >= > ( ) [ ] (. .) : := , ; . ..`) and ISO word-symbols (`and array begin case const div do downto else end file for function goto if in label mod nil not of or packed procedure program record repeat set then to type until var while with`). |
| 6.1.3 Identifiers | 🚧 | Case-insensitive letter/digit identifiers are supported; full ISO identifier-region validation is not independently audited. |
| 6.1.4 Directives | ✅ | `forward` is recognized and used by procedure declarations. |
| 6.1.5 Numbers | 🚧 | Decimal integer, real, and scientific literals with overflow diagnostics are implemented; all signed-number and implementation-defined numeric policies are not complete. |
| 6.1.6 Labels | 🚧 | Numeric labels are scanned and parsed; the complete ISO `0..9999` validation policy is not enforced. |
| 6.1.7 Character-strings | 🚧 | Single-quoted strings and doubled apostrophes are supported; the full ISO fixed string-type model remains narrower than the standard. |
| 6.1.8 Token separators | 🚧 | Whitespace, multiline `{...}` and `(*...*)` comments work; ISO cross-delimiter comment termination and every separator edge case are not complete. |
| 6.1.8.1 Brace comments | ✅ | Brace comments may span lines and produce source-correlated unterminated-comment errors. |
| 6.1.8.2 Parenthesis-star comments | ✅ | Parenthesis-star comments may span lines and produce source-correlated unterminated-comment errors. |
| 6.1.9 Lexical alternatives | 🚧 | `(.`/`[`, `.)`/`]`, and `@`/`^` aliases are supported; the complete implementation-defined alternative-token policy is not documented. |

## Section 6.2 Blocks, Scopes, and Activations

| ISO clause | Status | Current implementation |
| --- | :---: | --- |
| 6.2 Blocks, scopes, and activations | 🚧 | Program, routine, nested-scope, and activation behavior exists but does not yet model every ISO region and undefined-state rule. |
| 6.2.1 Blocks | 🚧 | Label, constant, type, variable, routine, and statement sections are parsed in order; all ISO declaration forms are not complete. |
| 6.2.2 Scopes | 🚧 | Case-insensitive lexical shadowing works for implemented declarations; complete defining-point/region rules are not implemented. |
| 6.2.3 Activations | 🚧 | Routine calls isolate local values, preserve lexical parents, and support recursion; full activation undefined-state and program-parameter rules remain. |
| 6.2.3.1 Local routines | ✅ | Nested procedures/functions are registered within routine execution. |
| 6.2.3.2 Activation contents | 🚧 | Locals, parameters, results, routines, and labels are represented; all ISO activation contents are not modeled. |
| 6.2.3.3 Activation nesting | 🚧 | Lexical-parent behavior is supported through routine state restoration; a formal activation-record graph is not exposed. |
| 6.2.3.4 Activation points | 🚧 | Calls create isolated routine execution, but activation-point termination semantics are incomplete. |
| 6.2.3.5 Undefined initial state | ⬜ | Variables are initialized to runtime defaults rather than modeled as ISO totally-undefined values. |

## Section 6.3 Constant Definitions

| ISO clause | Status | Current implementation |
| --- | :---: | --- |
| 6.3 Constant-definitions | 🚧 | Scalar constants, expressions, character strings, and nested shadowing are supported; complete constant grammar and undefined-state rules remain. |
| 6.3.1 Constant expressions | 🚧 | Numeric, Boolean, and string constant expressions work; every ISO constant form is not complete. |

## Section 6.4 Type Definitions

| ISO clause | Status | Current implementation |
| --- | :---: | --- |
| 6.4 Type-definitions | 🚧 | Named aliases and several new types are supported; identity, host-type, and recursive type rules remain incomplete. |
| 6.4.1 General | 🚧 | Type denoters and aliases exist; every type-definition restriction is not enforced. |
| 6.4.2 Simple-types | 🚧 | Integer, real, Boolean, char, enumerated, and subrange types are partially modeled. |
| 6.4.2.1 General ordinal types | 🚧 | Integer, Boolean, char, enumeration, and subrange ordinals are supported in selected operations. |
| 6.4.2.2 Required simple-types | 🚧 | Required scalar types exist; the complete implementation-defined character and numeric policies remain. |
| 6.4.2.3 Enumerated-types | 🚧 | Enumeration declarations and member values are supported; full type identity and ordinal compatibility remain. |
| 6.4.2.4 Subrange-types | 🚧 | Integer subranges and assignment bounds checks work; all ordinal host types and constant forms are not complete. |
| 6.4.3 Structured-types | 🚧 | Arrays, records, sets, and files exist in narrowed runtime forms. |
| 6.4.3.1 General structured types | 🚧 | `packed` metadata and structured values exist; full representation and compatibility rules remain. |
| 6.4.3.2 Array-types | 🚧 | Multidimensional integer/character-bounded arrays, indexing, element checks, and bounds errors work; conformant arrays and full string types remain. |
| 6.4.3.3 Record-types | 🚧 | Fixed and simple scalar variant fields, direct selection, and `with` work; active-variant semantics and nested field lists remain. |
| 6.4.3.4 Set-types | 🚧 | Bounded integer sets, constructors, ranges, membership, and algebra work; general ordinal base types and canonical packed sets remain. |
| 6.4.3.5 File-types | 🚧 | Typed in-memory file queues and predefined `text` exist; file modes, buffers, external bindings, and full text line semantics remain. |
| 6.4.4 Pointer-types | 🚧 | Named pointers, `nil`, heap cells, allocation, disposal, and dereference errors work; domain types, identifying values, and lifetime rules are narrower. |
| 6.4.5 Compatible types | 🚧 | Selected scalar, ordinal, set, string, pointer, and structured compatibility rules exist; full ISO compatibility is incomplete. |
| 6.4.6 Assignment-compatibility | 🚧 | Scalar promotion, char/string, subrange, set, pointer, and structured assignment checks exist; full value-range and aggregate rules remain. |
| 6.4.7 Example type-definition part | ⬜ | The standard's complete example type universe is not accepted as a conformance fixture. |

## Section 6.5 Variables and Variable Access

| ISO clause | Status | Current implementation |
| --- | :---: | --- |
| 6.5 Declarations and denotations of variables | 🚧 | Scalar and selected structured variables execute; complete variable-access categories are not implemented. |
| 6.5.1 Variable-declarations | 🚧 | Grouped variables and named/new supported types are parsed; total undefined state and all type denoters remain incomplete. |
| 6.5.2 Entire-variables | ✅ | Identifier variables can be assigned and read. |
| 6.5.3 Component-variables | 🚧 | Indexed variables and field designators exist; all component semantics remain. |
| 6.5.3.1 General component variables | 🚧 | Arrays, fields, and pointer dereferences are represented as components; full variable-access composition is incomplete. |
| 6.5.3.2 Indexed-variables | 🚧 | One or more subscripts, ordinal bounds, element checks, and runtime bounds diagnostics work. |
| 6.5.3.3 Field-designators | 🚧 | Fixed/simple variant fields and `with` access work; active variant restrictions and nested selectors remain. |
| 6.5.4 Identified-variables | 🚧 | Pointer dereference works for the simplified heap-cell model; full identified-variable lifetime/reference rules remain. |
| 6.5.5 Buffer-variables | ⬜ | File buffer variables using the ISO `^` access model are not implemented. |

## Section 6.6 Procedures and Functions

| ISO clause | Status | Current implementation |
| --- | :---: | --- |
| 6.6 Procedure and function declarations | 🚧 | User procedures/functions and selected predefined routines execute; full required routine set and parameter kinds remain. |
| 6.6.1 Procedure-declarations | 🚧 | Procedure bodies, calls, value/var parameters, nesting, and forward procedures work; full declaration identity rules remain. |
| 6.6.2 Function-declarations | 🚧 | Function bodies, results, value parameters, nesting, recursion, and forward-like registration work; all result and termination rules remain. |
| 6.6.3 Parameters | 🚧 | Value and `var` parameters work; procedural, functional, and conformant-array parameters remain. |
| 6.6.3.1 General parameters | 🚧 | Formal lists and value/var sections are parsed; all parameter-section alternatives remain. |
| 6.6.3.2 Value parameters | ✅ | Values are copied into isolated calls and validated against declared types. |
| 6.6.3.3 Variable parameters | 🚧 | Identifier-based `var` parameters copy changes back; full variable-access and exact-type rules remain. |
| 6.6.3.4 Procedural parameters | ⬜ | Passing procedures as parameters is not implemented. |
| 6.6.3.5 Functional parameters | ⬜ | Passing functions as parameters is not implemented. |
| 6.6.3.6 Parameter-list congruity | ⬜ | Formal-list congruity is not implemented. |
| 6.6.3.7 Conformant-array parameters | ⬜ | Level 1 conformant-array parameters are not implemented. |
| 6.6.3.7.1 General conformant arrays | ⬜ | Not implemented. |
| 6.6.3.7.2 Value conformant arrays | ⬜ | Not implemented. |
| 6.6.3.7.3 Variable conformant arrays | ⬜ | Not implemented. |
| 6.6.3.8 Conformability | ⬜ | Not implemented. |
| 6.6.4 Required procedures and functions | 🚧 | Several required routines exist; the complete ISO required set does not. |
| 6.6.5 Required procedures | 🚧 | `new`, `dispose`, `pack`, `unpack`, and selected read/write behavior exist; `get`, `put`, `reset`, `rewrite`, and `page` remain. |
| 6.6.5.1 General required procedures | 🚧 | Runtime procedure dispatch exists but does not cover the complete set. |
| 6.6.5.2 File handling procedures | 🚧 | Simplified queue transfers replace ISO file modes/buffers and are not clause-complete. |
| 6.6.5.3 Dynamic allocation procedures | 🚧 | `new`/`dispose` heap cells work; variant-specific forms and reference invalidation remain. |
| 6.6.5.4 Transfer procedures | 🚧 | `pack`/`unpack` copy bounded arrays; exact packed/unpacked type requirements remain. |
| 6.6.6 Required functions | 🚧 | Arithmetic, transfer, ordinal, and selected Boolean functions exist; `odd`, `eof`, and `eoln` remain. |
| 6.6.6.1 General required functions | 🚧 | Function dispatch exists but the complete required set is incomplete. |
| 6.6.6.2 Arithmetic functions | ✅ | `abs`, `sqr`, `sqrt`, `sin`, `cos`, `tan`, `arctan`, `exp`, and `ln` are implemented with domain diagnostics. |
| 6.6.6.3 Transfer functions | ✅ | `trunc` and `round` are implemented. |
| 6.6.6.4 Ordinal functions | ✅ | `ord`, `chr`, `succ`, and `pred` are implemented with character range checks. |
| 6.6.6.5 Boolean functions | 🚧 | Boolean function infrastructure exists; `odd`, `eof`, and `eoln` are incomplete. |

## Section 6.7 Expressions

| ISO clause | Status | Current implementation |
| --- | :---: | --- |
| 6.7 Expressions | 🚧 | Expression parsing/evaluation is broad for the implemented subset; complete variable-access and type rules remain. |
| 6.7.1 General expressions | 🚧 | Precedence, grouping, literals, calls, sets, indexing, fields, and dereference work; undefined-state and all factor forms remain. |
| 6.7.2 Operators | 🚧 | Arithmetic, Boolean, set, relational, and string operations are partially implemented. |
| 6.7.2.1 General operators | ✅ | Operator classes and precedence are implemented for supported tokens. |
| 6.7.2.2 Arithmetic operators | 🚧 | Integer/real arithmetic, overflow, division, and modulo diagnostics work; every ISO numeric rule is not complete. |
| 6.7.2.3 Boolean operators | ✅ | `and`, `or`, and `not` validate Boolean operands. |
| 6.7.2.4 Set operators | 🚧 | Integer set union, difference, and intersection work; general compatible set types remain. |
| 6.7.2.5 Relational operators | 🚧 | Numeric, Boolean, character, string, and set equality are supported; full compatible string/set ordering remains. |
| 6.7.3 Function-designators | 🚧 | User/predefined function calls, arity, and selected type validation work; all formal parameter kinds and undefined-result rules remain. |

## Section 6.8 Statements

| ISO clause | Status | Current implementation |
| --- | :---: | --- |
| 6.8 Statements | 🚧 | Most core statement families execute; complete ISO variable-access, file, and goto semantics remain. |
| 6.8.1 General statements | ✅ | Statements can be labeled and composed into executable blocks. |
| 6.8.2 Simple-statements | 🚧 | Assignment, procedure call, input/output, allocation, and goto variants exist; empty/file-buffer forms remain. |
| 6.8.2.1 General simple statements | ✅ | Empty statements, assignments, calls, and goto are represented. |
| 6.8.2.2 Assignment-statements | 🚧 | Scalar and selected structured assignments work; complete variable-access and undefined-state rules remain. |
| 6.8.2.3 Procedure-statements | 🚧 | User calls, value/var parameters, and selected required I/O calls work; procedural/functional parameters remain. |
| 6.8.2.4 Goto-statements | 🚧 | Numeric same-block goto works; full enclosing-block activation termination rules remain. |
| 6.8.3 Structured-statements | ✅ | Compound, conditional, repetitive, and `with` statement families are parsed/executed. |
| 6.8.3.1 General structured statements | ✅ | Statement sequences execute in textual order with supported goto behavior. |
| 6.8.3.2 Compound-statements | ✅ | `begin ... end` supports sequencing and empty statements. |
| 6.8.3.3 Conditional-statements | ✅ | `if` and `case` are implemented. |
| 6.8.3.4 If-statements | ✅ | Boolean conditions and nearest-`else` behavior are implemented. |
| 6.8.3.5 Case-statements | 🚧 | Scalar ordinal labels, alternatives, and `else` work; all case-constant and no-match rules remain. |
| 6.8.3.6 Repetitive-statements | ✅ | `repeat`, `while`, and `for` are implemented. |
| 6.8.3.7 Repeat-statements | ✅ | Post-test loop execution and Boolean conditions are implemented. |
| 6.8.3.8 While-statements | ✅ | Pre-test loop execution and Boolean conditions are implemented. |
| 6.8.3.9 For-statements | 🚧 | Integer/character ordinal `to`/`downto` loops work; ISO undefined-after-loop and threatening-statement rules remain. |
| 6.8.3.10 With-statements | 🚧 | Fixed/simple scalar record scopes work; full record-variable lists and field-region rules remain. |

## Section 6.9 Input and Output

| ISO clause | Status | Current implementation |
| --- | :---: | --- |
| 6.9 Input and output | 🚧 | Console and in-memory file I/O exist; ISO text-file modes, buffers, and page behavior remain. |
| 6.9.1 Procedure read | 🚧 | Scalar console/file reads and multiple targets work; ISO textfile buffer and mode semantics remain. |
| 6.9.2 Procedure readln | 🚧 | Multi-target and parameterless console line behavior works; full textfile line-sequence semantics remain. |
| 6.9.3 Procedure write | 🚧 | Scalar output, formatting, spacing option, and typed file writes work; full textfile representation remains. |
| 6.9.3.1 Write-parameters | ✅ | `expression`, `expression:width`, and `expression:width:precision` are supported. |
| 6.9.3.2 Char-type output | 🚧 | Character output and widths are supported through the generic formatter; full ISO representation policy remains. |
| 6.9.3.3 Integer-type output | 🚧 | Integer output and widths work; exact ISO sign/width formatting remains. |
| 6.9.3.4 Real-type output | 🚧 | Fixed precision output works; full floating-point representation and implementation-defined exponent policy remain. |
| 6.9.3.4.1 Floating-point representation | ⬜ | Not implemented as specified. |
| 6.9.3.4.2 Fixed-point representation | 🚧 | Fixed decimal formatting works; exact ISO rounding/width rules remain. |
| 6.9.3.5 Boolean-type output | 🚧 | `TRUE`/`FALSE` output works; exact ISO width/case policy remains configurable only indirectly. |
| 6.9.3.6 String-type output | 🚧 | String output works; fixed string-type truncation/width rules remain. |
| 6.9.4 Procedure writeln | 🚧 | Newline and configured spacing work; full file generation semantics remain. |
| 6.9.5 Procedure page | ⬜ | Not implemented. |

## Section 6.10 Programs

| ISO clause | Status | Current implementation |
| --- | :---: | --- |
| 6.10 Programs | 🚧 | Program headings, parameters, blocks, source-file execution, and CLI exit codes work; standard program-file binding remains simplified. |
| 6.10.1 Program headings | ✅ | Program name and parameter lists are parsed. |
| 6.10.2 Program blocks | 🚧 | Declaration and statement blocks execute; all ISO block/type/declaration semantics remain incomplete. |
| 6.10.3 Program parameters | 🚧 | `input`/`output` names are accepted and bound to injectable services; complete external file binding is not implemented. |

## Required Identifiers and Policies

| ISO area | Status | Current policy |
| --- | :---: | --- |
| Required identifiers | 🚧 | Implemented: `abs`, `arctan`, `Boolean`, `char`, `chr`, `cos`, `dispose`, `exp`, `false`, `input`, `integer`, `ln`, `maxint` policy, `new`, `ord`, `output`, `pack`, `pred`, `read`, `readln`, `real`, `round`, `sin`, `sqr`, `sqrt`, `succ`, `text`, `true`, `trunc`, `unpack`, `write`, `writeln`; missing: `eof`, `eoln`, `get`, `odd`, `page`, `put`, `reset`, `rewrite`. |
| Implementation-defined character set | 🚧 | Character values are represented by .NET strings and bounded to ordinals `0..255`. |
| `maxint` | ✅ | 64-bit signed integer policy: `9223372036854775807`. |
| Real arithmetic accuracy | 🚧 | Uses .NET `double`; exact ISO implementation-defined accuracy is not separately specified. |
| Output field defaults | 🚧 | Width/precision formatting is implemented; all ISO default-width policies are not fully specified. |
| Output spacing extension | ✅ | `StrictIsoSpacing=true` preserves ISO-adjacent output; `false` adds separators for book/UCSD-style output. |

## Conformance Summary

The current processor should be described as an **educational, partial ISO 7185 implementation**. It supports a substantial level-0-oriented subset, including programs, declarations, control flow, routines, arrays, records, sets, pointers, input/output, and source-correlated diagnostics. It is **not** currently an ISO 7185 level-0 or level-1 conforming processor because multiple required clauses, required identifiers, file semantics, undefined-state rules, parameter kinds, and conformance fixtures remain incomplete.

For implementation evidence, see [FEATURE-MATRIX.md](FEATURE-MATRIX.md), the focused tests under `tests/`, and the Phase 4 fixture runner. The reference standard is [iso7185.pdf](iso7185.pdf); it is retained as a local reference document and is not reproduced here.
