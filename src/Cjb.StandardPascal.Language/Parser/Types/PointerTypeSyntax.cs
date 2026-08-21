using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Types;

public sealed class PointerTypeSyntax : TypeSyntax
{
    public PointerTypeSyntax(TypeSyntax targetType, SourceSpan span) : base(span) { TargetType = targetType; }
    public TypeSyntax TargetType { get; }
}