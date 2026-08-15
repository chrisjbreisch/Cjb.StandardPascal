namespace Cjb.StandardPascal.Language.Scanner;

public enum TokenType
{
    // Single-character tokens
    LeftParen,
    Minus,
    Plus,
    RightParen,
    Semicolon,
    Slash,
    Star,

    // One- or two-character tokens
    Equal,
    GreaterThan,
    GreaterThanOrEqual,
    LessThan,
    LessThanOrEqual,
    NotEqual,

    // Literals
    Identifier,
    Number,

    // Expression keywords
    And,
    Div,
    In,
    Mod,
    Not,
    Or,
    Print,

    EndOfFile,
}