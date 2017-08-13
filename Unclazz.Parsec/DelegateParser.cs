using System;

namespace Unclazz.Parsec
{
    sealed class DelegateParser<T> : Parser<T>
    {
        internal DelegateParser(Func<ParserInput, ParseResult<T>> func)
        {
            Delegate = func ?? throw new ArgumentNullException(nameof(func));
        }

        Func<ParserInput, ParseResult<T>> Delegate { get; }

        public override ParseResult<T> Parse(ParserInput input)
        {
            return Delegate(input);
        }
    }
}
