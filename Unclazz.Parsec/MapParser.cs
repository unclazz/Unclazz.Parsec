using System;

namespace Unclazz.Parsec
{
    sealed class MapParser<T, U> : Parser<U>
    {
        internal MapParser(Parser<T> source, Func<T, U> transform)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _transform = transform ?? throw new ArgumentNullException(nameof(transform));
        }

        readonly Parser<T> _source;
        readonly Func<T, U> _transform;

        public override ParseResult<U> Parse(ParserInput input)
        {
            return _source.Parse(input).Map(_transform);
        }
        public override string ToString()
        {
            return string.Format("{0}.Map({1})", _source, _transform.GetType());
        }
    }
}
