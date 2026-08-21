using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Expressions;

public sealed class Index : Expression
{
    public Index(Token name, IReadOnlyList<Expression> subscripts, SourceSpan span) : base(span) { Name = name; Subscripts = subscripts.ToArray(); }
    public Token Name { get; }
    public IReadOnlyList<Expression> Subscripts { get; }
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitIndexExpression(this);
}