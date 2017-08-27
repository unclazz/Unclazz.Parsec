using System;
using System.Collections.Generic;
using System.Linq;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class MapParser<T, U> : Parser<U>
    {
        internal MapParser(IParserConfiguration conf, Parser<T> source, Func<T, U> transform, bool canThrow) : base(conf)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            _canThrow = canThrow;
        }

        readonly Parser<T> _source;
        readonly Func<T, U> _transform;
        readonly bool _canThrow;

        protected override ParseResult<U> DoParse(Reader input)
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
                _source, ParsecUtility.ObjectTypeToString(_transform));
        }
    }
}
