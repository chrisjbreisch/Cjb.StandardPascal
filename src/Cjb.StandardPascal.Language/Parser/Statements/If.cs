using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Statements;

public sealed class If : IStatement
{
    public If(Expression condition, IStatement thenBranch, IStatement? elseBranch, SourceSpan span)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
        Span = span;
    }

    public Expression Condition { get; }
    public IStatement ThenBranch { get; }
    public IStatement? ElseBranch { get; }
    public SourceSpan Span { get; }
    public T Accept<T>(IVisitor<T> visitor) => visitor.VisitIfStatement(this);
}