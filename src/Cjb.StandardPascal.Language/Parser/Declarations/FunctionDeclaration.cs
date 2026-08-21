using Cjb.StandardPascal.Language.Parser.Routines;
using Cjb.StandardPascal.Language.Parser.Types;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Declarations;

public sealed class FunctionDeclaration : RoutineDeclaration
{
    public FunctionDeclaration(Token name, IReadOnlyList<RoutineParameter> parameters, TypeSyntax returnType, Block body, SourceSpan span) : base(span) { Name = name; Parameters = parameters.ToArray(); ReturnType = returnType; Body = body; }
    public Token Name { get; }
    public IReadOnlyList<RoutineParameter> Parameters { get; }
    public TypeSyntax ReturnType { get; }
    public Block Body { get; }
}