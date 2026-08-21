namespace Cjb.StandardPascal.Language.Interpreter.Runtime;

public sealed class FileValue
{
    public Queue<object> Items { get; } = [];
}