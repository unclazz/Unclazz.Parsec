using System;
using System.Collections.Generic;

namespace Unclazz.Parsec
{
    sealed class MapParser<T, U> : Parser<U>
    {
        internal MapParser(Parser<T> source, Func<string, U> transform, bool canThrow)
        {
            var tmp = source ?? throw new ArgumentNullException(nameof(source));
            _source = source.Capture();
            _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            _canThrow = canThrow;
        }

        readonly Parser<string> _source;
        readonly Func<string, U> _transform;
        readonly bool _canThrow;

        public override ParseResult<U> Parse(ParserInput input)
        {
            var r = _source.Parse(input);
            try
            {
                return r.Map(_transform);
            }
            catch (Exception ex)
            {
                if (_canThrow) throw;
                return Failure(r.Position, ex.ToString(), r.CanBacktrack);
            }
        }
        public override string ToString()
        {
            return string.Format("Map({0}, transform = {1})",
                _source, ParsecUtility.FuncToString(_transform));
        }
    }
    sealed class EnumerableMapParser<T, U> : Parser<U>
    {
        internal EnumerableMapParser(Parser<IEnumerable<T>> source, Func<IEnumerable<T>, U> transform, bool canThrow)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            _canThrow = canThrow;
        }

        readonly Parser<IEnumerable<T>> _source;
        readonly Func<IEnumerable<T>, U> _transform;
        readonly bool _canThrow;

        public override ParseResult<U> Parse(ParserInput input)
        {
            var r = _source.Parse(input);
            try
            {
                return r.Map(_transform);
            }
            catch (Exception ex)
            {
                if (_canThrow) throw;
                return Failure(r.Position, ex.ToString(), r.CanBacktrack);
            }
        }
        public override string ToString()
        {
            return string.Format("Map({0}, transform = {1})",
                _source, ParsecUtility.FuncToString(_transform));
        }
    }
}
