using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser;

namespace Dc
{
    public class Dc
    {
        Lex Lexer;
        ShuntingYard SY;

        public Dc ()
        {
            SY = new ShuntingYard ();
            SY.Preprocessor = Preprocess;

            SY.Grammar.Add (new GrammarRule ("+", Kinds.Operator, GrammarTokenType.Operator, 2, GrammarTokenAssociativity.Left, 2));
            SY.Grammar.Add (new GrammarRule ("-", Kinds.Operator, GrammarTokenType.Operator, 2, GrammarTokenAssociativity.Left, 2));

            SY.Grammar.Add (new GrammarRule ("*", Kinds.Operator, GrammarTokenType.Operator, 3, GrammarTokenAssociativity.Left, 2));
            SY.Grammar.Add (new GrammarRule ("/", Kinds.Operator, GrammarTokenType.Operator, 3, GrammarTokenAssociativity.Left, 2));
            SY.Grammar.Add (new GrammarRule ("%", Kinds.Operator, GrammarTokenType.Operator, 3, GrammarTokenAssociativity.Left, 2));

            SY.Grammar.Add (new GrammarRule ("^", Kinds.Operator, GrammarTokenType.Operator, 4, GrammarTokenAssociativity.Right, 2));

            SY.Grammar.Add (new GrammarRule ("!", Kinds.Operator, GrammarTokenType.Operator, 1, GrammarTokenAssociativity.Left, 1));

            SY.Grammar.Add (new GrammarRule (null, Kinds.Value, GrammarTokenType.Value, 0));

            SY.Grammar.Add (new GrammarRule (null, Kinds.Identifier, GrammarTokenType.Function, 0));
            SY.Grammar.Add (new GrammarRule ("(", Kinds.Operator, GrammarTokenType.LeftParenthesis, 0));
            SY.Grammar.Add (new GrammarRule (")", Kinds.Operator, GrammarTokenType.RightParenthesis, 0));
            SY.Grammar.Add (new GrammarRule (",", Kinds.Operator, GrammarTokenType.Separator, 0));

            Lexer = new Lex ();
            Lexer.Rules.Add (LexRule.SpaceSkipper);
            Lexer.Rules.Add (new LexRule (LexPatterns.Float, typeof (double), Kinds.Value));
            Lexer.Rules.Add (new LexRule (LexPatterns.Identifier, typeof (string), Kinds.Identifier));
            foreach (var rule in SY.Grammar)
                if (rule.SymbolName != null)
                    Lexer.Rules.Add (new LexRule (System.Text.RegularExpressions.Regex.Escape (rule.SymbolName), typeof (string), rule.SymbolKind));

            // Lexem preprocessor changes unary -/+ to those
            SY.Grammar.Add (new GrammarRule ("u+", Kinds.Operator, GrammarTokenType.Operator, 0, GrammarTokenAssociativity.Right, 1));
            SY.Grammar.Add (new GrammarRule ("u-", Kinds.Operator, GrammarTokenType.Operator, 0, GrammarTokenAssociativity.Right, 1));
        }

        public double Calc (string input)
        {
            var lexems = Lexer.Execute (input);
            var upn = new UpnConverter ();
            SY.Execute (lexems, upn);
            Console.WriteLine ("[{0}]", upn.GetSource ());
            var engine = new DcEngine ();
            SY.Execute (lexems, engine);
            var result = engine.Stack.Pop ();
            if (engine.Stack.Count > 0)
                throw new InvalidOperationException ("Value left on stack");
            return result;
        }

        private void Preprocess (List<RuleMatch> rules)
        {
            RuleMatch prevRule = null;
            for (var i = 0; i < rules.Count; i++)
            {
                if (rules[i].Rule.Type == GrammarTokenType.Operator && (prevRule == null 
                    || (prevRule.Rule.Type == GrammarTokenType.Operator && prevRule.Rule.Operands == 2)
                    || (prevRule.Rule.Type == GrammarTokenType.Operator && prevRule.Rule.Operands == 1 && prevRule.Rule.Associativity == GrammarTokenAssociativity.Right)
                    || (prevRule.Rule.Type == GrammarTokenType.LeftParenthesis)))
                {
                    if (rules[i].Lexem.Text == "-")
                        rules[i] = SY.Match (new Lexem ("u-", typeof (string), Kinds.Operator));
                    else if (rules[i].Lexem.Text == "+")
                        rules[i] = SY.Match (new Lexem ("u+", typeof (string), Kinds.Operator));
                }

                prevRule = rules[i];
            }
        }
    }
}
