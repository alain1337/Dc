using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    internal static class Program
    {
        static void Main (string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine ("usage: Parser filename | token [token ...]");
                return;
            }

            var sb = new StringBuilder ();
            for (var i = 0; i < args.Length; i++)
            {
                if (i > 0)
                    sb.Append (' ');
                sb.Append (args[i]);
            }

            Test (sb.ToString ());
        }

        static void Test (string input)
        {
            Console.WriteLine (input);
            Console.WriteLine (new String ('-', input.Length));
            var lex = CreateLex ();
            var grammar = CreateGrammar ();
            var sy = new ShuntingYard ();
            sy.Grammar.AddRange (grammar);

            try
            {
                var lexems = lex.Execute (input);
                foreach (var lexem in lexems)
                    Console.WriteLine ("   {0,-30} {1,-15} {2}", lexem.Text, lexem.Kind == Kinds.Value ? lexem.ValueType.Name : KindsBase.ToString (lexem.Kind), lexem.Value);
                Console.WriteLine ();


                var upn = new UpnConverter ();
                sy.Execute (lexems, upn);
                Console.WriteLine ("UPN");
                Console.WriteLine ("   {0}", upn.GetSource ());
                Console.WriteLine ();
            }
            catch (LexException le)
            {
                Console.WriteLine (le.Message);
                Console.WriteLine (le.GetErrorHelper ());
            }
        }

        static Lex CreateLex ()
        {
            var lex = new Lex ();

            lex.Rules.Add (LexRule.SpaceSkipper);
            lex.Rules.Add (new LexRule (LexPatterns.Integer, typeof (Int32), Kinds.Value));
            lex.Rules.Add (new LexRule (LexPatterns.Float, typeof (double), Kinds.Value));
            lex.Rules.Add (new LexRule (LexPatterns.String, typeof (string), Kinds.Value));
            lex.Rules.Add (new LexRule (LexPatterns.Bool, typeof (bool), Kinds.Value));
            foreach (var op in @"\+ \- \* \/ \( \) && \|\| == != < > <= >= =~ ! ,".Split (' '))
                lex.Rules.Add (new LexRule (op, typeof (string), Kinds.Operator));
            lex.Rules.Add (new LexRule (LexPatterns.Identifier, typeof (string), Kinds.Identifier));

            return lex;
        }

        static List<GrammarRule> CreateGrammar ()
        {
            return new List<GrammarRule>
            {
                new GrammarRule("+", Kinds.Operator, GrammarTokenType.Operator, 2, GrammarTokenAssociativity.Left, 2),
                new GrammarRule("-", Kinds.Operator, GrammarTokenType.Operator, 2, GrammarTokenAssociativity.Left, 2),
                new GrammarRule("*", Kinds.Operator, GrammarTokenType.Operator, 3, GrammarTokenAssociativity.Left, 2),
                new GrammarRule("/", Kinds.Operator, GrammarTokenType.Operator, 3, GrammarTokenAssociativity.Left, 2),
                new GrammarRule(null, Kinds.Value, GrammarTokenType.Value, 0),
                new GrammarRule(null, Kinds.Identifier, GrammarTokenType.Function, 0),
                new GrammarRule("(", Kinds.Operator, GrammarTokenType.LeftParenthesis, 0),
                new GrammarRule(")", Kinds.Operator, GrammarTokenType.RightParenthesis, 0),
                new GrammarRule(",", Kinds.Operator, GrammarTokenType.Separator, 0)
            };
        }
    }

    internal abstract class Kinds : KindsBase
    {
        public const int Value = 1;
        public const int Operator = 2;
        public const int Identifier = 3;
    }
}
