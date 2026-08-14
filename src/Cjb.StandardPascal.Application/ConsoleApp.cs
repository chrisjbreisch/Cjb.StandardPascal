using Microsoft.Extensions.Logging;

namespace Cjb.StandardPascal.Application;

public sealed class ConsoleApp : IConsoleApp
{
    private readonly ILogger<ConsoleApp> _logger;

    public ConsoleApp(ILogger<ConsoleApp> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public int Run(IReadOnlyList<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);

        _logger.LogInformation(
            "Cjb.StandardPascal started with {ArgumentCount} source arguments.",
            arguments.Count);
        Console.Error.WriteLine(
            "Cjb.StandardPascal is under development; Pascal source execution is not implemented yet.");

        return 64;
    }
}