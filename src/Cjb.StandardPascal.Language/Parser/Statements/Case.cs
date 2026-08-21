using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Statements;

public sealed class Case : IStatement
{
    public Case(Expression selector, IReadOnlyList<CaseBranch> branches, IStatement? elseBranch, SourceSpan span)
    {
        Selector = selector;
        Branches = branches.ToArray();
        ElseBranch = elseBranch;
        Span = span;
    }

    public Expression Selector { get; }
    public IReadOnlyList<CaseBranch> Branches { get; }
    public IStatement? ElseBranch { get; }
    public SourceSpan Span { get; }
    public T Accept<T>(IVisitor<T> visitor) => visitor.VisitCaseStatement(this);
}

public sealed class CaseBranch
{
    public CaseBranch(IReadOnlyList<Expression> labels, IStatement statement)
    {
        Labels = labels.ToArray();
        Statement = statement;
    }

    public IReadOnlyList<Expression> Labels { get; }
    public IStatement Statement { get; }
}