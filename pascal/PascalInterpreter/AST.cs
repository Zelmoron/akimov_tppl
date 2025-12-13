namespace PascalInterpreter;

// Базовый класс для всех узлов AST
public abstract class ASTNode
{
}

// Узел для чисел
public class Num : ASTNode
{
    public Token Token { get; }
    public int Value { get; }

    public Num(Token token)
    {
        Token = token;
        Value = (int)token.Value!;
    }
}

// Узел для бинарных операций
public class BinOp : ASTNode
{
    public ASTNode Left { get; }
    public Token Op { get; }
    public ASTNode Right { get; }

    public BinOp(ASTNode left, Token op, ASTNode right)
    {
        Left = left;
        Op = op;
        Right = right;
    }
}

// Узел для унарных операций
public class UnaryOp : ASTNode
{
    public Token Op { get; }
    public ASTNode Expr { get; }

    public UnaryOp(Token op, ASTNode expr)
    {
        Op = op;
        Expr = expr;
    }
}

// Узел для составных операторов (BEGIN...END)
public class Compound : ASTNode
{
    public List<ASTNode> Children { get; }

    public Compound()
    {
        Children = new List<ASTNode>();
    }
}

// Узел для присваивания
public class Assign : ASTNode
{
    public Var Left { get; }
    public Token Op { get; }
    public ASTNode Right { get; }

    public Assign(Var left, Token op, ASTNode right)
    {
        Left = left;
        Op = op;
        Right = right;
    }
}

// Узел для переменных
public class Var : ASTNode
{
    public Token Token { get; }
    public string Value { get; }

    public Var(Token token)
    {
        Token = token;
        Value = (string)token.Value!;
    }
}

// Пустой узел
public class NoOp : ASTNode
{
}
