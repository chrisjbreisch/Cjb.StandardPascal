using Cjb.StandardPascal.Language.Semantics.Types;

namespace Cjb.StandardPascal.Language.Interpreter.Runtime;

public sealed class RuntimeValue
{
    public RuntimeValue(PascalType type, object? value)
    {
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Value = value;
    }

    public PascalType Type { get; }

    public object? Value { get; }
}