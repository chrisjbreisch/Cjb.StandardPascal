using Cjb.StandardPascal.Language.Scanner;
using Cjb.StandardPascal.Language.Semantics.Types;

namespace Cjb.StandardPascal.Language.Semantics.Symbols;

public sealed class VariableSymbol : Symbol
{
    public VariableSymbol(string name, PascalType type, SourceSpan span)
        : base(name, type, span)
    {
    }
}