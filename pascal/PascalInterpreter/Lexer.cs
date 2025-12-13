namespace PascalInterpreter;

public class Lexer
{
    private readonly string _text;
    private int _pos;
    private char? _currentChar;

    private static readonly Dictionary<string, Token> ReservedKeywords = new()
    {
        { "BEGIN", new Token(TokenType.BEGIN, "BEGIN") },
        { "END", new Token(TokenType.END, "END") }
    };

    public Lexer(string text)
    {
        _text = text;
        _pos = 0;
        _currentChar = _text.Length > 0 ? _text[0] : null;
    }

    private void Advance()
    {
        _pos++;
        _currentChar = _pos < _text.Length ? _text[_pos] : null;
    }

    private void SkipWhitespace()
    {
        while (_currentChar != null && char.IsWhiteSpace(_currentChar.Value))
        {
            Advance();
        }
    }

    private int Integer()
    {
        var result = "";
        while (_currentChar != null && char.IsDigit(_currentChar.Value))
        {
            result += _currentChar;
            Advance();
        }
        return int.Parse(result);
    }

    private Token Id()
    {
        var result = "";
        while (_currentChar != null && (char.IsLetterOrDigit(_currentChar.Value) || _currentChar == '_'))
        {
            result += _currentChar;
            Advance();
        }

        var upperResult = result.ToUpper();
        return ReservedKeywords.TryGetValue(upperResult, out var token) 
            ? token 
            : new Token(TokenType.ID, result);
    }

    private char Peek()
    {
        var peekPos = _pos + 1;
        return peekPos < _text.Length ? _text[peekPos] : '\0';
    }

    public Token GetNextToken()
    {
        while (_currentChar != null)
        {
            if (char.IsWhiteSpace(_currentChar.Value))
            {
                SkipWhitespace();
                continue;
            }

            if (char.IsDigit(_currentChar.Value))
            {
                return new Token(TokenType.INTEGER, Integer());
            }

            if (char.IsLetter(_currentChar.Value) || _currentChar == '_')
            {
                return Id();
            }

            if (_currentChar == ':' && Peek() == '=')
            {
                Advance();
                Advance();
                return new Token(TokenType.ASSIGN, ":=");
            }

            if (_currentChar == ';')
            {
                Advance();
                return new Token(TokenType.SEMI, ";");
            }

            if (_currentChar == '+')
            {
                Advance();
                return new Token(TokenType.PLUS, "+");
            }

            if (_currentChar == '-')
            {
                Advance();
                return new Token(TokenType.MINUS, "-");
            }

            if (_currentChar == '*')
            {
                Advance();
                return new Token(TokenType.MUL, "*");
            }

            if (_currentChar == '/')
            {
                Advance();
                return new Token(TokenType.DIV, "/");
            }

            if (_currentChar == '(')
            {
                Advance();
                return new Token(TokenType.LPAREN, "(");
            }

            if (_currentChar == ')')
            {
                Advance();
                return new Token(TokenType.RPAREN, ")");
            }

            if (_currentChar == '.')
            {
                Advance();
                return new Token(TokenType.DOT, ".");
            }

            throw new Exception($"Неизвестный символ: {_currentChar}");
        }

        return new Token(TokenType.EOF, null);
    }
}
