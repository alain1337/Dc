using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    /// <summary>
    /// http://en.wikipedia.org/wiki/Shunting_yard_algorithm
    /// </summary>
    public class ShuntingYard
    {
        public List<GrammarRule> Grammar { get; }
        public RuleMatchPreprocessor Preprocessor { get; set; }

        public delegate void RuleMatchPreprocessor (List<RuleMatch> matches);

        public ShuntingYard ()
        {
            Grammar = new List<GrammarRule> ();
        }

        public void Execute (List<Lexem> lexemInput, IEngine engine)
        {
            var input = new List<RuleMatch> ();
            foreach (var lexem in lexemInput)
                input.Add (Match (lexem));
            if (Preprocessor != null)
                Preprocessor (input);

            var stack = new Stack<RuleMatch> ();
            foreach (var rm in input)
            {
                switch (rm.Rule.Type)
                {
                    case GrammarTokenType.Value:
                        engine.Execute (rm.Lexem);
                        break;
                    case GrammarTokenType.Function:
                        stack.Push (rm);
                        break;
                    case GrammarTokenType.Separator:
                        while (stack.Peek ().Rule.Type != GrammarTokenType.LeftParenthesis)
                            engine.Execute (stack.Pop ().Lexem);
                        break;
                    case GrammarTokenType.Operator:
                        // HACK: This is a kludge
                        // TODO: Support right associative unary operands (http://www.reedbeta.com/blog/2011/12/11/the-shunting-yard-algorithm/)
                        if (rm.Rule.Operands == 1)
                        {
                            if (rm.Rule.Associativity == GrammarTokenAssociativity.Left)
                                engine.Execute (rm.Lexem);
                            else
                                stack.Push (rm);
                        }
                        else
                        {
                            while (stack.Count > 0 && stack.Peek ().Rule.Type == GrammarTokenType.Operator &&
                                ((rm.Rule.Associativity == GrammarTokenAssociativity.Left && rm.Rule.Precendence == stack.Peek ().Rule.Precendence)
                                    ||
                                    (rm.Rule.Precendence < stack.Peek ().Rule.Precendence)))
                                engine.Execute (stack.Pop ().Lexem);
                            stack.Push (rm);
                        }
                        break;
                    case GrammarTokenType.LeftParenthesis:
                        stack.Push (rm);
                        break;
                    case GrammarTokenType.RightParenthesis:
                        for (; ; )
                        {
                            if (stack.Count == 0)
                                throw new GrammarException ("Mismatched parenthesis", rm.Lexem);
                            if (stack.Peek ().Rule.Type == GrammarTokenType.LeftParenthesis)
                                break;
                            engine.Execute (stack.Pop ().Lexem);
                        }
                        stack.Pop ();
                        if (stack.Count > 0 && stack.Peek ().Rule.Type == GrammarTokenType.Function)
                            engine.Execute (stack.Pop ().Lexem);
                        break;


                    default:
                        throw new NotImplementedException ("GrammarTokenType " + rm.Rule.Type + " not implemented");
                }
            }

            while (stack.Count > 0)
            {
                var rm = stack.Pop ();
                if (rm.Rule.Type == GrammarTokenType.LeftParenthesis || rm.Rule.Type == GrammarTokenType.RightParenthesis)
                    throw new GrammarException ("Mismatched parenthesis", rm.Lexem);
                engine.Execute (rm.Lexem);
            }
        }

        public RuleMatch Match (Lexem lexem)
        {
            var rule = Grammar.Find ((r) => { return r.IsMatch (lexem); });
            if (rule == null)
                throw new GrammarException ("No rule for lexem " + lexem, lexem);
            return new RuleMatch (lexem, rule);
        }
    }

    public class RuleMatch
    {
        public Lexem Lexem { get; }
        public GrammarRule Rule { get; set; }

        public RuleMatch (Lexem lexem, GrammarRule rule)
        {
            Lexem = lexem;
            Rule = rule;
        }

        public override string ToString ()
        {
            return $"{Lexem}=>{Rule}";
        }
    }
}
