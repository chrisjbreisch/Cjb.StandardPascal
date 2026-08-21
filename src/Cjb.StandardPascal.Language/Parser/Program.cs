using Cjb.StandardPascal.Language.Parser.Statements;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser;

public sealed class Program : AstNode
{
    public Program(IStatement body, SourceSpan span)
        : base(span)
    {
        Body = body ?? throw new ArgumentNullException(nameof(body));
        FileParameters = [];
    }

    public Program(
        Token name,
        IReadOnlyList<Token> fileParameters,
        Block block,
        SourceSpan span)
        : base(span)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ArgumentNullException.ThrowIfNull(fileParameters);
        Block = block ?? throw new ArgumentNullException(nameof(block));
        FileParameters = fileParameters.ToArray();
        Body = new BlockStatement(Block);
    }

    public IStatement Body { get; }

    public Block? Block { get; }

    public IReadOnlyList<Token> FileParameters { get; }

    public Token? Name { get; }
}