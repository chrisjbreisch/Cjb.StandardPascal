namespace Cjb.StandardPascal.Language.Semantics.Types;

public sealed class FilePascalType : PascalType
{
    public FilePascalType(PascalType? elementType = null) : base("file") { ElementType = elementType; }

    public PascalType? ElementType { get; }
}