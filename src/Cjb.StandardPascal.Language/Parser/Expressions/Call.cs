using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Expressions;

public sealed class Call : Expression
{
    public Call(Token name, IReadOnlyList<Expression> arguments, SourceSpan span)
        : base(span)
    {
        Name = name;
        Arguments = arguments.ToArray();
    }

    public Token Name { get; }
    public IReadOnlyList<Expression> Arguments { get; }
    public override T Accept<T>(IVisitor<T> visitor) => visitor.VisitCallExpression(this);
}