using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Scanner;
namespace Cjb.StandardPascal.Language.Parser.Statements;
public sealed class FieldAssignment : IStatement { public FieldAssignment(Token record, Token field, Expression value, SourceSpan span) { Record=record; Field=field; Value=value; Span=span; } public Token Record { get; } public Token Field { get; } public Expression Value { get; } public SourceSpan Span { get; } public T Accept<T>(IVisitor<T> visitor) => visitor.VisitFieldAssignmentStatement(this); }