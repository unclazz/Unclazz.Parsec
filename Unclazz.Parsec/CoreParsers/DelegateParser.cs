using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class DelegateParser<T> : Parser<T>
    {
        internal DelegateParser(Func<Reader, ParseResult<T>> func)
        {
            _delegate = func ?? throw new ArgumentNullException(nameof(func));
        }

        readonly Func<Reader, ParseResult<T>> _delegate;

        public override ParseResult<T> Parse(Reader input)
        {
            return _delegate(input);
        }
        public override string ToString()
        {
            return string.Format("For({0})", _delegate.GetType());
        }
    }
}
