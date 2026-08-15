# Cjb.StandardPascal

A command-line interpreter for ISO 7185 Standard Pascal, implemented in C# and
designed to run Pascal source files directly without a machine emulator.

> [!IMPORTANT]
> The project is in early development. Scanning and parsing for simple
> expressions are implemented, but Pascal programs cannot be executed yet. See the
> [feature matrix](FEATURE-MATRIX.md) for exact progress.

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
    Interpreter/                     Tree-walking execution (planned)
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

Run the interactive expression scanner and parser:

```powershell
dotnet run --project src/Cjb.StandardPascal.Cli
```

Enter one expression per prompt. The console prints scanner tokens followed by
a parenthesized parse tree. A blank line or end-of-input closes the session.
Source-file arguments and program execution are planned but not yet implemented.

The temporary `Print` statement reserves the first output-oriented syntax for
the interpreter milestone:

```pascal
Print 3 * 5;
```

It currently parses as `(print (* 3 5))`; it will produce `15` once expression
interpretation is connected.

## Roadmap

Development proceeds in vertical slices:

1. Scan, parse, type-check, and evaluate simple expressions.
2. Add the Pascal program shell, declarations, assignments, and text output.
3. Add structured statements and control flow.
4. Add procedures, functions, nested scopes, and recursion.
5. Add the remaining ISO 7185 types, files, pointers, and labels.

Detailed status and scope are maintained in
[`FEATURE-MATRIX.md`](FEATURE-MATRIX.md).

## Language reference

The implementation targets ISO 7185 Standard Pascal. The
[Standard Pascal rules overview](https://www.standardpascal.org/iso7185rules.html)
is used as an accessible guide, with conformance decisions to be verified
against the formal standard and compatibility tests.
