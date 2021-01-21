using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parser;

namespace Dc
{
    internal static class Program
    {
        static void Main (string[] args)
        {
            var sb = new StringBuilder ();
            for (var i = 0; i < args.Length; i++)
            {
                if (i > 0)
                    sb.Append (' ');
                sb.Append (args[i]);
            }

            var dc = new Dc ();
            Console.WriteLine ("{0:r}", dc.Calc (sb.ToString ()));
        }
    }

    abstract class Kinds : KindsBase
    {
        public const int Value = 1;
        public const int Operator = 2;
        public const int Identifier = 3;
    }
}
