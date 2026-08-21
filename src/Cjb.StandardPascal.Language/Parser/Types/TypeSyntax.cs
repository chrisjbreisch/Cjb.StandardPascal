using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Types;

public abstract class TypeSyntax : AstNode
{
    protected TypeSyntax(SourceSpan span)
        : base(span)
    {
    }
}