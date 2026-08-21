using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.LValues;

public abstract class LValue : AstNode
{
    protected LValue(SourceSpan span)
        : base(span)
    {
    }
}