using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEditor;
using UnityEngine;

//一个简单的计算器中使用的Command模式，其撤销和重做的次数没有限制
//C#中 operator是一个关键字，用‘@’作为前缀可以把它作为一个标识符使用

namespace CommandExample1
{
    public class CommandExample1 : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            // Create user and let her compute
            User user = new User();
            
            // User presses calculator buttons
            user.Compute('+', 100);
            user.Compute('-',50);
            user.Compute('*',10);
            user.Compute('/',2);
            
            user.Undo(4);
            user.Redo(3);
        }
    }

    abstract class Command
    {
        public abstract void Execute();

        public abstract void UnExecute();
    }

    class CalculatorCommand : Command
    {
        private char _operator;
        private int _operand;
        private Calculator _calculator;

        public CalculatorCommand(Calculator calculator, char @operator, int operand)
        {
            this._calculator = calculator;
            this._operator = @operator;
            this._operand = operand;
        }

        public override void Execute()
        {
            _calculator.Operation(_operator, _operand);
        }

        public override void UnExecute()
        {
            _calculator.Operation(Undo(_operator), _operand);
        }

        private char Undo(char @operator)
        {
            switch (@operator)
            {
                case '+': return '-';
                case '-': return '+';
                case '*': return '/';
                case '/': return '*';
                default:
                    throw new 
                ArgumentException("@operator");
            }
        }
    }

    class Calculator
    {
        private int _curr = 0;

        public void Operation(char @operator, int operand)
        {
            switch (@operator)
            {
                case '+': _curr += operand;
                    break;
                case '-': _curr -= operand;
                    break;
                case '*': _curr *= operand;
                    break;
                case '/': _curr /= operand;
                    break;
            }
            Debug.Log("Current value = " + _curr+" ( following "+ @operator+operand+" )");
        }
    }

    /// <summary>
    /// The 'Invoker' class
    /// </summary>
    class User
    {
        private Calculator _calculator = new Calculator();
        private List<Command> _commands = new List<Command>();

        private int _current = 0;

        public void Redo(int levels)
        {
            for (int i = 0; i < levels; i++)
            {
                if (_current < _commands.Count - 1)
                {
                    Command command = _commands[_current++];
                    command.Execute();
                }
            }
        }

        public void Undo(int levels)
        {
            Debug.Log("\n---- Undo " + levels + " levels");
            // Perform undo operations
            for (int i = 0; i < levels; i++)
            {
                if (_current > 0)
                {
                    Command command = _commands[--_current] as Command;
                    command.UnExecute();
                }
            }
        }

        public void Compute(char @operator, int operand)
        {
            Command command = new CalculatorCommand(
                _calculator, @operator, operand);
            command.Execute();
            
            _commands.Add(command);
            _current++;
        }
    }    
}
