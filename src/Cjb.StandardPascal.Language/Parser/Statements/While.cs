using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Statements;

public sealed class While : IStatement
{
    public While(Expression condition, IStatement body, SourceSpan span)
    {
        Condition = condition;
        Body = body;
        Span = span;
    }

    public Expression Condition { get; }
    public IStatement Body { get; }
    public SourceSpan Span { get; }
    public T Accept<T>(IVisitor<T> visitor) => visitor.VisitWhileStatement(this);
}