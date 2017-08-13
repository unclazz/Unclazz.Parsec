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
            Console.WriteLine(Parser.Word("hello").Parse(ParserInput.FromString(args[0])));
        }
    }
}
