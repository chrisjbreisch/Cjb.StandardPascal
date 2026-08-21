using System.Globalization;
using System.Text;

namespace Cjb.StandardPascal.Language.Scanner;

public sealed class Scanner : IScanner
{
    private static readonly IReadOnlyDictionary<string, TokenType> Keywords =
        new Dictionary<string, TokenType>(StringComparer.OrdinalIgnoreCase)
        {
            ["and"] = TokenType.And,
            ["begin"] = TokenType.Begin,
            ["boolean"] = TokenType.Boolean,
            ["case"] = TokenType.Case,
            ["char"] = TokenType.Char,
            ["const"] = TokenType.Const,
            ["div"] = TokenType.Div,
            ["do"] = TokenType.Do,
            ["downto"] = TokenType.DownTo,
            ["else"] = TokenType.Else,
            ["end"] = TokenType.End,
            ["for"] = TokenType.For,
            ["function"] = TokenType.Function,
            ["goto"] = TokenType.Goto,
            ["in"] = TokenType.In,
            ["if"] = TokenType.If,
            ["integer"] = TokenType.Integer,
            ["label"] = TokenType.Label,
            ["mod"] = TokenType.Mod,
            ["not"] = TokenType.Not,
            ["of"] = TokenType.Of,
            ["or"] = TokenType.Or,
            ["print"] = TokenType.Print,
            ["procedure"] = TokenType.Procedure,
            ["program"] = TokenType.Program,
            ["real"] = TokenType.Real,
            ["repeat"] = TokenType.Repeat,
            ["then"] = TokenType.Then,
            ["to"] = TokenType.To,
            ["type"] = TokenType.Type,
            ["until"] = TokenType.Until,
            ["var"] = TokenType.Var,
            ["while"] = TokenType.While,
            ["with"] = TokenType.With,
            ["write"] = TokenType.Write,
            ["writeln"] = TokenType.WriteLn,
        };

    private string _source = string.Empty;
    private string _filePath = string.Empty;
    private List<Token> _tokens = [];
    private int _tokenStart;
    private int _tokenLine;
    private int _tokenColumn;
    private int _currentIndex;
    private int _line;
    private int _column;

    public List<Token> ScanTokens(SourceText source)
    {
        ArgumentNullException.ThrowIfNull(source);
        Initialize(source);

        while (!IsAtEnd())
        {
            _tokenStart = _currentIndex;
            _tokenLine = _line;
            _tokenColumn = _column;
            ScanToken();
        }

        _tokens.Add(new Token(
            TokenType.EndOfFile,
            string.Empty,
            null,
            new SourceSpan(_filePath, _currentIndex, 0, _line, _column)));

        return _tokens;
    }

    private void Initialize(SourceText source)
    {
        _source = source.Text;
        _filePath = source.FilePath;
        _tokens = [];
        _tokenStart = 0;
        _tokenLine = 1;
        _tokenColumn = 1;
        _currentIndex = 0;
        _line = 1;
        _column = 1;
    }

    private void ScanToken()
    {
        char character = Advance();

        if (char.IsWhiteSpace(character))
        {
            return;
        }

        if (char.IsAsciiDigit(character))
        {
            GetNumber();
            return;
        }

        if (char.IsAsciiLetter(character))
        {
            GetKeywordOrIdentifier();
            return;
        }

        switch (character)
        {
            case '^':
                AddToken(TokenType.Caret);
                break;
            case ':':
                AddToken(Match('=') ? TokenType.Assign : TokenType.Colon);
                break;
            case ',':
                AddToken(TokenType.Comma);
                break;
            case '.':
                AddToken(TokenType.Dot);
                break;
            case '(':
                AddToken(TokenType.LeftParen);
                break;
            case '[':
                AddToken(TokenType.LeftBracket);
                break;
            case ')':
                AddToken(TokenType.RightParen);
                break;
            case ']':
                AddToken(TokenType.RightBracket);
                break;
            case ';':
                AddToken(TokenType.Semicolon);
                break;
            case '-':
                AddToken(TokenType.Minus);
                break;
            case '+':
                AddToken(TokenType.Plus);
                break;
            case '*':
                AddToken(TokenType.Star);
                break;
            case '\'':
                GetString();
                break;
            case '/':
                AddToken(TokenType.Slash);
                break;
            case '=':
                AddToken(TokenType.Equal);
                break;
            case '<':
                AddToken(
                    Match('>') ? TokenType.NotEqual
                        : Match('=') ? TokenType.LessThanOrEqual
                        : TokenType.LessThan);
                break;
            case '>':
                AddToken(Match('=') ? TokenType.GreaterThanOrEqual : TokenType.GreaterThan);
                break;
            default:
                throw Error($"Unexpected character '{character}'.");
        }
    }

    private void GetKeywordOrIdentifier()
    {
        while (char.IsAsciiLetterOrDigit(Peek()))
        {
            Advance();
        }

        string lexeme = CurrentLexeme();
        TokenType type = Keywords.TryGetValue(lexeme, out TokenType keyword)
            ? keyword
            : TokenType.Identifier;
        AddToken(type);
    }

    private void GetNumber()
    {
        while (char.IsAsciiDigit(Peek()))
        {
            Advance();
        }

        bool isInteger = true;

        if (Peek() == '.' && char.IsAsciiDigit(PeekNext()))
        {
            isInteger = false;
            Advance();

            while (char.IsAsciiDigit(Peek()))
            {
                Advance();
            }
        }

        if (Peek() is 'e' or 'E')
        {
            isInteger = false;
            Advance();

            if (Peek() is '+' or '-')
            {
                Advance();
            }

            if (!char.IsAsciiDigit(Peek()))
            {
                throw Error("Expected at least one digit in the exponent.");
            }

            while (char.IsAsciiDigit(Peek()))
            {
                Advance();
            }
        }

        if (char.IsAsciiLetter(Peek()))
        {
            while (char.IsAsciiLetterOrDigit(Peek()))
            {
                Advance();
            }

            throw Error("A number and identifier must be separated.");
        }

        string lexeme = CurrentLexeme();
        object literal;

        if (isInteger)
        {
            if (!long.TryParse(
                    lexeme,
                    NumberStyles.None,
                    CultureInfo.InvariantCulture,
                    out long integer))
            {
                throw Error($"Integer literal '{lexeme}' is out of range.");
            }

            literal = integer;
        }
        else
        {
            if (!double.TryParse(
                    lexeme,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double real)
                || !double.IsFinite(real))
            {
                throw Error($"Real literal '{lexeme}' is out of range.");
            }

            literal = real;
        }

        AddToken(TokenType.Number, literal);
    }

    private void GetString()
    {
        StringBuilder value = new();

        while (!IsAtEnd())
        {
            char character = Peek();

            if (character is '\r' or '\n')
            {
                throw Error("Unterminated string literal.");
            }

            Advance();

            if (character != '\'')
            {
                value.Append(character);
                continue;
            }

            if (Peek() == '\'')
            {
                Advance();
                value.Append('\'');
                continue;
            }

            AddToken(TokenType.String, value.ToString());
            return;
        }

        throw Error("Unterminated string literal.");
    }

    private ScanException Error(string message)
    {
        return new ScanException(message, CurrentSpan());
    }

    private void AddToken(TokenType type, object? literal = null)
    {
        _tokens.Add(new Token(type, CurrentLexeme(), literal, CurrentSpan()));
    }

    private SourceSpan CurrentSpan()
    {
        return new SourceSpan(
            _filePath,
            _tokenStart,
            _currentIndex - _tokenStart,
            _tokenLine,
            _tokenColumn);
    }

    private string CurrentLexeme()
    {
        return _source[_tokenStart.._currentIndex];
    }

    private bool Match(char expected)
    {
        if (Peek() != expected)
        {
            return false;
        }

        Advance();
        return true;
    }

    private char Peek()
    {
        return IsAtEnd() ? '\0' : _source[_currentIndex];
    }

    private char PeekNext()
    {
        return _currentIndex + 1 >= _source.Length ? '\0' : _source[_currentIndex + 1];
    }

    private char Advance()
    {
        char character = _source[_currentIndex++];

        if (character == '\r')
        {
            _line++;
            _column = 1;
        }
        else if (character == '\n')
        {
            if (_currentIndex < 2 || _source[_currentIndex - 2] != '\r')
            {
                _line++;
            }

            _column = 1;
        }
        else
        {
            _column++;
        }

        return character;
    }

    private bool IsAtEnd()
    {
        return _currentIndex >= _source.Length;
    }
}