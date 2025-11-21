using System.IO;
using Xunit;

namespace CowInterpreter.Tests
{
    public class InterpreterTests
    {
        private static void RunInterpreter(string code, string input = "", string expectedOutput = null)
        {
            var inputReader = new StringReader(input);
            var outputWriter = new StringWriter();
            var interpreter = new Interpreter(code, inputReader, outputWriter);
            interpreter.Run();

            if (expectedOutput != null)
            {
                Assert.Equal(expectedOutput, outputWriter.ToString());
            }
        }

        [Fact]
        public void MoO_ShouldIncrementMemory()
        {
            RunInterpreter("MoO MoO OOM", expectedOutput: "2");
        }

        [Fact]
        public void MOo_ShouldDecrementMemory()
        {
            RunInterpreter("MoO MoO MOo OOM", expectedOutput: "1");
        }

        [Fact]
        public void moO_ShouldMoveToNextMemoryCellAndExpand()
        {
            RunInterpreter("moO MoO OOM", expectedOutput: "1");
        }

        [Fact]
        public void mOo_ShouldMoveToPreviousMemoryCell()
        {
            RunInterpreter("moO MoO mOo OOM", expectedOutput: "0");
        }

        [Fact]
        public void mOo_ShouldNotGoBelowZero()
        {
            RunInterpreter("mOo MoO OOM", expectedOutput: "1");
        }

        [Fact]
        public void OOM_ShouldOutputMemoryValue()
        {
            RunInterpreter("MoO MoO MoO OOM", expectedOutput: "3");
        }

        [Fact]
        public void oom_ShouldInputToMemory()
        {
            RunInterpreter("oom OOM", "123", "123");
        }

        [Fact]
        public void OOO_ShouldZeroMemory()
        {
            RunInterpreter("MoO MoO MoO OOO OOM", expectedOutput: "0");
        }

        [Fact]
        public void MMM_ShouldStoreAndRetrieveFromRegister()
        {
            RunInterpreter("MoO MoO MoO MMM moO MMM OOM", expectedOutput: "3");
        }

        [Fact]
        public void MMM_ShouldClearRegisterAfterPasting()
        {
            RunInterpreter("MoO MoO MMM moO MMM moO MoO MMM OOM", expectedOutput: "1");
        }

        [Fact]
        public void Moo_ShouldPrintCharWhenNotZero()
        {
            RunInterpreter("MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO Moo", expectedOutput: "A");
        }

        [Fact]
        public void Moo_ShouldReadCharWhenZero()
        {
            RunInterpreter("Moo OOM", "B", "66");
        }

        [Fact]
        public void SimpleLoop_ShouldExecuteCorrectly()
        {
            RunInterpreter("MoO MoO MoO MOO MOo moo OOM", expectedOutput: "0");
        }

        [Fact]
        public void NestedLoop_ShouldExecuteCorrectly()
        {
            RunInterpreter("MoO MoO MOO MOo moO MoO MOO MOo moo OOM moo mOo OOM", expectedOutput: "01");
        }

        [Fact]
        public void MOO_ShouldSkipLoopWhenZero()
        {
            RunInterpreter("MOO MOo OOM moo", expectedOutput: "0");
        }

        [Fact]
        public void mOO_ShouldExecuteInstructionFromMemory()
        {
            RunInterpreter("MoO MoO MoO MoO MoO MoO MoO mOO", expectedOutput: "A");
        }

        [Fact]
        public void HelloWorld_ShouldPrintHelloWorld()
        {
            var path = Path.Combine("..", "..", "..", "..", "hello.cow");
            var code = File.ReadAllText(path);
            RunInterpreter(code, expectedOutput: "Hello, World!");
        }
    }
}