using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Statements;

public sealed class Assignment : IStatement
{
    public Assignment(Token name, Expression value, SourceSpan span)
    {
        Name = name;
        Value = value;
        Span = span;
    }

    public Token Name { get; }

    public SourceSpan Span { get; }

    public Expression Value { get; }

    public T Accept<T>(IVisitor<T> visitor) => visitor.VisitAssignmentStatement(this);
}