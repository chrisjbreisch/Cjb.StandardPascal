using Cjb.StandardPascal.Language.Scanner;
using Cjb.StandardPascal.Language.Semantics.Types;

namespace Cjb.StandardPascal.Language.Parser.Types;

public sealed class ScalarTypeSyntax : TypeSyntax
{
    public ScalarTypeSyntax(Token name, PascalType type)
        : base(name.Span)
    {
        Name = name;
        Type = type;
    }

    public Token Name { get; }

    public PascalType Type { get; }
}