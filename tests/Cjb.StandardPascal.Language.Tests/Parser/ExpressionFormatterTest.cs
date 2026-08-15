using Cjb.StandardPascal.Language.Parser;
using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Tests.Parser;

[TestClass]
public sealed class ExpressionFormatterTest
{
    private readonly IScanner _scanner = new Language.Scanner.Scanner();
    private readonly IParser _parser = new Language.Parser.Parser();
    private readonly IExpressionFormatter _formatter = new ExpressionFormatter();

    [TestMethod]
    public void Format_Expression_Returns_Parenthesized_Syntax_Tree()
    {
        List<Token> tokens = _scanner.ScanTokens(
            new SourceText("not (value + 2 * 3 <= limit)"));
        Expression expression = _parser.Parse(tokens);

        string result = _formatter.Format(expression);

        Assert.AreEqual(
            "(not (group (<= (+ value (* 2 3)) limit)))",
            result);
    }
}