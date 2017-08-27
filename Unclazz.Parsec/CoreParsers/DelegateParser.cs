using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class DelegateParser<T> : Parser<T>
    {
        internal DelegateParser(IParserConfiguration conf, Func<Reader, ParseResult<T>> func) : base(conf)
        {
            _delegate = func ?? throw new ArgumentNullException(nameof(func));
        }

        readonly Func<Reader, ParseResult<T>> _delegate;

        protected override ParseResult<T> DoParse(Reader input)
        {
            return _delegate(input);
        }
        public override string ToString()
        {
            return string.Format("For({0})", _delegate.GetType());
        }
    }
}
