using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Interpreter.Runtime;

public sealed class ArrayValue
{
    private readonly object[] _elements;

    public ArrayValue(long lowerBound, long upperBound, object defaultValue)
    {
        LowerBound = lowerBound;
        UpperBound = upperBound;
        _elements = Enumerable.Repeat(defaultValue, checked((int)(upperBound - lowerBound + 1))).ToArray();
    }

    public long LowerBound { get; }
    public long UpperBound { get; }

    public object Get(long index, SourceSpan span) => _elements[Offset(index, span)];

    public void Set(long index, object value, SourceSpan span) => _elements[Offset(index, span)] = value;

    private int Offset(long index, SourceSpan span)
    {
        if (index < LowerBound || index > UpperBound)
        {
            throw new RuntimeException($"Array index {index} is outside {LowerBound}..{UpperBound}.", span);
        }

        return checked((int)(index - LowerBound));
    }
}