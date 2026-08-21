namespace Cjb.StandardPascal.Language.Interpreter;

public sealed class NullOutput : IOutput
{
    public void Write(string value) { }

    public void WriteLine(string value) { }
}