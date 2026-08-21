using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Statements;

public sealed class IndexedAssignment : IStatement
{
    public IndexedAssignment(Token name, Expression subscript, Expression value, SourceSpan span) { Name = name; Subscript = subscript; Value = value; Span = span; }
    public Token Name { get; }
    public Expression Subscript { get; }
    public Expression Value { get; }
    public SourceSpan Span { get; }
    public T Accept<T>(IVisitor<T> visitor) => visitor.VisitIndexedAssignmentStatement(this);
}