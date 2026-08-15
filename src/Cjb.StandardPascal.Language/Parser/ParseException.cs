using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser;

public sealed class ParseException : Exception
{
    public ParseException(string message, SourceSpan span)
        : base(message)
    {
        Span = span;
    }

    public SourceSpan Span { get; }
}