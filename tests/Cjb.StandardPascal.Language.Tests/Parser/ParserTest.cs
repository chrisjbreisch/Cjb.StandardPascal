using Cjb.StandardPascal.Language.Parser;
using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Tests.Parser;

[TestClass]
public sealed class ParserTest
{
    private readonly IScanner _scanner = new Language.Scanner.Scanner();
    private readonly IParser _parser = new Language.Parser.Parser();

    [TestMethod]
    public void Parser_Respects_Arithmetic_Precedence()
    {
        Expression expression = Parse("1 + 2 * 3");

        Binary addition = Assert.IsInstanceOfType<Binary>(expression);
        Assert.AreEqual(TokenType.Plus, addition.BinaryOperator.Type);
        Assert.IsInstanceOfType<Literal>(addition.Left);

        Binary multiplication = Assert.IsInstanceOfType<Binary>(addition.Right);
        Assert.AreEqual(TokenType.Star, multiplication.BinaryOperator.Type);
    }

    [TestMethod]
    public void Parser_Parses_Left_Associative_Operators()
    {
        Expression expression = Parse("8 - 3 - 2");

        Binary outer = Assert.IsInstanceOfType<Binary>(expression);
        Binary inner = Assert.IsInstanceOfType<Binary>(outer.Left);
        Assert.AreEqual(TokenType.Minus, inner.BinaryOperator.Type);
        Assert.AreEqual(TokenType.Minus, outer.BinaryOperator.Type);
    }

    [TestMethod]
    public void Parser_Parses_Grouping_Unary_And_Identifiers()
    {
        Expression expression = Parse("not (ready or false)");

        Unary not = Assert.IsInstanceOfType<Unary>(expression);
        Assert.AreEqual(TokenType.Not, not.UnaryOperator.Type);
        Grouping grouping = Assert.IsInstanceOfType<Grouping>(not.Right);
        Binary or = Assert.IsInstanceOfType<Binary>(grouping.InnerExpression);
        Assert.IsInstanceOfType<Identifier>(or.Left);
        Assert.IsInstanceOfType<Identifier>(or.Right);
    }

    [TestMethod]
    public void Parser_Parses_One_Relational_Operator()
    {
        Expression expression = Parse("value + 1 <= limit");

        Binary comparison = Assert.IsInstanceOfType<Binary>(expression);
        Assert.AreEqual(TokenType.LessThanOrEqual, comparison.BinaryOperator.Type);
        Assert.IsInstanceOfType<Binary>(comparison.Left);
        Assert.IsInstanceOfType<Identifier>(comparison.Right);
    }

    [TestMethod]
    public void Parser_Rejects_Chained_Relational_Operators()
    {
        ParseException exception = Assert.ThrowsExactly<ParseException>(
            () => Parse("1 < 2 < 3"));

        Assert.AreEqual("Expected the end of the expression.", exception.Message);
        Assert.AreEqual(7, exception.Span.Column);
    }

    [TestMethod]
    public void Parser_Rejects_Missing_Right_Parenthesis()
    {
        ParseException exception = Assert.ThrowsExactly<ParseException>(
            () => Parse("(1 + 2"));

        Assert.AreEqual("Expected ')' after expression.", exception.Message);
        Assert.AreEqual(7, exception.Span.Column);
    }

    [TestMethod]
    public void Parser_Rejects_Missing_Operand()
    {
        ParseException exception = Assert.ThrowsExactly<ParseException>(
            () => Parse("1 +"));

        Assert.AreEqual("Expected an expression.", exception.Message);
        Assert.AreEqual(4, exception.Span.Column);
    }

    [TestMethod]
    public void Parser_Assigns_Complete_Source_Span()
    {
        Expression expression = Parse("  1 + 2 * 3");

        Assert.AreEqual(2, expression.Span.Start);
        Assert.AreEqual(9, expression.Span.Length);
        Assert.AreEqual(1, expression.Span.Line);
        Assert.AreEqual(3, expression.Span.Column);
    }

    private Expression Parse(string source)
    {
        List<Token> tokens = _scanner.ScanTokens(new SourceText(source, "expression.pas"));
        return _parser.Parse(tokens);
    }
}