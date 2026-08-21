using Cjb.StandardPascal.Language.Parser;
using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Parser.Statements;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Tests.Parser;

[TestClass]
public sealed class AstNodeTest
{
    [TestMethod]
    public void Block_With_Statement_Preserves_Source_Span()
    {
        SourceSpan span = new("program.pas", 4, 8, 1, 5);
        Print print = new(new Literal(new Token(
            TokenType.Number,
            "1",
            1L,
            span)), span);

        Block block = new([], [print], span);

        Assert.AreEqual(span, block.Span);
        Assert.HasCount(0, block.Declarations);
        Assert.ContainsSingle(block.Statements);
        Assert.AreSame(print, block.Statements[0]);
    }
}