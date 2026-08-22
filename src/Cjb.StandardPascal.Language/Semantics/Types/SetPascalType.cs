namespace Cjb.StandardPascal.Language.Semantics.Types;

public sealed class SetPascalType : PascalType
{
    public SetPascalType(long lowerBound, long upperBound) : base("set")
    {
        LowerBound = lowerBound;
        UpperBound = upperBound;
    }

    public long LowerBound { get; }
    public long UpperBound { get; }
}