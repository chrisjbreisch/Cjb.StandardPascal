using Cjb.StandardPascal.Language.Parser.Routines;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Declarations;

public sealed class ProcedureDeclaration : RoutineDeclaration
{
    public ProcedureDeclaration(Token name, Block body, SourceSpan span) : base(span) { Name = name; Body = body; }
    public Token Name { get; }
    public Block Body { get; }
}