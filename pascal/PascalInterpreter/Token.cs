namespace PascalInterpreter;

public enum TokenType
{
    INTEGER,      // число
    PLUS,         // +
    MINUS,        // -
    MUL,          // *
    DIV,          // /
    LPAREN,       // (
    RPAREN,       // )
    BEGIN,        // BEGIN
    END,          // END
    DOT,          // .
    ASSIGN,       // :=
    SEMI,         // ;
    ID,           // идентификатор
    EOF           // конец файла
}

public class Token
{
    public TokenType Type { get; }
    public object? Value { get; }

    public Token(TokenType type, object? value)
    {
        Type = type;
        Value = value;
    }

    public override string ToString()
    {
        return $"Token({Type}, {Value})";
    }
}
