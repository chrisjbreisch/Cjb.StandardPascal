# Cjb.StandardPascal

A command-line interpreter for ISO 7185 Standard Pascal, implemented in C# and
designed to run Pascal source files directly without a machine emulator.

> [!IMPORTANT]
> The project is an educational, partial ISO 7185 implementation. It can execute
> a substantial Pascal subset, but it is not currently an ISO 7185 level 0 or
> level 1 conforming processor. See the [feature matrix](FEATURE-MATRIX.md) and
> the [clause-level compatibility ledger](ISO-COMPATIBILITY.md) for exact scope.

## Goals

- Implement ISO 7185 Pascal incrementally, beginning with expressions.
- Keep scanning, parsing, semantic analysis, and execution clearly separated.
- Produce useful file, line, and column diagnostics.
- Run consistently on every platform supported by .NET.
- Track implemented and remaining language features explicitly.

The architecture is informed by
[`Trs80.Level1Basic`](https://github.com/chrisjbreisch/Trs80.Level1Basic), while
omitting its virtual-machine and host-machine layers. Pascal's declarations and
strong type rules will instead be handled by a dedicated semantic-analysis
phase.

## Project structure

```text
src/
  Cjb.StandardPascal.Language/
    Scanner/                         Tokens and lexical analysis
    Parser/                          Recursive-descent parser and expression AST
    Interpreter/                     Tree-walking expression execution
  Cjb.StandardPascal.Application/    Configuration, logging, and dependency injection
  Cjb.StandardPascal.Cli/            Command-line entry point
tests/
  Cjb.StandardPascal.Language.Tests/ Language unit and integration tests
  Cjb.StandardPascal.Cli.Tests/      Command-line integration tests
```

## Getting started

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0),
then build and test the solution:

```powershell
dotnet build Cjb.StandardPascal.sln
dotnet test Cjb.StandardPascal.sln
```

Run the interactive expression interpreter:

```powershell
dotnet run --project src/Cjb.StandardPascal.Cli
```

Enter one expression per prompt. The console scans, parses, and evaluates it,
then prints only the interpreted result. A blank line or end-of-input closes
the session. Source-file arguments and full program execution are supported
for the implemented subset. The CLI accepts one or more source paths:

```powershell
dotnet run --project src/Cjb.StandardPascal.Cli -- path\to\program.pas
```

The application also supports the `StrictIsoSpacing` setting. Set it to `false`
for the space-separated output convention used by some educational Pascal texts.

The temporary `Print` statement reserves the first output-oriented syntax for
the interpreter milestone:

```pascal
Print 3 * 5;
```

It parses as `(print (* 3 5))` and produces `15`.

Single-quoted ISO Pascal string literals and doubled quote escaping are
supported:

```pascal
Print 'isn''t this useful?';
```

## Roadmap

Development proceeds in vertical slices:

1. Scan, parse, type-check, and evaluate simple expressions.
2. Add the Pascal program shell, declarations, assignments, and text output.
3. Add structured statements and control flow.
4. Add procedures, functions, nested scopes, and recursion.
5. Add the remaining ISO 7185 types, files, pointers, and labels.

Detailed status and scope are maintained in
[`FEATURE-MATRIX.md`](FEATURE-MATRIX.md). ISO clause status is maintained in
[`ISO-COMPATIBILITY.md`](ISO-COMPATIBILITY.md).

## Language reference

The implementation targets ISO 7185 Standard Pascal. The
[Standard Pascal rules overview](https://www.standardpascal.org/iso7185rules.html)
is used as an accessible guide, with conformance decisions to be verified
against the formal standard and compatibility tests.
