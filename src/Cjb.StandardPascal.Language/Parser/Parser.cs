using Cjb.StandardPascal.Language.Parser.Expressions;
using Cjb.StandardPascal.Language.Parser.Statements;
using Cjb.StandardPascal.Language.Scanner;

namespace Cjb.StandardPascal.Language.Parser;

public sealed class Parser : IParser
{
    private List<Token> _tokens = [];
    private int _current;

    public Expression Parse(List<Token> tokens)
    {
        Initialize(tokens);

        Expression expression = Expression();
        Consume(TokenType.EndOfFile, "Expected the end of the expression.");
        return expression;
    }

    public IStatement ParseStatement(List<Token> tokens)
    {
        Initialize(tokens);

        IStatement statement = Statement();
        Consume(TokenType.EndOfFile, "Expected the end of the statement.");
        return statement;
    }

    public Program ParseProgram(List<Token> tokens)
    {
        Initialize(tokens);

        IStatement body = Statement();
        Consume(TokenType.EndOfFile, "Expected the end of the program.");
        return new Program(body, body.Span);
    }

    private IStatement Statement()
    {
        Token print = Consume(TokenType.Print, "Expected 'Print'.");
        Expression expression = Expression();
        Token semicolon = Consume(
            TokenType.Semicolon,
            "Expected ';' after the Print expression.");

        SourceSpan span = new(
            print.Span.FilePath,
            print.Span.Start,
            semicolon.Span.Start + semicolon.Span.Length - print.Span.Start,
            print.Span.Line,
            print.Span.Column);
        return new Print(expression, span);
    }

    private void Initialize(List<Token> tokens)
    {
        ArgumentNullException.ThrowIfNull(tokens);

        if (tokens.Count == 0 || tokens[^1].Type != TokenType.EndOfFile)
        {
            throw new ArgumentException(
                "The token list must end with an EndOfFile token.",
                nameof(tokens));
        }

        _tokens = tokens;
        _current = 0;
    }

    private Expression Expression()
    {
        Expression left = SimpleExpression();

        if (!Match(
                TokenType.Equal,
                TokenType.NotEqual,
                TokenType.LessThan,
                TokenType.LessThanOrEqual,
                TokenType.GreaterThan,
                TokenType.GreaterThanOrEqual,
                TokenType.In))
        {
            return left;
        }

        Token binaryOperator = Previous();
        Expression right = SimpleExpression();
        return new Binary(left, binaryOperator, right);
    }

    private Expression SimpleExpression()
    {
        Token? sign = Match(TokenType.Plus, TokenType.Minus) ? Previous() : null;
        Expression expression = Term();

        if (sign is not null)
        {
            expression = new Unary(sign, expression);
        }

        while (Match(TokenType.Plus, TokenType.Minus, TokenType.Or))
        {
            Token binaryOperator = Previous();
            Expression right = Term();
            expression = new Binary(expression, binaryOperator, right);
        }

        return expression;
    }

    private Expression Term()
    {
        Expression expression = Factor();

        while (Match(
                   TokenType.Star,
                   TokenType.Slash,
                   TokenType.Div,
                   TokenType.Mod,
                   TokenType.And))
        {
            Token binaryOperator = Previous();
            Expression right = Factor();
            expression = new Binary(expression, binaryOperator, right);
        }

        return expression;
    }

    private Expression Factor()
    {
        if (Match(TokenType.Not))
        {
            Token unaryOperator = Previous();
            return new Unary(unaryOperator, Factor());
        }

        return Primary();
    }

    private Expression Primary()
    {
        if (Match(TokenType.Number, TokenType.String))
        {
            return new Literal(Previous());
        }

        if (Match(TokenType.Identifier))
        {
            return new Identifier(Previous());
        }

        if (Match(TokenType.LeftParen))
        {
            Token leftParenthesis = Previous();
            Expression expression = Expression();
            Token rightParenthesis = Consume(
                TokenType.RightParen,
                "Expected ')' after expression.");
            SourceSpan span = new(
                leftParenthesis.Span.FilePath,
                leftParenthesis.Span.Start,
                rightParenthesis.Span.Start
                    + rightParenthesis.Span.Length
                    - leftParenthesis.Span.Start,
                leftParenthesis.Span.Line,
                leftParenthesis.Span.Column);
            return new Grouping(expression, span);
        }

        throw Error(Peek(), "Expected an expression.");
    }

    private Token Consume(TokenType type, string message)
    {
        if (Check(type))
        {
            return Advance();
        }

        throw Error(Peek(), message);
    }

    private bool Match(params TokenType[] types)
    {
        if (!types.Any(Check))
        {
            return false;
        }

        Advance();
        return true;
    }

    private bool Check(TokenType type)
    {
        return Peek().Type == type;
    }

    private Token Advance()
    {
        Token current = Peek();

        if (current.Type != TokenType.EndOfFile)
        {
            _current++;
        }

        return current;
    }

    private Token Peek()
    {
        return _tokens[_current];
    }

    private Token Previous()
    {
        return _tokens[_current - 1];
    }

    private static ParseException Error(Token token, string message)
    {
        return new ParseException(message, token.Span);
    }
}