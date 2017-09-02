using Example.Unclazz.Parsec.Math;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec;

namespace Example.Unclazz.Parsec
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new ExpressionParser();
            var input = args.Aggregate(new StringBuilder(),
                (b, a) => b.Append(a).Append(' ')).ToString();

            parser.Parse(input)
                .IfSuccessful(v => Console.WriteLine("result = {0}", v),
                                m => Console.WriteLine("error = {0}", m));
        }
    }
}
