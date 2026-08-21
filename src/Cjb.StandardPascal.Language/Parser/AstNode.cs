using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser;

public abstract class AstNode
{
    protected AstNode(SourceSpan span)
    {
        Span = span;
    }

    public SourceSpan Span { get; }
}