namespace PascalInterpreter;

public class Interpreter
{
    private readonly ASTNode _tree;
    private readonly Dictionary<string, double> _globalScope;

    public Interpreter(ASTNode tree)
    {
        _tree = tree;
        _globalScope = new Dictionary<string, double>();
    }

    public Dictionary<string, double> GetVariables()
    {
        return new Dictionary<string, double>(_globalScope);
    }

    private double Visit(ASTNode node)
    {
        return node switch
        {
            BinOp binOp => VisitBinOp(binOp),
            Num num => VisitNum(num),
            UnaryOp unaryOp => VisitUnaryOp(unaryOp),
            Compound compound => VisitCompound(compound),
            Assign assign => VisitAssign(assign),
            Var var => VisitVar(var),
            NoOp => VisitNoOp(),
            _ => throw new Exception($"Неизвестный тип узла: {node.GetType()}")
        };
    }

    private double VisitBinOp(BinOp node)
    {
        return node.Op.Type switch
        {
            TokenType.PLUS => Visit(node.Left) + Visit(node.Right),
            TokenType.MINUS => Visit(node.Left) - Visit(node.Right),
            TokenType.MUL => Visit(node.Left) * Visit(node.Right),
            TokenType.DIV => Visit(node.Left) / Visit(node.Right),
            _ => throw new Exception($"Неизвестная бинарная операция: {node.Op.Type}")
        };
    }

    private double VisitNum(Num node)
    {
        return node.Value;
    }

    private double VisitUnaryOp(UnaryOp node)
    {
        return node.Op.Type switch
        {
            TokenType.PLUS => +Visit(node.Expr),
            TokenType.MINUS => -Visit(node.Expr),
            _ => throw new Exception($"Неизвестная унарная операция: {node.Op.Type}")
        };
    }

    private double VisitCompound(Compound node)
    {
        foreach (var child in node.Children)
        {
            Visit(child);
        }
        return 0;
    }

    private double VisitAssign(Assign node)
    {
        var varName = node.Left.Value;
        var value = Visit(node.Right);
        _globalScope[varName] = value;
        return value;
    }

    private double VisitVar(Var node)
    {
        var varName = node.Value;
        if (_globalScope.TryGetValue(varName, out var value))
        {
            return value;
        }
        throw new Exception($"Переменная {varName} не определена");
    }

    private double VisitNoOp()
    {
        return 0;
    }

    public void Interpret()
    {
        Visit(_tree);
    }
}
