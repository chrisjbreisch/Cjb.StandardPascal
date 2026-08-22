using Cjb.StandardPascal.Language.Parser.Types;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Declarations;

public sealed class TypeAliasDeclaration : Declaration
{
    public TypeAliasDeclaration(Token name, TypeSyntax targetType, SourceSpan span) : base(span)
    {
        Name = name;
        TargetType = targetType;
    }

    public Token Name { get; }
    public TypeSyntax TargetType { get; }
}