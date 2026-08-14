namespace Cjb.StandardPascal.Language.Scanner;

public sealed class Token
{
    public Token(
        TokenType type,
        string lexeme,
        object? literal,
        SourceSpan span)
    {
        Type = type;
        Lexeme = lexeme;
        Literal = literal;
        Span = span;
    }

    public string Lexeme { get; }

    public object? Literal { get; }

    public SourceSpan Span { get; }

    public TokenType Type { get; }

    public override string ToString()
    {
        return $"{Type} {Lexeme} {Literal}";
    }
}