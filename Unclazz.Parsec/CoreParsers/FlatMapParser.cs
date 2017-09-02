using System;
using System.Linq;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class FlatMapParser<TSource, TResult> : Parser<TResult>
    {
        internal FlatMapParser(IParserConfiguration conf, Parser<TSource> source,
            Func<TSource, Parser<TResult>> mapper, bool canThrow) : base(conf)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _canThrow = canThrow;
        }

        readonly Parser<TSource> _source;
        readonly Func<TSource, Parser<TResult>> _mapper;
        readonly bool _canThrow;

        protected override ResultCore<TResult> DoParse(Reader input)
        {
            var r = _source.Parse(input);
            try
            {
                if (r.Successful)
                {
                    return _mapper(r.Capture).Parse(input);
                }
                return r.Retyped<TResult>();
            }
            catch (Exception ex)
            {
                if (_canThrow) throw;
                return Failure(ex.ToString(), r.CanBacktrack);
            }
        }
        public override string ToString()
        {
            return string.Format("FlatMap({0}, mapper = {1})",
                _source, ParsecUtility.ObjectTypeToString(_mapper));
        }
    }
    sealed class FlatMapParser<TSource> : Parser
    {
        internal FlatMapParser(IParserConfiguration conf, Parser<TSource> source,
            Func<TSource, Parser> mapper, bool canThrow) : base(conf)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _canThrow = canThrow;
        }

        readonly Parser<TSource> _source;
        readonly Func<TSource, Parser> _mapper;
        readonly bool _canThrow;

        protected override ResultCore DoParse(Reader input)
        {
            var r = _source.Parse(input);
            try
            {
                if (r.Successful)
                {
                    return _mapper(r.Capture).Parse(input);
                }
                return r.Untyped();
            }
            catch (Exception ex)
            {
                if (_canThrow) throw;
                return Failure(ex.ToString(), r.CanBacktrack);
            }
        }
        public override string ToString()
        {
            return string.Format("FlatMap({0}, mapper = {1})",
                _source, ParsecUtility.ObjectTypeToString(_mapper));
        }
    }
}
