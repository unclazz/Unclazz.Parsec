using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class LazyParser<T> : Parser<T>
    {
        internal LazyParser(IParserConfiguration conf, Func<Parser<T>> factory) : base(conf)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }
        readonly Func<Parser<T>> _factory;
        Parser<T> _cache;
        protected override ParseResult<T> DoParse(Reader input)
        {
            return (_cache ?? (_cache = _factory())).Parse(input);
        }
    }
}
