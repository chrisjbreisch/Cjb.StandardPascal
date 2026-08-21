using Cjb.StandardPascal.Language.Parser.Statements;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser;

public sealed class Program : AstNode
{
    public Program(IStatement body, SourceSpan span)
        : base(span)
    {
        Body = body ?? throw new ArgumentNullException(nameof(body));
    }

    public IStatement Body { get; }
}