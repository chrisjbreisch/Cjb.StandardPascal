using Cjb.StandardPascal.Language.Scanner;
using Cjb.StandardPascal.Language.Semantics.Types;

namespace Cjb.StandardPascal.Language.Semantics.Symbols;

public abstract class Symbol
{
    protected Symbol(string name, PascalType type, SourceSpan span)
    {
        Name = string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("A symbol name is required.", nameof(name))
            : name;
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Span = span;
    }

    public string Name { get; }

    public PascalType Type { get; }

    public SourceSpan Span { get; }
}