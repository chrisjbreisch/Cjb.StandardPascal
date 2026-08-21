using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Expressions;

public abstract class Expression : AstNode
{
    protected Expression(SourceSpan span)
        : base(span)
    {
    }

    public abstract T Accept<T>(IVisitor<T> visitor);
}