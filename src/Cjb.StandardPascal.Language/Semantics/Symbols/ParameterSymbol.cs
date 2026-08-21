using Cjb.StandardPascal.Language.Scanner;
using Cjb.StandardPascal.Language.Semantics.Types;

namespace Cjb.StandardPascal.Language.Semantics.Symbols;

public sealed class ParameterSymbol : Symbol
{
    public ParameterSymbol(string name, PascalType type, SourceSpan span, bool isVariable)
        : base(name, type, span)
    {
        IsVariable = isVariable;
    }

    public bool IsVariable { get; }
}