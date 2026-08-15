using System.Globalization;

using Cjb.StandardPascal.Language.Interpreter;
using Cjb.StandardPascal.Language.Parser;
using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Parser.Statements;
using Cjb.StandardPascal.Language.Scanner;

using Microsoft.Extensions.Logging;

namespace Cjb.StandardPascal.Application;

public sealed class ConsoleApp : IConsoleApp
{
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