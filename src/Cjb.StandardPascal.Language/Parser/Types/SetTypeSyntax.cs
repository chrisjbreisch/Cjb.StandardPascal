using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Types;

public sealed class SetTypeSyntax : TypeSyntax
{
    public SetTypeSyntax(long lowerBound, long upperBound, SourceSpan span) : base(span)
    {
        LowerBound = lowerBound;
        UpperBound = upperBound;
    }

    public long LowerBound { get; }
    public long UpperBound { get; }
}