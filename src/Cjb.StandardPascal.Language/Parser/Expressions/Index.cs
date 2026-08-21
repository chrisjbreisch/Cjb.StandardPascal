using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Expressions;

public sealed class Index : Expression
{
    public Index(Token name, Expression subscript, SourceSpan span) : base(span) { Name = name; Subscript = subscript; }
    public Token Name { get; }
    public Expression Subscript { get; }
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitIndexExpression(this);
}