using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Expressions;

public sealed class Binary : Expression
{
    public Binary(
        Expression left,
        Token binaryOperator,
        Expression right)
        : base(Combine(left.Span, right.Span))
    {
        Left = left;
        BinaryOperator = binaryOperator;
        Right = right;
    }

    public Token BinaryOperator { get; }

    public Expression Left { get; }

    public Expression Right { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitBinaryExpression(this);
    }

    private static SourceSpan Combine(SourceSpan left, SourceSpan right)
    {
        return new SourceSpan(
            left.FilePath,
            left.Start,
            right.Start + right.Length - left.Start,
            left.Line,
            left.Column);
    }
}