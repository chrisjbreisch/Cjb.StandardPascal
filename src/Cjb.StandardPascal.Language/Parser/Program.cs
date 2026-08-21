using Cjb.StandardPascal.Language.Parser.Statements;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser;

public sealed class Program
{
    public Program(IStatement body, SourceSpan span)
    {
        Body = body ?? throw new ArgumentNullException(nameof(body));
        Span = span;
    }

    public IStatement Body { get; }

    public SourceSpan Span { get; }
}