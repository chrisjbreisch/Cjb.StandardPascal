namespace Cjb.StandardPascal.Language.Semantics.Types;

public abstract class PascalType
{
    protected PascalType(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public string Name { get; }

    public virtual bool IsAssignmentCompatibleWith(PascalType source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return ReferenceEquals(this, source);
    }
}