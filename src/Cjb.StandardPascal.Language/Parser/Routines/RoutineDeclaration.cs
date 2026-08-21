using Cjb.StandardPascal.Language.Parser.Declarations;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Routines;

public abstract class RoutineDeclaration : Declaration
{
    protected RoutineDeclaration(SourceSpan span)
        : base(span)
    {
    }
}