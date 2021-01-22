using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser;

namespace Dc
{
    internal class DcEngine : IEngine
    {
        public Stack<double> Stack { get; }
        public DcFunctions Functions { get; } = new DcFunctions();

        public DcEngine()
        {
            Stack = new Stack<double>();
        }

        public void Execute(Lexem lexem)
        {
            switch (lexem.Kind)
            {
                case Kinds.Value:
                    Stack.Push((double)lexem.Value);
                    break;

                case Kinds.Operator:
                    double a1, a2;
                    switch ((string)lexem.Value)
                    {
                        case "+":
                            Needs(2);
                            Stack.Push(Stack.Pop() + Stack.Pop());
                            break;
                        case "-":
                            Needs(2);
                            a2 = Stack.Pop();
                            a1 = Stack.Pop();
                            Stack.Push(a1 - a2);
                            break;
                        case "*":
                            Needs(2);
                            Stack.Push(Stack.Pop() * Stack.Pop());
                            break;
                        case "/":
                            Needs(2);
                            a2 = Stack.Pop();
                            a1 = Stack.Pop();
                            Stack.Push(a1 / a2);
                            break;
                        case "%":
                            Needs(2);
                            a2 = Stack.Pop();
                            a1 = Stack.Pop();
                            Stack.Push(a1 % a2);
                            break;
                        case "!":
                            Needs(1);
                            Stack.Push(Fac(Stack.Pop()));
                            break;
                        case "^":
                            Needs(2);
                            a2 = Stack.Pop();
                            a1 = Stack.Pop();
                            Stack.Push(Math.Pow(a1, a2));
                            break;
                        case "u-":
                            Needs(1);
                            Stack.Push(-Stack.Pop());
                            break;
                        case "u+":
                            // does nothing, still check for argument
                            Needs(1);
                            break;

                        default:
                            throw new NotImplementedException();
                    }
                    break;

                case Kinds.Identifier:
                    Functions.Execute(lexem.Text, Stack);
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        void Needs(int arguments)
        {
            if (Stack.Count < arguments)
                throw new InvalidOperationException("No enough values on stack");
        }

        double Fac(double n)
        {
            if (Math.Abs(n - Math.Floor(n)) > Double.Epsilon)
                throw new InvalidOperationException("! needs a integer argument");

            if (n < 3)
                return n;
            else
                return n * Fac(n - 1);
        }
    }
}
