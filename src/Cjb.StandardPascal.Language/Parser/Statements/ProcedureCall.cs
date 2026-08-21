using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Statements;

public sealed class ProcedureCall : IStatement
{
    public ProcedureCall(Token name, IReadOnlyList<Expression> arguments, SourceSpan span) { Name = name; Arguments = arguments.ToArray(); Span = span; }
    public Token Name { get; }
    public IReadOnlyList<Expression> Arguments { get; }
    public SourceSpan Span { get; }
    public T Accept<T>(IVisitor<T> visitor) => visitor.VisitProcedureCallStatement(this);
}