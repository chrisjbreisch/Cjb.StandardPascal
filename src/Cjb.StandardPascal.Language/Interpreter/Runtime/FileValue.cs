using Cjb.StandardPascal.Language.Semantics.Types;

namespace Cjb.StandardPascal.Language.Interpreter.Runtime;

public sealed class FileValue
{
    public FileValue(PascalType? elementType = null) { ElementType = elementType; }

    public PascalType? ElementType { get; }

    public Queue<object> Items { get; } = [];
}