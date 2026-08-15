using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Expressions;

public abstract class Expression
{
    protected Expression(SourceSpan span)
    {
        Span = span;
    }

    public SourceSpan Span { get; }

    public abstract T Accept<T>(IVisitor<T> visitor);
}