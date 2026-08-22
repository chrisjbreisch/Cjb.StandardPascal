using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Statements;

public sealed class Read : IStatement
{
    public Read(IReadOnlyList<Token> targets, bool readLine, SourceSpan span) { Targets = targets.ToArray(); ReadLine = readLine; Span = span; }
    public IReadOnlyList<Token> Targets { get; }
    public bool ReadLine { get; }
    public SourceSpan Span { get; }
    public T Accept<T>(IVisitor<T> visitor) => visitor.VisitReadStatement(this);
}