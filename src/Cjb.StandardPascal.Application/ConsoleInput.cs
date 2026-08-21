using Cjb.StandardPascal.Language.Interpreter;

namespace Cjb.StandardPascal.Application;

public sealed class ConsoleInput : IInput
{
    private readonly IConsole _console;

    public ConsoleInput(IConsole console)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    public string ReadLine()
    {
        return _console.ReadLine() ?? throw new InvalidOperationException("Input ended unexpectedly.");
    }
}