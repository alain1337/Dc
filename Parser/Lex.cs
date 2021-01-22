using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Parser
{
    public class Lex
    {
        public List<LexRule> Rules { get; }

        public Lex ()
        {
            Rules = new List<LexRule> ();
        }

        public List<Lexem> Execute (string input)
        {
            var lexems = new List<Lexem> ();

            for (var pos = 0; pos < input.Length; )
            {
                var token = input.Substring (pos);
                if (!FindBestMatch (token, out var foundRule, out var foundMatch))
                    throw new LexException ("Invalid character", input, pos);

                if (foundRule.Capturing)
                    lexems.Add (new Lexem (foundMatch.Value, foundRule.ValueType, foundRule.Kind));
                pos += foundMatch.Length;
            }

            return lexems;
        }

        bool FindBestMatch (string input, out LexRule foundRule, out Match foundMatch)
        {
            foundRule = null;
            foundMatch = null;

            foreach (var rule in Rules)
            {
                var ma = rule.Regex.Match (input);
                if (ma.Success)
                {
                    if (ma.Length == 0)
                        throw new InvalidOperationException ("Invalid rule: 0 length result matched [" + rule.Pattern + "]");
                    if (foundRule == null || ma.Length > foundMatch?.Length)
                    {
                        foundRule = rule;
                        foundMatch = ma;
                    }
                }
            }

            return foundRule != null;
        }
    }

    public class LexRule
    {
        public string Pattern { get; }
        public Regex Regex { get; }
        public bool Capturing { get; }

        public Type ValueType { get; }
        public int Kind { get; }

        public LexRule (string regexp, Type type, int kind, bool capturing = true)
        {
            Pattern = regexp;
            Regex = new Regex ("^(" + Pattern + ")");
            Capturing = capturing;
            ValueType = type;
            Kind = kind;
        }

        public static LexRule SpaceSkipper => new LexRule (@"\s+", null, 0, false);
    }

    [Serializable]
    public class LexException : Exception
    {
        public string Input { get; private set; }
        public int Position { get; private set; }

        public LexException (string message, string input, int position)
            : base ($"{message} [pos {position}]")
        {
            Input = input;
            Position = position;
        }

        public string GetErrorHelper ()
        {
            return $"{Input}\n{new string(' ', Position)}^";
        }
    }
}