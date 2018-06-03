﻿using System;
using System.Linq;

namespace Unclazz.Parsec.Intrinsics
{
    sealed class FlatMapParser<TSource, TResult> : Parser<TResult>
    {
        internal FlatMapParser(Parser<TSource> source, Func<TSource, Parser<TResult>> mapper, bool canThrow) : base("FlatMap")
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _canThrow = canThrow;
        }

        readonly Parser<TSource> _source;
        readonly Func<TSource, Parser<TResult>> _mapper;
        readonly bool _canThrow;

        protected override ResultCore<TResult> DoParse(Reader src)
        {
            var r = _source.Parse(src);
            try
            {
                if (r.Successful)
                {
                    return _mapper(r.Capture).Parse(src);
                }
                return r.Retyped<TResult>();
            }
            catch (Exception ex)
            {
                if (_canThrow) throw;
                return Failure(ex.ToString(), r.CanBacktrack);
            }
        }
    }
    sealed class FlatMapParser<TSource> : Parser
    {
        internal FlatMapParser(Parser<TSource> source, Func<TSource, Parser> mapper, bool canThrow) : base("FlatMap")
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _canThrow = canThrow;
        }

        readonly Parser<TSource> _source;
        readonly Func<TSource, Parser> _mapper;
        readonly bool _canThrow;

        protected override ResultCore DoParse(Reader src)
        {
            var r = _source.Parse(src);
            try
            {
                if (r.Successful)
                {
                    return _mapper(r.Capture).Parse(src);
                }
                return r.Untyped();
            }
            catch (Exception ex)
            {
                if (_canThrow) throw;
                return Failure(ex.ToString(), r.CanBacktrack);
            }
        }
    }
}
