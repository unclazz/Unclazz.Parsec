using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class MapParser<TSource, TResult> : Parser<TResult>
    {
        internal MapParser(IParserConfiguration conf, Parser<TSource> source, 
            Func<TSource, TResult> transform, bool canThrow) : base(conf)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            _canThrow = canThrow;
        }

        readonly Parser<TSource> _source;
        readonly Func<TSource, TResult> _transform;
        readonly bool _canThrow;

        protected override ResultCore<TResult> DoParse(Reader input)
        {
            var r = _source.Parse(input);
            try
            {
                return r.Map(_transform);
            }
            catch (Exception ex)
            {
                if (_canThrow) throw;
                return Failure(ex.ToString(), r.CanBacktrack);
            }
        }
        public override string ToString()
        {
            return string.Format("Map({0}, transform = {1})",
                _source, ParsecUtility.ObjectTypeToString(_transform));
        }
    }
}
