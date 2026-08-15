using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Expressions;

public sealed class Unary : Expression
{
    public Unary(Token unaryOperator, Expression right)
        : base(Combine(unaryOperator.Span, right.Span))
    {
        UnaryOperator = unaryOperator;
        Right = right;
    }

    public Expression Right { get; }

    public Token UnaryOperator { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitUnaryExpression(this);
    }

    private static SourceSpan Combine(SourceSpan start, SourceSpan end)
    {
        return new SourceSpan(
            start.FilePath,
            start.Start,
            end.Start + end.Length - start.Start,
            start.Line,
            start.Column);
    }
}