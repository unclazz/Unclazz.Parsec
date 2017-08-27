using Example.Unclazz.Parcec.Math;
using System;
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
            var parser = new ExpressionParser();
            var input = args.Aggregate(new StringBuilder(),
                (b, a) => b.Append(a).Append(' ')).ToString();

            parser.Parse(input)
                .IfSuccessful(c => Console.WriteLine("result = {0}", c.Value), 
                m => Console.WriteLine("error = {0}", m));
        }
    }
}
