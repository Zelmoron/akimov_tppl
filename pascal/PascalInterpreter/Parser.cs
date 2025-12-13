namespace PascalInterpreter;

public class Parser
{
    private readonly Lexer _lexer;
    private Token _currentToken;

    public Parser(Lexer lexer)
    {
        _lexer = lexer;
        _currentToken = _lexer.GetNextToken();
    }

    private void Eat(TokenType tokenType)
    {
        if (_currentToken.Type == tokenType)
        {
            _currentToken = _lexer.GetNextToken();
        }
        else
        {
            throw new Exception($"Ошибка парсинга: ожидался {tokenType}, получен {_currentToken.Type}");
        }
    }

    // program ::= complex_statement DOT
    public ASTNode Program()
    {
        var node = CompoundStatement();
        Eat(TokenType.DOT);
        return node;
    }

    // complex_statement ::= BEGIN statement_list END
    private Compound CompoundStatement()
    {
        Eat(TokenType.BEGIN);
        var nodes = StatementList();
        Eat(TokenType.END);

        var root = new Compound();
        foreach (var node in nodes)
        {
            root.Children.Add(node);
        }

        return root;
    }

    // statement_list ::= statement | statement SEMI statement_list
    private List<ASTNode> StatementList()
    {
        var node = Statement();
        var results = new List<ASTNode> { node };

        while (_currentToken.Type == TokenType.SEMI)
        {
            Eat(TokenType.SEMI);
            results.Add(Statement());
        }

        return results;
    }

    // statement ::= compound_statement | assignment | empty
    private ASTNode Statement()
    {
        if (_currentToken.Type == TokenType.BEGIN)
        {
            return CompoundStatement();
        }
        else if (_currentToken.Type == TokenType.ID)
        {
            return AssignmentStatement();
        }
        else
        {
            return Empty();
        }
    }

    // assignment ::= variable ASSIGN expr
    private ASTNode AssignmentStatement()
    {
        var left = Variable();
        var token = _currentToken;
        Eat(TokenType.ASSIGN);
        var right = Expr();
        return new Assign(left, token, right);
    }

    // variable ::= ID
    private Var Variable()
    {
        var node = new Var(_currentToken);
        Eat(TokenType.ID);
        return node;
    }

    // empty ::= ''
    private ASTNode Empty()
    {
        return new NoOp();
    }

    // expr ::= term (('+' | '-') term)*
    private ASTNode Expr()
    {
        var node = Term();

        while (_currentToken.Type == TokenType.PLUS || _currentToken.Type == TokenType.MINUS)
        {
            var token = _currentToken;
            if (token.Type == TokenType.PLUS)
            {
                Eat(TokenType.PLUS);
            }
            else if (token.Type == TokenType.MINUS)
            {
                Eat(TokenType.MINUS);
            }

            node = new BinOp(node, token, Term());
        }

        return node;
    }

    // term ::= factor (('*' | '/') factor)*
    private ASTNode Term()
    {
        var node = Factor();

        while (_currentToken.Type == TokenType.MUL || _currentToken.Type == TokenType.DIV)
        {
            var token = _currentToken;
            if (token.Type == TokenType.MUL)
            {
                Eat(TokenType.MUL);
            }
            else if (token.Type == TokenType.DIV)
            {
                Eat(TokenType.DIV);
            }

            node = new BinOp(node, token, Factor());
        }

        return node;
    }

    // factor ::= ('+' | '-') factor | INTEGER | LPAREN expr RPAREN | variable
    private ASTNode Factor()
    {
        var token = _currentToken;

        if (token.Type == TokenType.PLUS)
        {
            Eat(TokenType.PLUS);
            return new UnaryOp(token, Factor());
        }
        else if (token.Type == TokenType.MINUS)
        {
            Eat(TokenType.MINUS);
            return new UnaryOp(token, Factor());
        }
        else if (token.Type == TokenType.INTEGER)
        {
            Eat(TokenType.INTEGER);
            return new Num(token);
        }
        else if (token.Type == TokenType.LPAREN)
        {
            Eat(TokenType.LPAREN);
            var node = Expr();
            Eat(TokenType.RPAREN);
            return node;
        }
        else
        {
            return Variable();
        }
    }

    public ASTNode Parse()
    {
        var node = Program();
        if (_currentToken.Type != TokenType.EOF)
        {
            throw new Exception("Ошибка: неожиданные символы после конца программы");
        }
        return node;
    }
}
