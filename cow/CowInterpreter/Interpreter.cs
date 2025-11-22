using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CowInterpreter
{
    public class Interpreter
    {
        private readonly List<int> _memory = new List<int> { 0 };
        private int _memoryPtr;
        private int? _register;
        private readonly string[] _instructions;
        private int _instructionPtr;
        private readonly TextReader _input;
        private readonly TextWriter _output;

        public Interpreter(string code, TextReader input, TextWriter output)
        {
            _instructions = code.Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            _input = input;
            _output = output;
        }

        public void Run()
        {
            while (_instructionPtr < _instructions.Length)
            {
                ExecuteInstruction(_instructions[_instructionPtr]);
            }
        }

        private void ExecuteInstruction(string instruction)
        {
            switch (instruction)
            {
                case "MoO":
                    _memory[_memoryPtr]++;
                    _instructionPtr++;
                    break;
                case "MOo":
                    _memory[_memoryPtr]--;
                    _instructionPtr++;
                    break;
                case "moO":
                    _memoryPtr++;
                    if (_memoryPtr == _memory.Count)
                    {
                        _memory.Add(0);
                    }
                    _instructionPtr++;
                    break;
                case "mOo":
                    _memoryPtr--;
                    if (_memoryPtr < 0)
                    {
                        _memoryPtr = 0;
                    }
                    _instructionPtr++;
                    break;
                case "moo":
                    // Always jump back to matching MOO
                    int loopDepthBack = 1;
                    while (loopDepthBack > 0)
                    {
                        _instructionPtr--;
                        if (_instructionPtr < 0)
                        {
                            break;
                        }
                        if (_instructions[_instructionPtr] == "moo")
                        {
                            loopDepthBack++;
                        }
                        else if (_instructions[_instructionPtr] == "MOO")
                        {
                            loopDepthBack--;
                        }
                    }
                    // Don't increment instructionPtr - we want to execute the MOO command
                    break;
                case "MOO":
                    if (_memory[_memoryPtr] == 0)
                    {
                        int loopDepth = 1;
                        _instructionPtr++; // Skip instruction immediately after MOO
                        while (loopDepth > 0 && _instructionPtr < _instructions.Length)
                        {
                            if (_instructions[_instructionPtr] == "MOO")
                            {
                                loopDepth++;
                            }
                            else if (_instructions[_instructionPtr] == "moo")
                            {
                                loopDepth--;
                            }
                            
                            if (loopDepth > 0)
                            {
                                _instructionPtr++;
                            }
                        }
                    }
                    _instructionPtr++;
                    break;
                case "OOM":
                    _output.Write(_memory[_memoryPtr]);
                    _instructionPtr++;
                    break;
                case "oom":
                    _memory[_memoryPtr] = int.Parse(_input.ReadLine() ?? "0");
                    _instructionPtr++;
                    break;
                case "mOO":
                    _instructionPtr = _memory[_memoryPtr];
                    break;
                case "Moo":
                    if (_memory[_memoryPtr] == 0)
                    {
                        _memory[_memoryPtr] = _input.Read();
                    }
                    else
                    {
                        _output.Write((char)_memory[_memoryPtr]);
                    }
                    _instructionPtr++;
                    break;
                case "OOO":
                    _memory[_memoryPtr] = 0;
                    _instructionPtr++;
                    break;
                case "MMM":
                    if (_register.HasValue)
                    {
                        _memory[_memoryPtr] = _register.Value;
                        _register = null;
                    }
                    else
                    {
                        _register = _memory[_memoryPtr];
                    }
                    _instructionPtr++;
                    break;
                default:
                    _instructionPtr++;
                    break;
            }
        }
    }
}
