using Cjb.StandardPascal.Language.Interpreter;

namespace Cjb.StandardPascal.Application;

public sealed class ConsoleOutput : IOutput
{
    private readonly IConsole _console;

    public ConsoleOutput(IConsole console)
    {
        _console = console ?? throw new ArgumentNullException(nameof(console));
    }

    public void Write(string value) => _console.Write(value);

    public void WriteLine(string value) => _console.WriteLine(value);
}