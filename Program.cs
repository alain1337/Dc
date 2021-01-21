using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser;

namespace Dc
{
    class Program
    {
        static Lex Lexer;
        static ShuntingYard SY;

        static void Main (string[] args)
        {
            SetupParser ();

            List<Lexem> lexems = new List<Lexem> ();
            foreach (string arg in args)
                lexems.AddRange (Lexer.Execute (arg));

            CalcEngine engine = new CalcEngine ();
            SY.Execute (lexems, engine);
            Console.WriteLine (engine.Stack.Pop ());
            if (engine.Stack.Count > 0)
                Console.Error.WriteLine ("WARNING: Some value(s) left on stack");
        }


        static void SetupParser ()
        {
            Lexer = new Lex ();
            Lexer.Rules.Add (LexRule.SpaceSkipper);
            Lexer.Rules.Add (new LexRule (LexPatterns.Float, typeof (double), Kinds.Value));
            foreach (string op in @"\+ \- \* \/ \( \) ! ,".Split (' '))
                Lexer.Rules.Add (new LexRule (op, typeof (string), Kinds.Operator));
            Lexer.Rules.Add (new LexRule (LexPatterns.Identifier, typeof (string), Kinds.Identifier));

            
            SY = new ShuntingYard ();
            SY.Grammar.Add (new GrammarRule ("+", Kinds.Operator, GrammarTokenType.Operator, 2, GrammarTokenAssociativity.Left));
            SY.Grammar.Add (new GrammarRule ("-", Kinds.Operator, GrammarTokenType.Operator, 2, GrammarTokenAssociativity.Left));

            SY.Grammar.Add (new GrammarRule ("*", Kinds.Operator, GrammarTokenType.Operator, 3, GrammarTokenAssociativity.Left));
            SY.Grammar.Add (new GrammarRule ("/", Kinds.Operator, GrammarTokenType.Operator, 3, GrammarTokenAssociativity.Left));

            SY.Grammar.Add (new GrammarRule ("!", Kinds.Operator, GrammarTokenType.Operator, 1, GrammarTokenAssociativity.Left));

            SY.Grammar.Add (new GrammarRule (null, Kinds.Value, GrammarTokenType.Value, 0, GrammarTokenAssociativity.None));

            SY.Grammar.Add (new GrammarRule (null, Kinds.Identifier, GrammarTokenType.Function, 0, GrammarTokenAssociativity.None));
            SY.Grammar.Add (new GrammarRule ("(", Kinds.Operator, GrammarTokenType.LeftParenthesis, 0, GrammarTokenAssociativity.None));
            SY.Grammar.Add (new GrammarRule (")", Kinds.Operator, GrammarTokenType.RightParenthesis, 0, GrammarTokenAssociativity.None));
            SY.Grammar.Add (new GrammarRule (",", Kinds.Operator, GrammarTokenType.Separator, 0, GrammarTokenAssociativity.None));
        }
    }

    abstract class Kinds : KindsBase
    {
        public const int Value = 1;
        public const int Operator = 2;
        public const int Identifier = 3;
    }
}
