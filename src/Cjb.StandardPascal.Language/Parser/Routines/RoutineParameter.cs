using Cjb.StandardPascal.Language.Parser.Types;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Routines;

public sealed class RoutineParameter
{
    public RoutineParameter(Token name, TypeSyntax type, bool isVariable) { Name = name; Type = type; IsVariable = isVariable; }
    public Token Name { get; }
    public TypeSyntax Type { get; }
    public bool IsVariable { get; }
}