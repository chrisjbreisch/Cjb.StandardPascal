using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Tests.Scanner;

[TestClass]
public sealed class ScannerTest
{
    private readonly IScanner _scanner = new Language.Scanner.Scanner();

    [TestMethod]
    public void Scanner_Can_Scan_A_Simple_Expression_Correctly()
    {
        List<Token> tokens = _scanner.ScanTokens(
            new SourceText("12 + 3.5 * (2 - 1)", "expression.pas"));

        AssertTokenTypes(
            tokens,
            TokenType.Number,
            TokenType.Plus,
            TokenType.Number,
            TokenType.Star,
            TokenType.LeftParen,
            TokenType.Number,
            TokenType.Minus,
            TokenType.Number,
            TokenType.RightParen,
            TokenType.EndOfFile);
        Assert.AreEqual(12L, tokens[0].Literal);
        Assert.AreEqual(3.5, tokens[2].Literal);
        Assert.AreEqual(
            new SourceSpan("expression.pas", 0, 2, 1, 1),
            tokens[0].Span);
    }

    [TestMethod]
    public void Scanner_Recognizes_Expression_Keywords_Case_Insensitively()
    {
        List<Token> tokens = _scanner.ScanTokens(
            new SourceText("NOT ready AnD 8 DIV 2 mod 3 OR item in set1"));

        AssertTokenTypes(
            tokens,
            TokenType.Not,
            TokenType.Identifier,
            TokenType.And,
            TokenType.Number,
            TokenType.Div,
            TokenType.Number,
            TokenType.Mod,
            TokenType.Number,
            TokenType.Or,
            TokenType.Identifier,
            TokenType.In,
            TokenType.Identifier,
            TokenType.EndOfFile);
        Assert.AreEqual("NOT", tokens[0].Lexeme);
        Assert.AreEqual("AnD", tokens[2].Lexeme);
        Assert.AreEqual("set1", tokens[11].Lexeme);
    }

    [TestMethod]
    public void Scanner_Recognizes_Program_Declaration_And_Block_Tokens()
    {
        List<Token> tokens = _scanner.ScanTokens(new SourceText(
            "program Demo(input); const max = 1; var count: integer; begin count := max; end."));

        AssertTokenTypes(
            tokens,
            TokenType.Program,
            TokenType.Identifier,
            TokenType.LeftParen,
            TokenType.Identifier,
            TokenType.RightParen,
            TokenType.Semicolon,
            TokenType.Const,
            TokenType.Identifier,
            TokenType.Equal,
            TokenType.Number,
            TokenType.Semicolon,
            TokenType.Var,
            TokenType.Identifier,
            TokenType.Colon,
            TokenType.Integer,
            TokenType.Semicolon,
            TokenType.Begin,
            TokenType.Identifier,
            TokenType.Assign,
            TokenType.Identifier,
            TokenType.Semicolon,
            TokenType.End,
            TokenType.Dot,
            TokenType.EndOfFile);
    }

    [TestMethod]
    public void Scanner_Skips_Comments_And_Normalizes_Delimiter_Aliases()
    {
        List<Token> tokens = _scanner.ScanTokens(
            new SourceText("{ heading } (. value .) (* body *) [item] @pointer"));

        AssertTokenTypes(
            tokens,
            TokenType.LeftBracket,
            TokenType.Identifier,
            TokenType.RightBracket,
            TokenType.LeftBracket,
            TokenType.Identifier,
            TokenType.RightBracket,
            TokenType.Caret,
            TokenType.Identifier,
            TokenType.EndOfFile);
    }

    [TestMethod]
    [DataRow("{ missing")]
    [DataRow("(* missing")]
    public void Scanner_Reports_Unterminated_Comments(string source)
    {
        ScanException exception = Assert.ThrowsExactly<ScanException>(
            () => _scanner.ScanTokens(new SourceText(source, "comment.pas")));

        Assert.AreEqual("Unterminated comment.", exception.Message);
        Assert.AreEqual(new SourceSpan("comment.pas", 0, source.Length, 1, 1), exception.Span);
    }

    [TestMethod]
    [DataRow("1.0", 1.0)]
    [DataRow("1e-12", 1e-12)]
    [DataRow("25E+2", 2500.0)]
    public void Scanner_Recognizes_Real_Numbers(string source, double expected)
    {
        List<Token> tokens = _scanner.ScanTokens(new SourceText(source));

        Assert.AreEqual(TokenType.Number, tokens[0].Type);
        Assert.AreEqual(expected, tokens[0].Literal);
    }

    [TestMethod]
    public void Scanner_Tracks_Positions_Across_Lines()
    {
        List<Token> tokens = _scanner.ScanTokens(
            new SourceText("1 +\r\n  2\n\t* 3", "lines.pas"));

        Assert.AreEqual(
            new SourceSpan("lines.pas", 7, 1, 2, 3),
            tokens[2].Span);
        Assert.AreEqual(
            new SourceSpan("lines.pas", 10, 1, 3, 2),
            tokens[3].Span);
        Assert.AreEqual(3, tokens[^1].Span.Line);
        Assert.AreEqual(5, tokens[^1].Span.Column);
    }

    [TestMethod]
    public void Scanner_Recognizes_Relational_Operators()
    {
        List<Token> tokens = _scanner.ScanTokens(
            new SourceText("a=b <> c <= d < e >= f > g"));

        AssertTokenTypes(
            tokens,
            TokenType.Identifier,
            TokenType.Equal,
            TokenType.Identifier,
            TokenType.NotEqual,
            TokenType.Identifier,
            TokenType.LessThanOrEqual,
            TokenType.Identifier,
            TokenType.LessThan,
            TokenType.Identifier,
            TokenType.GreaterThanOrEqual,
            TokenType.Identifier,
            TokenType.GreaterThan,
            TokenType.Identifier,
            TokenType.EndOfFile);
    }

    [TestMethod]
    [DataRow("'Hello, world!'", "Hello, world!")]
    [DataRow("'isn''t'", "isn't")]
    [DataRow("''", "")]
    public void Scanner_Recognizes_String_Literals(string source, string expected)
    {
        List<Token> tokens = _scanner.ScanTokens(new SourceText(source));

        AssertTokenTypes(tokens, TokenType.String, TokenType.EndOfFile);
        Assert.AreEqual(source, tokens[0].Lexeme);
        Assert.AreEqual(expected, tokens[0].Literal);
    }

    [TestMethod]
    [DataRow("'unterminated")]
    [DataRow("'line\r\nbreak'")]
    public void Scanner_Reports_Unterminated_String_Literals(string source)
    {
        ScanException exception = Assert.ThrowsExactly<ScanException>(
            () => _scanner.ScanTokens(new SourceText(source, "string.pas")));

        Assert.AreEqual("Unterminated string literal.", exception.Message);
        Assert.AreEqual("string.pas", exception.Span.FilePath);
        Assert.AreEqual(1, exception.Span.Column);
    }

    [TestMethod]
    public void Scanner_Reports_Unexpected_Characters()
    {
        ScanException exception = Assert.ThrowsExactly<ScanException>(
            () => _scanner.ScanTokens(new SourceText("1 # 2", "bad.pas")));

        Assert.AreEqual(
            new SourceSpan("bad.pas", 2, 1, 1, 3),
            exception.Span);
    }

    [TestMethod]
    [DataRow("1e+")]
    [DataRow("12abc")]
    [DataRow("999999999999999999999999999999")]
    public void Scanner_Reports_Invalid_Numbers(string source)
    {
        ScanException exception = Assert.ThrowsExactly<ScanException>(
            () => _scanner.ScanTokens(new SourceText(source)));

        Assert.AreEqual(0, exception.Span.Start);
        Assert.AreEqual(source.Length, exception.Span.Length);
    }

    private static void AssertTokenTypes(
        List<Token> tokens,
        params TokenType[] expected)
    {
        Assert.HasCount(expected.Length, tokens);
        CollectionAssert.AreEqual(
            expected,
            tokens.Select(static token => token.Type).ToArray());
    }
}