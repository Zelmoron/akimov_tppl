using System;
using System.IO;

namespace CowInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide a path to a .cow file.");
                return;
            }

            var filePath = args[0];
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }

            var code = File.ReadAllText(filePath);
            var interpreter = new Interpreter(code, Console.In, Console.Out);
            interpreter.Run();
        }
    }
}

