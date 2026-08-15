using Cjb.StandardPascal.Language.Parser;
using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Scanner;

using Microsoft.Extensions.Logging;

namespace Cjb.StandardPascal.Application;

public sealed class ConsoleApp : IConsoleApp
{
    private readonly ILogger<ConsoleApp> _logger;
    private readonly IScanner _scanner;
    private readonly IParser _parser;
    private readonly IExpressionFormatter _expressionFormatter;
    private readonly IConsole _console;

    public ConsoleApp(
        ILogger<ConsoleApp> logger,
        IScanner scanner,
        IParser parser,
        IExpressionFormatter expressionFormatter,
        IConsole console)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _expressionFormatter = expressionFormatter
            ?? throw new ArgumentNullException(nameof(expressionFormatter));
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    public int Run(IReadOnlyList<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);

        _logger.LogInformation(
            "Cjb.StandardPascal started with {ArgumentCount} source arguments.",
            arguments.Count);
        _console.WriteLine("Cjb.StandardPascal expression scanner and parser");
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

            _console.WriteLine("Scan:");
            foreach (Token token in tokens)
            {
                _console.WriteLine($"  {token}");
            }

            Expression parsedExpression = _parser.Parse(tokens);
            _console.WriteLine("Parse:");
            _console.WriteLine($"  {_expressionFormatter.Format(parsedExpression)}");
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
    }
}