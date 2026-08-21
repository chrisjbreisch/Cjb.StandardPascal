using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Types;

public sealed class ArrayTypeSyntax : TypeSyntax
{
    public ArrayTypeSyntax(long lowerBound, long upperBound, TypeSyntax elementType, SourceSpan span) : base(span) { LowerBound = lowerBound; UpperBound = upperBound; ElementType = elementType; }
    public long LowerBound { get; }
    public long UpperBound { get; }
    public TypeSyntax ElementType { get; }
}