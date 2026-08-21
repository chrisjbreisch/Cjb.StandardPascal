using Cjb.StandardPascal.Language.Parser.Declarations;
using Cjb.StandardPascal.Language.Parser.Statements;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser;

public sealed class Block : AstNode
{
    public Block(
        IReadOnlyList<Declaration> declarations,
        IReadOnlyList<IStatement> statements,
        SourceSpan span)
        : base(span)
    {
        ArgumentNullException.ThrowIfNull(declarations);
        ArgumentNullException.ThrowIfNull(statements);
        Declarations = declarations.ToArray();
        Statements = statements.ToArray();
    }

    public IReadOnlyList<Declaration> Declarations { get; }

    public IReadOnlyList<IStatement> Statements { get; }
}