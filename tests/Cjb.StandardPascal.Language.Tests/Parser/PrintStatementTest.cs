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

    [TestMethod]
    public void ParseProgram_Print_Statement_Returns_Source_Spanned_Program()
    {
        List<Token> tokens = _scanner.ScanTokens(
            new SourceText("Print 3 * 5;", "program.pas"));

        Program program = _parser.ParseProgram(tokens);

        Print print = Assert.IsInstanceOfType<Print>(program.Body);
        Assert.AreEqual(
            new SourceSpan("program.pas", 0, 12, 1, 1),
            program.Span);
        Assert.AreEqual(TokenType.Star, ((Binary)print.Expression).BinaryOperator.Type);
    }

    [TestMethod]
    public void ParseProgram_Empty_Headed_Block_Returns_Program_Block()
    {
        List<Token> tokens = _scanner.ScanTokens(
            new SourceText("program Demo(input, output); begin end.", "demo.pas"));

        Program program = _parser.ParseProgram(tokens);

        Assert.AreEqual("Demo", program.Name!.Lexeme);
        Assert.HasCount(2, program.FileParameters);
        Assert.IsNotNull(program.Block);
        Assert.IsEmpty(program.Block.Declarations);
        Assert.IsEmpty(program.Block.Statements);
        Assert.AreEqual(new SourceSpan("demo.pas", 0, 39, 1, 1), program.Span);
    }
}