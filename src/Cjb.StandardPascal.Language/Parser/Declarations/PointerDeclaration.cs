using Cjb.StandardPascal.Language.Parser.Types;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Declarations;

public sealed class PointerDeclaration : Declaration
{
    public PointerDeclaration(Token name, PointerTypeSyntax type, SourceSpan span) : base(span) { Name = name; Type = type; }
    public Token Name { get; }
    public PointerTypeSyntax Type { get; }
}