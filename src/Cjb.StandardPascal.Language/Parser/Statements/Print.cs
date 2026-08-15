using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Statements;

public sealed class Print : IStatement
{
    public Print(Expression expression, SourceSpan span)
    {
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        Span = span;
    }

    public Expression Expression { get; }

    public SourceSpan Span { get; }

    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitPrintStatement(this);
    }
}