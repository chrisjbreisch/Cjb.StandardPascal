using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Expressions;

public sealed class SetLiteral : Expression
{
    public SetLiteral(IReadOnlyList<Expression> elements, SourceSpan span) : base(span) { Elements = elements.ToArray(); }
    public IReadOnlyList<Expression> Elements { get; }
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitSetLiteralExpression(this);
}