using Cjb.StandardPascal.Language.Scanner;
using Cjb.StandardPascal.Language.Parser.Types;

namespace Cjb.StandardPascal.Language.Interpreter.Runtime;

public sealed class ArrayValue
{
    private readonly object[] _elements;

    private readonly IReadOnlyList<ArrayBound> _bounds;

    public ArrayValue(IReadOnlyList<ArrayBound> bounds, object defaultValue)
    {
        _bounds = bounds;
        int count = bounds.Aggregate(1, static (size, bound) => checked(size * (int)(bound.Upper - bound.Lower + 1)));
        _elements = Enumerable.Repeat(defaultValue, count).ToArray();
    }

    public object Get(IReadOnlyList<long> indices, SourceSpan span) => _elements[Offset(indices, span)];

    public void Set(IReadOnlyList<long> indices, object value, SourceSpan span) => _elements[Offset(indices, span)] = value;

    public void CopyFrom(ArrayValue source, long sourceStart, SourceSpan span)
    {
        for (int offset = 0; offset < _elements.Length; offset++)
        {
            _elements[offset] = source.Get([sourceStart + offset], span);
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
}