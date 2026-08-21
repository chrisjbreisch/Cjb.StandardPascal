using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Types;

public readonly record struct ArrayBound(long Lower, long Upper);

public sealed class ArrayTypeSyntax : TypeSyntax
{
    public ArrayTypeSyntax(IReadOnlyList<ArrayBound> bounds, TypeSyntax elementType, SourceSpan span) : base(span) { Bounds = bounds.ToArray(); ElementType = elementType; }
    public IReadOnlyList<ArrayBound> Bounds { get; }
    public TypeSyntax ElementType { get; }
}