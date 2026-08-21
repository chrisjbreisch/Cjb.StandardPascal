using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Statements;

public enum ForDirection { To, DownTo }

public sealed class For : IStatement
{
    public For(Token variable, Expression initial, ForDirection direction, Expression limit, IStatement body, SourceSpan span)
    {
        Variable = variable;
        Initial = initial;
        Direction = direction;
        Limit = limit;
        Body = body;
        Span = span;
    }

    public Token Variable { get; }
    public Expression Initial { get; }
    public ForDirection Direction { get; }
    public Expression Limit { get; }
    public IStatement Body { get; }
    public SourceSpan Span { get; }
    public T Accept<T>(IVisitor<T> visitor) => visitor.VisitForStatement(this);
}