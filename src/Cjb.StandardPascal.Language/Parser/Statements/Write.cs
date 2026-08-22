using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Statements;

public sealed class Write : IStatement
{
    public Write(IReadOnlyList<WriteItem> items, bool appendNewLine, SourceSpan span)
    {
        Items = items.ToArray();
        AppendNewLine = appendNewLine;
        Span = span;
    }

    public bool AppendNewLine { get; }

    public IReadOnlyList<WriteItem> Items { get; }

    public SourceSpan Span { get; }

    public T Accept<T>(IVisitor<T> visitor) => visitor.VisitWriteStatement(this);
}