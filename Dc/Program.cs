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
        static void Main(string[] args)
        {
            var dc = new Dc();
            var str = String.Join(" ", args);
            Console.WriteLine($"{str} => {dc.Calc(str)}");
        }
    }

    abstract class Kinds : KindsBase
    {
        public const int Value = 1;
        public const int Operator = 2;
        public const int Identifier = 3;
    }
}
