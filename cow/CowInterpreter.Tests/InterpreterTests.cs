using System.IO;
using Xunit;

namespace CowInterpreter.Tests
{
    public class InterpreterTests
    {
        private static string RunInterpreter(string code, string input = "")
        {
            var inputReader = new StringReader(input);
            var outputWriter = new StringWriter();
            var interpreter = new Interpreter(code, inputReader, outputWriter);
            interpreter.Run();
            return outputWriter.ToString();
        }

        [Fact]
        public void MoO_ShouldIncrementMemory()
        {
            var output = RunInterpreter("MoO MoO OOM");
            Assert.Equal("2", output);
        }

        [Fact]
        public void MOo_ShouldDecrementMemory()
        {
            var output = RunInterpreter("MoO MoO MoO MOo OOM");
            Assert.Equal("2", output);
        }

        [Fact]
        public void moO_ShouldMoveToNextCell()
        {
            var output = RunInterpreter("MoO moO MoO MoO OOM");
            Assert.Equal("2", output);
        }

        [Fact]
        public void mOo_ShouldMoveToPreviousCell()
        {
            var output = RunInterpreter("moO MoO MoO mOo OOM");
            Assert.Equal("0", output);
        }

        [Fact]
        public void OOM_ShouldOutputValue()
        {
            var output = RunInterpreter("MoO MoO MoO MoO MoO OOM");
            Assert.Equal("5", output);
        }

        [Fact]
        public void oom_ShouldInputValue()
        {
            var output = RunInterpreter("oom OOM", "42");
            Assert.Equal("42", output);
        }

        [Fact]
        public void OOO_ShouldZeroMemory()
        {
            var output = RunInterpreter("MoO MoO MoO OOO OOM");
            Assert.Equal("0", output);
        }

        [Fact]
        public void MMM_ShouldCopyToRegister()
        {
            var output = RunInterpreter("MoO MoO MoO MMM moO MMM OOM");
            Assert.Equal("3", output);
        }

        [Fact]
        public void Moo_ShouldPrintChar()
        {
            // ASCII 65 = 'A'
            var output = RunInterpreter("MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO MoO Moo");
            Assert.Equal("A", output);
        }

        [Fact]
        public void Moo_ShouldReadChar()
        {
            var output = RunInterpreter("Moo OOM", "B");
            Assert.Equal("66", output);
        }

        [Fact]
        public void SimpleLoop_ShouldDecrementToZero()
        {
            // Start with 3, decrement until 0
            var output = RunInterpreter("MoO MoO MoO MOO MOo moo OOM");
            Assert.Equal("0", output);
        }

        [Fact]
        public void MOO_ShouldSkipWhenZero()
        {
            // Memory is 0, so skip the loop
            var output = RunInterpreter("MOO MoO MoO MoO moo OOM");
            Assert.Equal("0", output);
        }

        [Fact]
        public void NestedLoops_ShouldWork()
        {
            // Outer loop with memory[0]=2, inner loop that moves right
            var output = RunInterpreter("MoO MoO MOO MOo moO MoO MOO MOo moo OOM moo mOo OOM");
            Assert.Equal("01", output);
        }

        [Fact]
        public void MultipleOutputs()
        {
            var output = RunInterpreter("MoO OOM MoO OOM MoO OOM");
            Assert.Equal("123", output);
        }

        [Fact]
        public void MoveBetweenCells()
        {
            var output = RunInterpreter("MoO MoO moO MoO MoO MoO mOo OOM moO OOM");
            Assert.Equal("23", output);
        }

        [Fact]
        public void RegisterCopyAndClear()
        {
            var output = RunInterpreter("MoO MoO MoO MoO MoO MMM moO MMM moO MoO MMM OOM");
            Assert.Equal("1", output);
        }
    }
}
