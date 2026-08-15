using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Expressions;

public sealed class Literal : Expression
{
    public Literal(Token token)
        : base(token.Span)
    {
        Token = token;
        Value = token.Literal
            ?? throw new ArgumentException("A literal token must have a value.", nameof(token));
    }

    public Token Token { get; }

    public object Value { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitLiteralExpression(this);
    }
}