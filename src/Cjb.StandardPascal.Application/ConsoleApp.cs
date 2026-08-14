using Cjb.StandardPascal.Language.Scanner;

using Microsoft.Extensions.Logging;

namespace Cjb.StandardPascal.Application;

public sealed class ConsoleApp : IConsoleApp
{
    private readonly ILogger<ConsoleApp> _logger;
    private readonly IScanner _scanner;
    private readonly IConsole _console;

    public ConsoleApp(
        ILogger<ConsoleApp> logger,
        IScanner scanner,
        IConsole console)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _scanner = scanner ?? throw new ArgumentNullException(nameof(scanner));
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    public int Run(IReadOnlyList<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);

        _logger.LogInformation(
            "Cjb.StandardPascal started with {ArgumentCount} source arguments.",
            arguments.Count);
        _console.WriteLine("Cjb.StandardPascal expression scanner");
        _console.WriteLine("Enter an expression. Submit a blank line to exit.");

        while (true)
        {
            _console.Write("> ");
            string? expression = _console.ReadLine();

            if (string.IsNullOrWhiteSpace(expression))
            {
                return 0;
            }

            ScanExpression(expression);
        }
    }

    private void ScanExpression(string expression)
    {
        try
        {
            List<Token> tokens = _scanner.ScanTokens(new SourceText(expression));

            foreach (Token token in tokens)
            {
                _console.WriteLine(token.ToString());
            }
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
    }
}