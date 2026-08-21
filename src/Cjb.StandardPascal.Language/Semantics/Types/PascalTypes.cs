namespace Cjb.StandardPascal.Language.Semantics.Types;

public static class PascalTypes
{
    public static PrimitivePascalType Integer { get; } = new("integer");

    public static PrimitivePascalType Real { get; } = new("real");

    public static PrimitivePascalType Boolean { get; } = new("boolean");

    public static PrimitivePascalType Character { get; } = new("char");

    public static PrimitivePascalType Void { get; } = new("void");
}