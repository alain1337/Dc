using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    public static class LexPatterns
    {
        public const string Integer = @"[0-9]+";
        public const string Float = @"[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?";
        public const string String = @"""[^""\\]*(?:\\.[^""\\]*)*""";
        public const string Bool = @"(true)|(false)";
        public const string Identifier = @"[A-Za-z_][A-Za-z_0-9]*";
    }
}
