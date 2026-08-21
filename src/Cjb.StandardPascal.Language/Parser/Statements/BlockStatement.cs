using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser.Statements;

public sealed class BlockStatement : IStatement
{
    public BlockStatement(Block block)
    {
        Block = block ?? throw new ArgumentNullException(nameof(block));
    }

    public Block Block { get; }

    public SourceSpan Span => Block.Span;

    public T Accept<T>(IVisitor<T> visitor)
    {
        return visitor.VisitBlockStatement(this);
    }
}