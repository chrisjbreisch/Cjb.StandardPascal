using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Interpreter;

public sealed class RuntimeException : Exception
{
    public RuntimeException(string message, SourceSpan span)
        : base(message)
    {
        Span = span;
    }

    public SourceSpan Span { get; }
}