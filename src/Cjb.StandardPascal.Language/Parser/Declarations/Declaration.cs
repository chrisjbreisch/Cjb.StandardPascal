using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Declarations;

public abstract class Declaration : AstNode
{
    protected Declaration(SourceSpan span)
        : base(span)
    {
    }
}