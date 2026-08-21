using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Types;

public sealed class FileTypeSyntax : TypeSyntax
{
    public FileTypeSyntax(TypeSyntax elementType, SourceSpan span) : base(span) { ElementType = elementType; }
    public TypeSyntax ElementType { get; }
}