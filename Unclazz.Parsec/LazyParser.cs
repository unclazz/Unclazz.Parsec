using System;

namespace Unclazz.Parsec
{
    sealed class LazyParser<T> : Parser<T>
    {
        internal LazyParser(Func<Parser<T>> factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }
        readonly Func<Parser<T>> _factory;
        Parser<T> _cache;
        public override ParseResult<T> Parse(ParserInput input)
        {
            return (_cache ?? (_cache = _factory())).Parse(input);
        }
    }
}
