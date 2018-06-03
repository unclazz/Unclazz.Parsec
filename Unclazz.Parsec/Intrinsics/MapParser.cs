using System;

namespace Unclazz.Parsec.Intrinsics
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

        protected override ResultCore<TResult> DoParse(Reader src)
        {
            var r = _source.Parse(src);
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
    sealed class MapParser<TResult> : Parser<TResult>
    {
        internal MapParser(Parser source, Func<string, TResult> transform, bool canThrow) : base("Map")
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            _canThrow = canThrow;
        }

        readonly Parser _source;
        readonly Func<string, TResult> _transform;
        readonly bool _canThrow;

        protected override ResultCore<TResult> DoParse(Reader src)
        {
            src.Mark();
            var r = _source.Parse(src);
            if (r.Successful)
            {
                try
                {
                    return r.Typed(_transform(src.Capture(true)));
                }
                catch (Exception ex)
                {
                    if (_canThrow) throw;
                    return Failure(ex.ToString(), r.CanBacktrack);
                }
            }
            src.Unmark();
            return r.Typed<TResult>();
        }
    }
}
