using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Expressions;

public sealed class Dereference : Expression
{
    public Dereference(Token name, SourceSpan span) : base(span) { Name = name; }
    public Token Name { get; }
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitDereferenceExpression(this);
}