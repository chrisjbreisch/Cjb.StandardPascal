namespace Cjb.StandardPascal.Language.Scanner;

public sealed class ScanException : Exception
{
    public ScanException(string message, SourceSpan span)
        : base(message)
    {
        Span = span;
    }

    public SourceSpan Span { get; }
}