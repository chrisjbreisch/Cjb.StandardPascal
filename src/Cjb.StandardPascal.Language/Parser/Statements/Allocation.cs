using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Statements;

public sealed class Allocation : IStatement
{
    public Allocation(Token target, bool dispose, SourceSpan span) { Target = target; Dispose = dispose; Span = span; }
    public Token Target { get; }
    public bool Dispose { get; }
    public SourceSpan Span { get; }
    public T Accept<T>(IVisitor<T> visitor) => visitor.VisitAllocationStatement(this);
}