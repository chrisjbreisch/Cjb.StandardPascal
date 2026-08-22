using Cjb.StandardPascal.Language.Scanner;
using Cjb.StandardPascal.Language.Parser.Types;
using Cjb.StandardPascal.Language.Semantics.Types;

namespace Cjb.StandardPascal.Language.Interpreter.Runtime;

public sealed class ArrayValue
{
    private readonly object[] _elements;

    private readonly IReadOnlyList<ArrayBound> _bounds;

    public ArrayValue(IReadOnlyList<ArrayBound> bounds, PascalType elementType, object defaultValue)
    {
        _bounds = bounds;
        ElementType = elementType ?? throw new ArgumentNullException(nameof(elementType));
        int count = bounds.Aggregate(1, static (size, bound) => checked(size * (int)(bound.Upper - bound.Lower + 1)));
        _elements = Enumerable.Repeat(defaultValue, count).ToArray();
    }

    public PascalType ElementType { get; }

    public object Get(IReadOnlyList<long> indices, SourceSpan span) => _elements[Offset(indices, span)];

    public void Set(IReadOnlyList<long> indices, object value, SourceSpan span)
    {
        if (!IsCompatible(value))
        {
            throw new RuntimeException($"Cannot assign {ValueTypeName(value)} to {ElementType.Name} array element.", span);
        }

        _elements[Offset(indices, span)] = value;
    }

    public void CopyFrom(ArrayValue source, long sourceStart, SourceSpan span)
    {
        for (int offset = 0; offset < _elements.Length; offset++)
        {
            _elements[offset] = source.Get([sourceStart + offset], span);
        }
    }

    public void SetCharacters(string value, SourceSpan span)
    {
        if (value.Length != _elements.Length)
        {
            throw new RuntimeException($"String length {value.Length} does not match array length {_elements.Length}.", span);
        }

        for (int index = 0; index < value.Length; index++)
        {
            _elements[index] = value[index].ToString();
        }
    }

    private int Offset(IReadOnlyList<long> indices, SourceSpan span)
    {
        if (indices.Count != _bounds.Count)
        {
            throw new RuntimeException("Incorrect number of array subscripts.", span);
        }
        int offset = 0;
        for (int dimension = 0; dimension < _bounds.Count; dimension++)
        {
            ArrayBound bound = _bounds[dimension];
            long index = indices[dimension];
            if (index < bound.Lower || index > bound.Upper) { throw new RuntimeException($"Array index {index} is outside {bound.Lower}..{bound.Upper}.", span); }
            offset = checked(offset * (int)(bound.Upper - bound.Lower + 1) + (int)(index - bound.Lower));
        }
        return offset;
    }

    private bool IsCompatible(object value)
    {
        return ElementType switch
        {
            PrimitivePascalType type when ReferenceEquals(type, PascalTypes.Integer) => value is long,
            PrimitivePascalType type when ReferenceEquals(type, PascalTypes.Real) => value is long or double,
            PrimitivePascalType type when ReferenceEquals(type, PascalTypes.Boolean) => value is bool,
            PrimitivePascalType type when ReferenceEquals(type, PascalTypes.Character) => value is string { Length: 1 },
            _ => true,
        };
    }

    private static string ValueTypeName(object value) => value switch
    {
        long => "integer",
        double => "real",
        bool => "boolean",
        string => "char",
        _ => "value",
    };
}