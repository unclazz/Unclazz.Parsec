using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec
{
    public abstract class Parser<T>
    {
        public static implicit operator Parser<T>(Func<ParserInput, ParseResult<T>> func)
        {
            return For(func);
        }
        public static Parser<T> operator!(Parser<T> operand)
        {
            return Not(operand);
        }
        public static Parser<T> operator|(Parser<T> left, Parser<T> right)
        {
            return Or(left, right);
        }

        public static Parser<T> For<T>(Func<ParserInput, ParseResult<T>> func)
        {
            return new DelegateParser<T>(func);
        }
        public static Parser<T> Not(Parser<T> operand)
        {
            return new NotParser<T>(operand);
        }
        public static Parser<T> Or(Parser<T> left, Parser<T> right)
        {
            return new OrParser<T>(left, right);
        }

        public abstract ParseResult<T> Parse(ParserInput input);

        public Parser<U> Map<U>(Func<T,U> transform)
        {
            return new MapParser<T, U>(this, transform);
        }
        public Parser<IEnumerable<T>> Repeat(int min, int max)
        {
            return new RepeatMinMaxParser<T>(this, min, max);
        }
        public Parser<IEnumerable<T>> RepeatMin(int min)
        {
            return new RepeatMinMaxParser<T>(this, min, -1);
        }
        public Parser<IEnumerable<T>> RepeatMax(int max)
        {
            return new RepeatMinMaxParser<T>(this, 0, max);
        }
        public Parser<IEnumerable<T>> RepeatExactly(int exactly)
        {
            return new RepeatExactlyParser<T>(this, exactly);
        }
        public Parser<T> Or(Parser<T> another)
        {
            return new OrParser<T>(this, another);
        }
    }
}
