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
            var sign = Parser.Optional(Parser.CharIn("+-"));
            var space = Parser.CharsWhileIn(" \r\n", min: 0);
            var digits = Parser.CharsWhileIn("0123456789");
            var exponent = Parser.CharIn("eE").Then(sign).Then(digits);
            var fractional = Parser.Char('.').Then(digits);
            var integral = Parser.Char('.').Or(Parser.CharBetween('1', '9').Then(digits));
            var number = sign.Then(integral, Parser.Optional(fractional),
                Parser.Optional(exponent), Parser.EndOfFile)
                .Map(s => double.Parse(s));

            Console.WriteLine("input = {0}", args[0]);
            Console.WriteLine("parsed = {0}", number.Parse(args[0]));
        }
    }
}
