using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Expressions;

public sealed class Identifier : Expression
{
    public Identifier(Token name)
        : base(name.Span)
    {
        Name = name;
    }

    public Token Name { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitIdentifierExpression(this);
    }
}