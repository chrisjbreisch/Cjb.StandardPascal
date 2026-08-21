using Cjb.StandardPascal.Language.Scanner;
using Cjb.StandardPascal.Language.Semantics.Types;

namespace Cjb.StandardPascal.Language.Semantics.Symbols;

public sealed class ConstantSymbol : Symbol
{
    public ConstantSymbol(string name, PascalType type, object value, SourceSpan span)
        : base(name, type, span)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public object Value { get; }
}