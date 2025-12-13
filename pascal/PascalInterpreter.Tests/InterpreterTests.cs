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
}
