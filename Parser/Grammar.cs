using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public class GrammarRule
    {
        public string SymbolName { get; }
        public int SymbolKind { get; }
        public GrammarTokenType Type { get; }
        public int Precendence { get; }
        public GrammarTokenAssociativity Associativity { get; }
        public int Operands { get; }

        public GrammarRule (string name, int kind, GrammarTokenType type, int precendence, GrammarTokenAssociativity associativity = GrammarTokenAssociativity.None, int operands = 0)
        {
            if (name == null && kind == 0)
                throw new ArgumentException ("name and kind cannot both be null/0, at least one must be defined");

            SymbolName = name;
            SymbolKind = kind;
            Type = type;
            Precendence = precendence;
            Associativity = associativity;
            Operands = operands;
        }

        public bool IsMatch (Lexem lexem)
        {
            if (SymbolName != null && !Equals (SymbolName, lexem.Value))
                return false;
            else if (SymbolKind != 0 && SymbolKind != lexem.Kind)
                return false;
            else
                return true;
        }

        public override string ToString ()
        {
            return $"{SymbolName}/{SymbolKind} [{Type},{Precendence},{Associativity},{Operands}]";
        }
    }

    public enum GrammarTokenType
    {
        Value,
        Operator,
        Function,
        Separator,
        LeftParenthesis,
        RightParenthesis
    }

    public enum GrammarTokenAssociativity
    {
        None,
        Left,
        Right
    }

    [Serializable]
    public class GrammarException : Exception
    {
        public Lexem Lexem { get; private set; }

        public GrammarException (string message, Lexem lexem)
            : base (message)
        {
            Lexem = lexem;
        }

        //public string GetErrorHelper ()
        //{
        //    return String.Format ("{0}\n{1}^", Input, new string (' ', Position));
        //}
    }
}