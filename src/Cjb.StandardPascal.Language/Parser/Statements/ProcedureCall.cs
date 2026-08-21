using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Statements;

public sealed class ProcedureCall : IStatement
{
    public ProcedureCall(Token name, SourceSpan span) { Name = name; Span = span; }
    public Token Name { get; }
    public SourceSpan Span { get; }
    public T Accept<T>(IVisitor<T> visitor) => visitor.VisitProcedureCallStatement(this);
}