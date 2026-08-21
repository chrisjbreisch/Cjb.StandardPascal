using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Statements;

public sealed class Goto : IStatement
{
    public Goto(Token label, SourceSpan span) { Label = label; Span = span; }
    public Token Label { get; }
    public SourceSpan Span { get; }
    public T Accept<T>(IVisitor<T> visitor) => visitor.VisitGotoStatement(this);
}