using Cjb.StandardPascal.Language.Parser.Routines;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Declarations;

public sealed class ProcedureDeclaration : RoutineDeclaration
{
    public ProcedureDeclaration(Token name, IReadOnlyList<RoutineParameter> parameters, Block body, SourceSpan span) : base(span) { Name = name; Parameters = parameters.ToArray(); Body = body; }
    public Token Name { get; }
    public IReadOnlyList<RoutineParameter> Parameters { get; }
    public Block Body { get; }
}