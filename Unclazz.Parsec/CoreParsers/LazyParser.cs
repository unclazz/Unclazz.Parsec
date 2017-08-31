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
        protected override ResultCore<T> DoParse(Reader input)
        {
            return (_cache ?? (_cache = _factory())).Parse(input);
        }
        public override string ToString()
        {
            return string.Format("Lazy<{0}>()", ParsecUtility.ObjectTypeToString(_factory));
        }
    }
    sealed class LazyParser : Parser
    {
        internal LazyParser(IParserConfiguration conf, Func<Parser> factory) : base(conf)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }
        readonly Func<Parser> _factory;
        Parser _cache;
        protected override ResultCore DoParse(Reader input)
        {
            return (_cache ?? (_cache = _factory())).Parse(input);
        }
        public override string ToString()
        {
            return string.Format("Lazy<{0}>()", ParsecUtility.ObjectTypeToString(_factory));
        }
    }
}
