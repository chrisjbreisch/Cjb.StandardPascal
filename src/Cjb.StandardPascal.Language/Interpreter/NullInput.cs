namespace Cjb.StandardPascal.Language.Interpreter;

public sealed class NullInput : IInput
{
    public string ReadLine() => throw new InvalidOperationException("No input service is configured.");
}