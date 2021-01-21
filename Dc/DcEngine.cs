using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser;

namespace Dc
{
    class DcEngine : IEngine
    {
        public Stack<double> Stack { get; }

        public DcEngine ()
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
                    double a1, a2;
                    switch ((string)lexem.Value)
                    {
                        case "+":
                            Needs (2);
                            Stack.Push (Stack.Pop () + Stack.Pop ());
                            break;
                        case "-":
                            Needs (2);
                            a2 = Stack.Pop ();
                            a1 = Stack.Pop ();
                            Stack.Push (a1 - a2);
                            break;
                        case "*":
                            Needs (2);
                            Stack.Push (Stack.Pop () * Stack.Pop ());
                            break;
                        case "/":
                            Needs (2);
                            a2 = Stack.Pop ();
                            a1 = Stack.Pop ();
                            Stack.Push (a1 / a2);
                            break;
                        case "%":
                            Needs (2);
                            a2 = Stack.Pop ();
                            a1 = Stack.Pop ();
                            Stack.Push (a1 % a2);
                            break;
                        case "!":
                            Needs (1);
                            Stack.Push (Fac (Stack.Pop ()));
                            break;
                        case "^":
                            Needs (2);
                            a2 = Stack.Pop ();
                            a1 = Stack.Pop ();
                            Stack.Push (Math.Pow (a1, a2));
                            break;
                        case "u-":
                            Needs (1);
                            Stack.Push (-Stack.Pop ());
                            break;
                        case "u+":
                            // does nothing, still check for argument
                            Needs (1);
                            break;

                        default:
                            throw new NotImplementedException ();
                    }
                    break;

                case Kinds.Identifier:
                    ExecFunction (lexem);
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
                return n * Fac (n - 1);
        }

        void ExecFunction (Lexem lexem)
        {
            foreach (var m in typeof (Math).GetMethods (System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
            {
                if (!String.Equals(lexem.Text, m.Name, StringComparison.InvariantCultureIgnoreCase)) 
                    continue;
                
                if (m.ReturnType != typeof (double))
                    continue;
                var paras = m.GetParameters ();
                foreach (var para in paras)
                    if (para.ParameterType != typeof (double))
                        continue;
                Needs (paras.Length);
                var p = new object[paras.Length];
                for (var i = p.Length - 1; i >= 0; i--)
                    p[i] = Stack.Pop ();
                Stack.Push ((double)m.Invoke (null, p));
                return;
            }

            foreach (var f in typeof (Math).GetFields (System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
            {
                if (String.Equals (lexem.Text, f.Name, StringComparison.InvariantCultureIgnoreCase) && typeof (double) == f.FieldType)
                {
                    Stack.Push ((double)f.GetValue (null));
                    return;
                }
            }

            throw new NotImplementedException ("Function " + lexem.Text + " not implemented");
        }
    }
}
