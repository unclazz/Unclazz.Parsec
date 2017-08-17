using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec;
using static Unclazz.Parsec.Parser;

namespace Example.Unclazz.Parcec
{
    class Program
    {
        static void Main(string[] args)
        {
            // 例：JavaScriptのIntegerリテラルを読み取るパーサー
            var space = CharsWhileIn(" \r\n", min: 0);
            var sign = CharIn("+-").OrNot();
            var digits = CharsWhileIn("0123456789");
            var integral = Char('0') | (CharBetween('1', '9') & digits);
            var fractional = Char('.') & digits;
            var exponent = CharIn("eE") & (sign) & (digits);
            var number = (space & (sign & integral & fractional.OrNot() & exponent.OrNot()).Capture() & space).Map(double.Parse);

            Console.WriteLine("input = {0}", args[0]);
            Console.WriteLine("parsed = {0}", number.Parse(args[0]));
        }
    }
}
