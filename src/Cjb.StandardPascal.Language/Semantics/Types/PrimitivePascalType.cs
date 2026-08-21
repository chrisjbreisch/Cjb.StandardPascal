namespace Cjb.StandardPascal.Language.Semantics.Types;

public sealed class PrimitivePascalType : PascalType
{
    internal PrimitivePascalType(string name)
        : base(name)
    {
    }

    public override bool IsAssignmentCompatibleWith(PascalType source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return ReferenceEquals(this, source)
            || ReferenceEquals(this, PascalTypes.Real)
                && ReferenceEquals(source, PascalTypes.Integer);
    }
}