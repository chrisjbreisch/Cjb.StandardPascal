using Cjb.StandardPascal.Language.Scanner;
using Cjb.StandardPascal.Language.Semantics.Types;

namespace Cjb.StandardPascal.Language.Semantics.Symbols;

public sealed class RoutineSymbol : Symbol
{
    public RoutineSymbol(
        string name,
        IReadOnlyList<ParameterSymbol> parameters,
        PascalType returnType,
        SourceSpan span)
        : base(name, returnType, span)
    {
        ArgumentNullException.ThrowIfNull(parameters);
        Parameters = parameters.ToArray();
    }

    public IReadOnlyList<ParameterSymbol> Parameters { get; }

    public PascalType ReturnType => Type;
}