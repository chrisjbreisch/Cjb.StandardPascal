using Cjb.StandardPascal.Language.Parser.Types;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Declarations;

public sealed class VariableDeclaration : Declaration
{
    public VariableDeclaration(IReadOnlyList<Token> names, TypeSyntax type, SourceSpan span)
        : base(span)
    {
        Names = names.ToArray();
        Type = type;
    }

    public IReadOnlyList<Token> Names { get; }

    public TypeSyntax Type { get; }
}