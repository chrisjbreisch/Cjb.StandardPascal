namespace Cjb.StandardPascal.Language.Semantics.Types;

public sealed class SubrangePascalType : PascalType
{
    public SubrangePascalType(string name, long minimum, long maximum) : base(name) { Minimum = minimum; Maximum = maximum; }
    public long Minimum { get; }
    public long Maximum { get; }
    public override bool IsAssignmentCompatibleWith(PascalType source) => ReferenceEquals(source, PascalTypes.Integer) || ReferenceEquals(source, this);
}