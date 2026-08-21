using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Statements;

public sealed class Repeat : IStatement
{
    public Repeat(IReadOnlyList<IStatement> body, Expression condition, SourceSpan span)
    {
        Body = body.ToArray();
        Condition = condition;
        Span = span;
    }

    public IReadOnlyList<IStatement> Body { get; }
    public Expression Condition { get; }
    public SourceSpan Span { get; }
    public T Accept<T>(IVisitor<T> visitor) => visitor.VisitRepeatStatement(this);
}