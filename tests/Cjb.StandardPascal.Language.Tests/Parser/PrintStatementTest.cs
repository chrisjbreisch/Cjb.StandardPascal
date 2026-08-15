using Cjb.StandardPascal.Language.Parser;
using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Parser.Statements;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Tests.Parser;

[TestClass]
public sealed class PrintStatementTest
{
    private readonly IScanner _scanner = new Language.Scanner.Scanner();
    private readonly IParser _parser = new Language.Parser.Parser();
    private readonly IStatementFormatter _formatter =
        new StatementFormatter(new ExpressionFormatter());

    [TestMethod]
    public void ParseStatement_Print_Expression_Returns_Print_Statement()
    {
        List<Token> tokens = _scanner.ScanTokens(
            new SourceText("Print 3 * 5;", "statement.pas"));

        IStatement statement = _parser.ParseStatement(tokens);

        Print print = Assert.IsInstanceOfType<Print>(statement);
        Binary multiplication = Assert.IsInstanceOfType<Binary>(print.Expression);
        Assert.AreEqual(TokenType.Star, multiplication.BinaryOperator.Type);
        Assert.AreEqual("(print (* 3 5))", _formatter.Format(statement));
        Assert.AreEqual(
            new SourceSpan("statement.pas", 0, 12, 1, 1),
            print.Span);
    }

    [TestMethod]
    public void ParseStatement_Missing_Semicolon_Throws_Parse_Exception()
    {
        List<Token> tokens = _scanner.ScanTokens(new SourceText("Print 3 * 5"));

        ParseException exception = Assert.ThrowsExactly<ParseException>(
            () => _parser.ParseStatement(tokens));

        Assert.AreEqual(
            "Expected ';' after the Print expression.",
            exception.Message);
        Assert.AreEqual(12, exception.Span.Column);
    }
}