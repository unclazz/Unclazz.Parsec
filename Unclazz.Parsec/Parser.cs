using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec
{
    public abstract class Parser<T>
    {
        public static implicit operator Parser<T>(Func<IParserInput, ParseResult<T>> func)
        {
            return For(func);
        }
        public static Parser<T> operator!(Parser<T> operand)
        {
            return Not(operand);
        }

        public static Parser<T> For<T>(Func<IParserInput, ParseResult<T>> func)
        {
            return new DelegateParser<T>(func);
        }
        public static Parser<T> Not(Parser<T> operand)
        {
            return new NegativeParser<T>(operand);
        }

        public abstract ParseResult<T> Parse(IParserInput input);
    }

    sealed class NegativeParser<T> : Parser<T>
    {
        internal NegativeParser(Parser<T> original)
        {
            Original = original ?? throw new ArgumentNullException(nameof(original));
        }

        Parser<T> Original { get; }
        
        public override ParseResult<T> Parse(IParserInput input)
        {
            // TODO
            throw new NotImplementedException();
        }
    }
}
