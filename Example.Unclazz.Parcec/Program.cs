using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec;

namespace Example.Unclazz.Parcec
{
    class Program
    {
        static void Main(string[] args)
        {
            // 例：JavaScriptのIntegerリテラルを読み取るパーサー
            var space = Parser.CharsWhileIn(" \r\n", min: 0);
            var sign = Parser.CharIn("+-").OrNot();
            var digits = Parser.CharsWhileIn("0123456789");
            var integral = Parser.Char('0').Or(Parser.CharBetween('1', '9').Concat(digits));
            var fractional = Parser.Char('.').Concat(digits);
            var exponent = Parser.CharIn("eE").Concat(sign).Concat(digits);
            var number = space.Concat(sign.Concat(integral, fractional.OrNot(),
                exponent.OrNot()).Capture()).Concat(space).Map(double.Parse);

            Console.WriteLine("input = {0}", args[0]);
            Console.WriteLine("parsed = {0}", number.Parse(args[0]));
        }
    }
}
