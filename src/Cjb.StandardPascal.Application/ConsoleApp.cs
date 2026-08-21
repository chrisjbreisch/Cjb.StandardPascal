using System.Globalization;

using Cjb.StandardPascal.Language.Interpreter;
using Cjb.StandardPascal.Language.Parser;
using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Parser.Statements;
using Cjb.StandardPascal.Language.Scanner;
using Cjb.StandardPascal.Language.Semantics;

using Microsoft.Extensions.Logging;

namespace Cjb.StandardPascal.Application;

public sealed class ConsoleApp : IConsoleApp
{
    private const int FileErrorExitCode = 1;
    private const int SyntaxErrorExitCode = 2;
    private const int RuntimeErrorExitCode = 3;
    private readonly ILogger<ConsoleApp> _logger;
    private readonly IScanner _scanner;
    private readonly IParser _parser;
    private readonly IInterpreter _interpreter;
    private readonly IConsole _console;

    public ConsoleApp(
        ILogger<ConsoleApp> logger,
        IScanner scanner,
        IParser parser,
        IInterpreter interpreter,
        IConsole console)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _interpreter = interpreter ?? throw new ArgumentNullException(nameof(interpreter));
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    public int Run(IReadOnlyList<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);

        _logger.LogInformation(
            "Cjb.StandardPascal started with {ArgumentCount} source arguments.",
            arguments.Count);

        if (arguments.Count > 0)
        {
            return RunFiles(arguments);
        }

        _console.WriteLine("Cjb.StandardPascal expression interpreter");
        _console.WriteLine("Enter an expression. Submit a blank line to exit.");

        while (true)
        {
            _console.Write("> ");
            string? expression = _console.ReadLine();

            if (string.IsNullOrWhiteSpace(expression))
            {
                return 0;
            }

            ProcessExpression(expression);
        }
    }

    private void ProcessExpression(string expression)
    {
        try
        {
            List<Token> tokens = _scanner.ScanTokens(new SourceText(expression));
            object result = ParseAndInterpret(tokens);
            _console.WriteLine(FormatValue(result));
        }
        catch (ScanException exception)
        {
            _logger.LogWarning(
                exception,
                "Expression scan failed at line {Line}, column {Column}.",
                exception.Span.Line,
                exception.Span.Column);
            _console.WriteLine(
                $"error ({exception.Span.Line},{exception.Span.Column}): {exception.Message}");
        }
        catch (ParseException exception)
        {
            _logger.LogWarning(
                exception,
                "Expression parse failed at line {Line}, column {Column}.",
                exception.Span.Line,
                exception.Span.Column);
            _console.WriteLine(
                $"error ({exception.Span.Line},{exception.Span.Column}): {exception.Message}");
        }
        catch (RuntimeException exception)
        {
            _logger.LogWarning(
                exception,
                "Expression execution failed at line {Line}, column {Column}.",
                exception.Span.Line,
                exception.Span.Column);
            _console.WriteLine(
                $"error ({exception.Span.Line},{exception.Span.Column}): {exception.Message}");
        }
        catch (SemanticException exception)
        {
            WriteError(exception.Message, exception.Span);
        }
    }

    private int RunFiles(IReadOnlyList<string> arguments)
    {
        foreach (string path in arguments)
        {
            try
            {
                string source = File.ReadAllText(path);
                Program program = _parser.ParseProgram(
                    _scanner.ScanTokens(new SourceText(source, path)));
                _interpreter.Execute(program);
            }
            catch (IOException exception)
            {
                _console.WriteLine($"error: {exception.Message}");
                return FileErrorExitCode;
            }
            catch (UnauthorizedAccessException exception)
            {
                _console.WriteLine($"error: {exception.Message}");
                return FileErrorExitCode;
            }
            catch (ScanException exception)
            {
                WriteError(exception.Message, exception.Span);
                return SyntaxErrorExitCode;
            }
            catch (ParseException exception)
            {
                WriteError(exception.Message, exception.Span);
                return SyntaxErrorExitCode;
            }
            catch (SemanticException exception)
            {
                WriteError(exception.Message, exception.Span);
                return SyntaxErrorExitCode;
            }
            catch (RuntimeException exception)
            {
                WriteError(exception.Message, exception.Span);
                return RuntimeErrorExitCode;
            }
        }

        return 0;
    }

    private void WriteError(string message, SourceSpan span)
    {
        _console.WriteLine($"error ({span.Line},{span.Column}): {message}");
    }

    private object ParseAndInterpret(List<Token> tokens)
    {
        if (tokens[0].Type == TokenType.Print)
        {
            IStatement statement = _parser.ParseStatement(tokens);
            return _interpreter.Interpret(statement);
        }

        Expression expression = _parser.Parse(tokens);
        return _interpreter.Evaluate(expression);
    }

    private static string FormatValue(object value)
    {
        return value switch
        {
            bool boolean => boolean ? "TRUE" : "FALSE",
            _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty,
        };
    }
}