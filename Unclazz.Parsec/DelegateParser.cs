using System;

namespace Unclazz.Parsec
{
    sealed class DelegateParser<T> : Parser<T>
    {
        internal DelegateParser(Func<IParserInput, ParseResult<T>> func)
        {
            Delegate = func ?? throw new ArgumentNullException(nameof(func));
        }

        Func<IParserInput, ParseResult<T>> Delegate { get; }

        public override ParseResult<T> Parse(IParserInput input)
        {
            return Delegate(input);
        }
    }
}
