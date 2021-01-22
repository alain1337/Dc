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
        readonly Lex _lexer;
        readonly ShuntingYard _sy;

        public Dc()
        {
            _sy = new ShuntingYard { Preprocessor = Preprocess };

            _sy.Grammar.Add(new GrammarRule("+", Kinds.Operator, GrammarTokenType.Operator, 2, GrammarTokenAssociativity.Left, 2));
            _sy.Grammar.Add(new GrammarRule("-", Kinds.Operator, GrammarTokenType.Operator, 2, GrammarTokenAssociativity.Left, 2));

            _sy.Grammar.Add(new GrammarRule("*", Kinds.Operator, GrammarTokenType.Operator, 3, GrammarTokenAssociativity.Left, 2));
            _sy.Grammar.Add(new GrammarRule("/", Kinds.Operator, GrammarTokenType.Operator, 3, GrammarTokenAssociativity.Left, 2));
            _sy.Grammar.Add(new GrammarRule("%", Kinds.Operator, GrammarTokenType.Operator, 3, GrammarTokenAssociativity.Left, 2));

            _sy.Grammar.Add(new GrammarRule("^", Kinds.Operator, GrammarTokenType.Operator, 4, GrammarTokenAssociativity.Right, 2));

            _sy.Grammar.Add(new GrammarRule("!", Kinds.Operator, GrammarTokenType.Operator, 1, GrammarTokenAssociativity.Left, 1));

            _sy.Grammar.Add(new GrammarRule(null, Kinds.Value, GrammarTokenType.Value, 0));

            _sy.Grammar.Add(new GrammarRule(null, Kinds.Identifier, GrammarTokenType.Function, 0));
            _sy.Grammar.Add(new GrammarRule("(", Kinds.Operator, GrammarTokenType.LeftParenthesis, 0));
            _sy.Grammar.Add(new GrammarRule(")", Kinds.Operator, GrammarTokenType.RightParenthesis, 0));
            _sy.Grammar.Add(new GrammarRule(",", Kinds.Operator, GrammarTokenType.Separator, 0));

            _lexer = new Lex();
            _lexer.Rules.Add(LexRule.SpaceSkipper);
            _lexer.Rules.Add(new LexRule(LexPatterns.Float, typeof(double), Kinds.Value));
            _lexer.Rules.Add(new LexRule(LexPatterns.Identifier, typeof(string), Kinds.Identifier));
            foreach (var rule in _sy.Grammar)
                if (rule.SymbolName != null)
                    _lexer.Rules.Add(new LexRule(System.Text.RegularExpressions.Regex.Escape(rule.SymbolName), typeof(string), rule.SymbolKind));

            // Lexem preprocessor changes unary -/+ to those
            _sy.Grammar.Add(new GrammarRule("u+", Kinds.Operator, GrammarTokenType.Operator, 0, GrammarTokenAssociativity.Right, 1));
            _sy.Grammar.Add(new GrammarRule("u-", Kinds.Operator, GrammarTokenType.Operator, 0, GrammarTokenAssociativity.Right, 1));
        }

        public double Calc(string input)
        {
            var lexems = _lexer.Execute(input);
            var upn = new UpnConverter();
            _sy.Execute(lexems, upn);
            Console.WriteLine("[{0}]", upn.GetSource());
            var engine = new DcEngine();
            _sy.Execute(lexems, engine);
            var result = engine.Stack.Pop();
            if (engine.Stack.Count > 0)
                throw new InvalidOperationException("Value left on stack");
            return result;
        }

        void Preprocess(List<RuleMatch> rules)
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
                        rules[i] = _sy.Match(new Lexem("u-", typeof(string), Kinds.Operator));
                    else if (rules[i].Lexem.Text == "+")
                        rules[i] = _sy.Match(new Lexem("u+", typeof(string), Kinds.Operator));
                }

                prevRule = rules[i];
            }
        }
    }
}
