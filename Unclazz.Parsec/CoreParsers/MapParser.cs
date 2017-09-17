using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class MapParser<TSource, TResult> : Parser<TResult>
    {
        internal MapParser(Parser<TSource> source, 
            Func<TSource, TResult> transform, bool canThrow) : base("Map")
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            _canThrow = canThrow;
        }

        readonly Parser<TSource> _source;
        readonly Func<TSource, TResult> _transform;
        readonly bool _canThrow;

        protected override ResultCore<TResult> DoParse(Context ctx)
        {
            var r = _source.Parse(ctx);
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
    }
}
