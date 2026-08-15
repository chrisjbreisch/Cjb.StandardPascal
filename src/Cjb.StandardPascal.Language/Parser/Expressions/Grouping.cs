using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Expressions;

public sealed class Grouping : Expression
{
    public Grouping(
        Expression expression,
        SourceSpan span)
        : base(span)
    {
        InnerExpression = expression;
    }

    public Expression InnerExpression { get; }

    public override T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitGroupingExpression(this);
    }
}