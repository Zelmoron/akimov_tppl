using PascalInterpreter;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Pascal Interpreter\n");
        Console.WriteLine("Выберите режим:");
        Console.WriteLine("1 - Запустить встроенные тесты");
        Console.WriteLine("2 - Запустить код из файла");
        Console.Write("\nВыбор: ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                RunBuiltInTests();
                break;
            case "2":
                RunFromFile();
                break;
            default:
                Console.WriteLine("Неверный выбор");
                break;
        }
    }

    static void RunBuiltInTests()
    {
        Console.WriteLine("\nТест 1: Пустая программа");
        var code1 = @"
BEGIN
END.
";
        RunProgram(code1);

        Console.WriteLine("\nТест 2: Арифметические выражения");
        var code2 = @"
BEGIN
    x:= 2 + 3 * (2 + 3);
    y:= 2 / 2 - 2 + 3 * ((1 + 1) + (1 + 1))
END.
";
        RunProgram(code2);

        Console.WriteLine("\nТест 3: Вложенные блоки");
        var code3 = @"
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
        RunProgram(code3);
    }

    static void RunFromFile()
    {
        Console.Write("\nВведите путь к файлу: ");
        var filePath = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(filePath))
        {
            Console.WriteLine("Путь к файлу не указан");
            return;
        }

        try
        {
            var code = File.ReadAllText(filePath);
            Console.WriteLine();
            RunProgram(code);
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Файл не найден: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка чтения файла: {ex.Message}");
        }
    }

    static void RunProgram(string code)
    {
        try
        {
            var lexer = new Lexer(code);
            var parser = new Parser(lexer);
            var tree = parser.Parse();
            var interpreter = new Interpreter(tree);
            
            interpreter.Interpret();
            
            var variables = interpreter.GetVariables();
            
            if (variables.Count == 0)
            {
                Console.WriteLine("Переменные: (нет переменных)");
            }
            else
            {
                Console.WriteLine("Переменные:");
                foreach (var (name, value) in variables.OrderBy(kvp => kvp.Key))
                {
                    Console.WriteLine($"  {name} = {value}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}
