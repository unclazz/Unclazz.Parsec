using System;
using System.Collections.Generic;

namespace Unclazz.Parsec
{
    sealed class MapParser<T, U> : Parser<U>
    {
        internal MapParser(Parser<T> source, Func<string, U> transform)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _transform = transform ?? throw new ArgumentNullException(nameof(transform));
        }

        readonly Parser<T> _source;
        readonly Func<string, U> _transform;

        public override ParseResult<U> Parse(ParserInput input)
        {
            return ParsecUtility.ProxyCapture(_source, input).Map(_transform);
        }
        public override string ToString()
        {
            return string.Format("Map({0}, transform = {1})",
                _source, ParsecUtility.FuncToString(_transform));
        }
    }
    sealed class EnumerableMapParser<T, U> : Parser<U>
    {
        internal EnumerableMapParser(Parser<IEnumerable<T>> source, Func<IEnumerable<T>, U> transform)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _transform = transform ?? throw new ArgumentNullException(nameof(transform));
        }

        readonly Parser<IEnumerable<T>> _source;
        readonly Func<IEnumerable<T>, U> _transform;

        public override ParseResult<U> Parse(ParserInput input)
        {
            return _source.Parse(input).Map(_transform);
        }
        public override string ToString()
        {
            return string.Format("Map({0}, transform = {1})",
                _source, ParsecUtility.FuncToString(_transform));
        }
    }
}
