using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Types;

public sealed class NamedTypeSyntax : TypeSyntax
{
    public NamedTypeSyntax(Token name) : base(name.Span) { Name = name; }
    public Token Name { get; }
}