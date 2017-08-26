using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class LazyParser<T> : Parser<T>
    {
        internal LazyParser(Func<IParser<T>> factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }
        readonly Func<IParser<T>> _factory;
        IParser<T> _cache;
        public override ParseResult<T> Parse(Reader input)
        {
            return (_cache ?? (_cache = _factory())).Parse(input);
        }
    }
}
