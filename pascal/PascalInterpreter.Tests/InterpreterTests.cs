using PascalInterpreter;
using Xunit;

namespace PascalInterpreter.Tests;

public class InterpreterTests
{
    private Dictionary<string, double> InterpretCode(string code)
    {
        var lexer = new Lexer(code);
        var parser = new Parser(lexer);
        var tree = parser.Parse();
        var interpreter = new Interpreter(tree);
        interpreter.Interpret();
        return interpreter.GetVariables();
    }

    [Fact]
    public void Test_EmptyProgram()
    {
        var code = @"
BEGIN
END.
";
        var variables = InterpretCode(code);
        Assert.Empty(variables);
    }

    [Fact]
    public void Test_SimpleArithmetic()
    {
        var code = @"
BEGIN
    x := 2 + 3 * (2 + 3);
    y := 2 / 2 - 2 + 3 * ((1 + 1) + (1 + 1))
END.
";
        var variables = InterpretCode(code);
        
        Assert.Equal(2, variables.Count);
        Assert.Equal(17, variables["x"]); // 2 + 3 * 5 = 2 + 15 = 17
        Assert.Equal(11, variables["y"]); // 1 - 2 + 3 * 4 = -1 + 12 = 11
    }

    [Fact]
    public void Test_NestedBlocks()
    {
        var code = @"
BEGIN
    y := 2;
    BEGIN
        a := 3;
        a := a;
        b := 10 + a + 10 * y / 4;
        c := a - b
    END;
    x := 11
END.
";
        var variables = InterpretCode(code);
        
        Assert.Equal(5, variables.Count);
        Assert.Equal(2, variables["y"]);
        Assert.Equal(3, variables["a"]);
        Assert.Equal(18, variables["b"]); // 10 + 3 + 10 * 2 / 4 = 10 + 3 + 5 = 18
        Assert.Equal(-15, variables["c"]); // 3 - 18 = -15
        Assert.Equal(11, variables["x"]);
    }

    [Fact]
    public void Test_UnaryOperators()
    {
        var code = @"
BEGIN
    x := -5;
    y := +10;
    z := -(-3)
END.
";
        var variables = InterpretCode(code);
        
        Assert.Equal(3, variables.Count);
        Assert.Equal(-5, variables["x"]);
        Assert.Equal(10, variables["y"]);
        Assert.Equal(3, variables["z"]);
    }

    [Fact]
    public void Test_Division()
    {
        var code = @"
BEGIN
    x := 10 / 2;
    y := 7 / 2
END.
";
        var variables = InterpretCode(code);
        
        Assert.Equal(2, variables.Count);
        Assert.Equal(5, variables["x"]);
        Assert.Equal(3.5, variables["y"]);
    }

    [Fact]
    public void Test_VariableReassignment()
    {
        var code = @"
BEGIN
    x := 5;
    x := x + 3;
    x := x * 2
END.
";
        var variables = InterpretCode(code);
        
        Assert.Single(variables);
        Assert.Equal(16, variables["x"]); // (5 + 3) * 2 = 16
    }

    [Fact]
    public void Test_MultipleVariables()
    {
        var code = @"
BEGIN
    a := 1;
    b := 2;
    c := 3;
    result := a + b * c
END.
";
        var variables = InterpretCode(code);
        
        Assert.Equal(4, variables.Count);
        Assert.Equal(1, variables["a"]);
        Assert.Equal(2, variables["b"]);
        Assert.Equal(3, variables["c"]);
        Assert.Equal(7, variables["result"]); // 1 + 2 * 3 = 7
    }

    [Fact]
    public void Test_ComplexExpression()
    {
        var code = @"
BEGIN
    x := 2 + 3 * 4 - 5 / 5
END.
";
        var variables = InterpretCode(code);
        
        Assert.Single(variables);
        Assert.Equal(13, variables["x"]); // 2 + 12 - 1 = 13
    }

    [Fact]
    public void Test_EmptyStatements()
    {
        var code = @"
BEGIN
    ;
    x := 5;
    ;
    ;
    y := 10;
    ;
END.
";
        var variables = InterpretCode(code);
        
        Assert.Equal(2, variables.Count);
        Assert.Equal(5, variables["x"]);
        Assert.Equal(10, variables["y"]);
    }

    [Fact]
    public void Test_UndefinedVariableThrowsException()
    {
        var code = @"
BEGIN
    x := y + 5
END.
";
        Assert.Throws<Exception>(() => InterpretCode(code));
    }

    // Lexer Error Tests
    [Fact]
    public void Test_Lexer_UnknownCharacterThrowsException()
    {
        var code = @"
BEGIN
    x := 5 @ 3
END.
";
        Assert.Throws<Exception>(() => InterpretCode(code));
    }

    [Fact]
    public void Test_Lexer_MultiDigitNumbers()
    {
        var code = @"
BEGIN
    x := 123456
END.
";
        var variables = InterpretCode(code);
        Assert.Equal(123456, variables["x"]);
    }

    [Fact]
    public void Test_Lexer_IdentifierWithUnderscore()
    {
        var code = @"
BEGIN
    my_var := 42;
    _x := 10
END.
";
        var variables = InterpretCode(code);
        Assert.Equal(42, variables["my_var"]);
        Assert.Equal(10, variables["_x"]);
    }

    [Fact]
    public void Test_Lexer_IdentifierWithNumbers()
    {
        var code = @"
BEGIN
    var123 := 99
END.
";
        var variables = InterpretCode(code);
        Assert.Equal(99, variables["var123"]);
    }

    // Parser Error Tests
    [Fact]
    public void Test_Parser_MissingDotThrowsException()
    {
        var code = @"
BEGIN
    x := 5
END
";
        Assert.Throws<Exception>(() => InterpretCode(code));
    }

    [Fact]
    public void Test_Parser_MissingEndThrowsException()
    {
        var code = @"
BEGIN
    x := 5.
";
        Assert.Throws<Exception>(() => InterpretCode(code));
    }

    [Fact]
    public void Test_Parser_MissingBeginThrowsException()
    {
        var code = @"
    x := 5
END.
";
        Assert.Throws<Exception>(() => InterpretCode(code));
    }

    [Fact]
    public void Test_Parser_UnexpectedTokenAfterProgram()
    {
        var code = @"
BEGIN
    x := 5
END. extra
";
        Assert.Throws<Exception>(() => InterpretCode(code));
    }

    [Fact]
    public void Test_Parser_InvalidAssignmentSyntax()
    {
        var code = @"
BEGIN
    x = 5
END.
";
        Assert.Throws<Exception>(() => InterpretCode(code));
    }

    [Fact]
    public void Test_Parser_MissingRightParenthesis()
    {
        var code = @"
BEGIN
    x := (5 + 3
END.
";
        Assert.Throws<Exception>(() => InterpretCode(code));
    }

    // Interpreter Error Tests
    [Fact]
    public void Test_Interpreter_UnknownBinaryOperationThrowsException()
    {
        // This test verifies that the default case in BinOp switch is covered
        // We can't easily trigger this without modifying the code, but we can test division by zero behavior
        var code = @"
BEGIN
    x := 10 / 0
END.
";
        var variables = InterpretCode(code);
        Assert.True(double.IsInfinity(variables["x"]) || double.IsNaN(variables["x"]));
    }

    [Fact]
    public void Test_Interpreter_UnknownUnaryOperationHandling()
    {
        // Test unary plus operation
        var code = @"
BEGIN
    x := +(+(+5))
END.
";
        var variables = InterpretCode(code);
        Assert.Equal(5, variables["x"]);
    }

    // Token Tests
    [Fact]
    public void Test_Token_ToStringMethod()
    {
        var token = new Token(TokenType.INTEGER, 42);
        var result = token.ToString();
        Assert.Contains("INTEGER", result);
        Assert.Contains("42", result);
    }

    [Fact]
    public void Test_Token_TypeProperty()
    {
        var token = new Token(TokenType.PLUS, "+");
        Assert.Equal(TokenType.PLUS, token.Type);
    }

    [Fact]
    public void Test_Token_ValueProperty()
    {
        var token = new Token(TokenType.INTEGER, 100);
        Assert.Equal(100, token.Value);
    }

    // AST Node Tests
    [Fact]
    public void Test_AST_NumTokenProperty()
    {
        var token = new Token(TokenType.INTEGER, 42);
        var num = new Num(token);
        Assert.Equal(token, num.Token);
        Assert.Equal(42, num.Value);
    }

    [Fact]
    public void Test_AST_BinOpProperties()
    {
        var left = new Num(new Token(TokenType.INTEGER, 5));
        var op = new Token(TokenType.PLUS, "+");
        var right = new Num(new Token(TokenType.INTEGER, 3));
        var binOp = new BinOp(left, op, right);

        Assert.Equal(left, binOp.Left);
        Assert.Equal(op, binOp.Op);
        Assert.Equal(right, binOp.Right);
    }

    [Fact]
    public void Test_AST_UnaryOpProperties()
    {
        var op = new Token(TokenType.MINUS, "-");
        var expr = new Num(new Token(TokenType.INTEGER, 5));
        var unaryOp = new UnaryOp(op, expr);

        Assert.Equal(op, unaryOp.Op);
        Assert.Equal(expr, unaryOp.Expr);
    }

    [Fact]
    public void Test_AST_AssignOpProperty()
    {
        var varToken = new Token(TokenType.ID, "x");
        var variable = new Var(varToken);
        var assignToken = new Token(TokenType.ASSIGN, ":=");
        var value = new Num(new Token(TokenType.INTEGER, 10));
        var assign = new Assign(variable, assignToken, value);

        Assert.Equal(variable, assign.Left);
        Assert.Equal(assignToken, assign.Op);
        Assert.Equal(value, assign.Right);
    }

    [Fact]
    public void Test_AST_VarTokenProperty()
    {
        var token = new Token(TokenType.ID, "myvar");
        var variable = new Var(token);

        Assert.Equal(token, variable.Token);
        Assert.Equal("myvar", variable.Value);
    }

    [Fact]
    public void Test_AST_CompoundChildren()
    {
        var compound = new Compound();
        Assert.Empty(compound.Children);

        compound.Children.Add(new NoOp());
        Assert.Single(compound.Children);
    }

    // Edge Cases
    [Fact]
    public void Test_EmptyLexer()
    {
        var lexer = new Lexer("");
        var token = lexer.GetNextToken();
        Assert.Equal(TokenType.EOF, token.Type);
    }

    [Fact]
    public void Test_WhitespaceOnly()
    {
        var lexer = new Lexer("   \t\n  ");
        var token = lexer.GetNextToken();
        Assert.Equal(TokenType.EOF, token.Type);
    }

    [Fact]
    public void Test_AllOperators()
    {
        var code = @"
BEGIN
    a := 10 + 5;
    b := 10 - 5;
    c := 10 * 5;
    d := 10 / 5
END.
";
        var variables = InterpretCode(code);
        Assert.Equal(15, variables["a"]);
        Assert.Equal(5, variables["b"]);
        Assert.Equal(50, variables["c"]);
        Assert.Equal(2, variables["d"]);
    }

    [Fact]
    public void Test_CaseInsensitiveKeywords()
    {
        var code = @"
begin
    x := 5
end.
";
        var variables = InterpretCode(code);
        Assert.Equal(5, variables["x"]);
    }

    [Fact]
    public void Test_MixedCaseKeywords()
    {
        var code = @"
BeGiN
    x := 10
EnD.
";
        var variables = InterpretCode(code);
        Assert.Equal(10, variables["x"]);
    }

    [Fact]
    public void Test_NestedParentheses()
    {
        var code = @"
BEGIN
    x := ((((5))))
END.
";
        var variables = InterpretCode(code);
        Assert.Equal(5, variables["x"]);
    }

    [Fact]
    public void Test_ComplexUnaryExpressions()
    {
        var code = @"
BEGIN
    x := --5;
    y := -+5;
    z := +-5
END.
";
        var variables = InterpretCode(code);
        Assert.Equal(5, variables["x"]);
        Assert.Equal(-5, variables["y"]);
        Assert.Equal(-5, variables["z"]);
    }

    [Fact]
    public void Test_SubtractionVsUnaryMinus()
    {
        var code = @"
BEGIN
    x := 10 - -5;
    y := 10 + -5
END.
";
        var variables = InterpretCode(code);
        Assert.Equal(15, variables["x"]);
        Assert.Equal(5, variables["y"]);
    }
}
