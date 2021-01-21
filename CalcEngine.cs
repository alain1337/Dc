using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser;

namespace Dc
{
    class CalcEngine : Parser.IEngine
    {
        public Stack<double> Stack { get; private set; }

        public CalcEngine ()
        {
            Stack = new Stack<double> ();
        }

        public void Execute (Lexem lexem)
        {
            switch (lexem.Kind)
            {
                case Kinds.Value:
                    Stack.Push ((double)lexem.Value);
                    break;

                case Kinds.Operator:
                    switch ((string)lexem.Value)
                    {
                        case "+":
                            Needs (2);
                            Stack.Push (Stack.Pop () + Stack.Pop ());
                            break;
                        case "-":
                            Needs (2);
                            Stack.Push (Stack.Pop () - Stack.Pop ());
                            break;
                        case "*":
                            Needs (2);
                            Stack.Push (Stack.Pop () * Stack.Pop ());
                            break;
                        case "/":
                            Needs (2);
                            Stack.Push (Stack.Pop () / Stack.Pop ());
                            break;
                        case "!":
                            Needs (1);
                            Stack.Push (Fac (Stack.Pop ()));
                            break;

                        default:
                            throw new NotImplementedException ();
                    }
                    break;

                default:
                    throw new NotImplementedException ();
            }
        }

        void Needs (int arguments)
        {
            if (Stack.Count < arguments)
                throw new InvalidOperationException ("No enough values on stack");
        }

        double Fac (double n)
        {
            if (n != Math.Floor (n))
                throw new InvalidOperationException ("! needs a integer argument");

            if (n < 3)
                return n;
            else
                return n * Fac(n-1);
        }
    }
}
