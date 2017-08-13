using System;

namespace Unclazz.Parsec
{
    sealed class DelegateParser<T> : Parser<T>
    {
        internal DelegateParser(Func<ParserInput, ParseResult<T>> func)
        {
            _delegate = func ?? throw new ArgumentNullException(nameof(func));
        }

        readonly Func<ParserInput, ParseResult<T>> _delegate;

        public override ParseResult<T> Parse(ParserInput input)
        {
            return _delegate(input);
        }
        public override string ToString()
        {
            return string.Format("Parser({0}", _delegate.GetType());
        }
    }
}
