using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Statements;

public sealed class Labeled : IStatement
{
    public Labeled(Token label, IStatement statement, SourceSpan span) { Label = label; Statement = statement; Span = span; }
    public Token Label { get; }
    public IStatement Statement { get; }
    public SourceSpan Span { get; }
    public T Accept<T>(IVisitor<T> visitor) => visitor.VisitLabeledStatement(this);
}