using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Semantics;

public sealed class SemanticException : Exception
{
    public SemanticException(string message, SourceSpan span)
        : base(message)
    {
        Span = span;
    }

    public SourceSpan Span { get; }
}