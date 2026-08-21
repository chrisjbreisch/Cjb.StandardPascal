using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Statements;

public sealed class Read : IStatement
{
    public Read(Token target, bool readLine, SourceSpan span) { Target = target; ReadLine = readLine; Span = span; }
    public Token Target { get; }
    public bool ReadLine { get; }
    public SourceSpan Span { get; }
    public T Accept<T>(IVisitor<T> visitor) => visitor.VisitReadStatement(this);
}