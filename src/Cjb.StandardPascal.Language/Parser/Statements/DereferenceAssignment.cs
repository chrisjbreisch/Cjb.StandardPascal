using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Statements;

public sealed class DereferenceAssignment : IStatement
{
    public DereferenceAssignment(Token name, Expression value, SourceSpan span) { Name = name; Value = value; Span = span; }
    public Token Name { get; }
    public Expression Value { get; }
    public SourceSpan Span { get; }
    public T Accept<T>(IVisitor<T> visitor) => visitor.VisitDereferenceAssignmentStatement(this);
}