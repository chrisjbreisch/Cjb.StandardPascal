using Cjb.StandardPascal.Language.Parser.Types;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Declarations;

public sealed class VariableDeclaration : Declaration
{
    public VariableDeclaration(IReadOnlyList<Token> names, ScalarTypeSyntax type, SourceSpan span)
        : base(span)
    {
        Names = names.ToArray();
        Type = type;
    }

    public IReadOnlyList<Token> Names { get; }

    public ScalarTypeSyntax Type { get; }
}