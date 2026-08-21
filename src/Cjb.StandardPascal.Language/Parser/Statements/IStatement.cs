using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Statements;

public interface IStatement
{
    SourceSpan Span { get; }

    T Accept<T>(IVisitor<T> visitor);
}