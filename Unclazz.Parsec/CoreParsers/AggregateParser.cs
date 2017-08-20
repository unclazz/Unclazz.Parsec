using System;
using System.Linq;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class AggregateParser<TSource, TAccumulate> : Parser<TAccumulate>
    {
        internal AggregateParser(IParser<TSource> source, TAccumulate seed, 
            Func<TAccumulate, TSource, TAccumulate> func,
            bool canThrow)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _func = func ?? throw new ArgumentNullException(nameof(func));
            _canThrow = canThrow;
            _seed = seed;
        }

        readonly IParser<TSource> _source;
        readonly TAccumulate _seed;
        readonly Func<TAccumulate,TSource,TAccumulate> _func;
        readonly bool _canThrow;

        public override ParseResult<TAccumulate> Parse(ParserInput input)
        {
            var r = _source.Parse(input);
            try
            {
                if (r.Successful)
                {
                    var cap = Capture<TAccumulate>.Of(r.Capture.Aggregate(_seed, _func));
                    return Success(r.Position, cap, r.CanBacktrack);
                }
            }
            catch (Exception ex)
            {
                if (_canThrow) throw;
                return Failure(r.Position, ex.ToString(), r.CanBacktrack);
            }
            return r.Cast<TAccumulate>();
        }
        public override string ToString()
        {
            return string.Format("Aggregate({0}, seed = {1}, accumulator = {2})",
                _source, _seed, ParsecUtility.ObjectTypeToString(_func));
        }
    }
}
