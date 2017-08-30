using System;
using System.Linq;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class FlatMapParser<T, U> : Parser<U>
    {
        internal FlatMapParser(IParserConfiguration conf, Parser<T> source, 
            Func<T, Parser<U>> mapper, bool canThrow) : base(conf)
        {
            _source = source ?? throw new ArgumentNullException(nameof(source));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _canThrow = canThrow;
        }

        readonly Parser<T> _source;
        readonly Func<T, Parser<U>> _mapper;
        readonly bool _canThrow;

        protected override ParseResult<U> DoParse(Reader input)
        {
            var r = _source.Parse(input);
            try
            {
                if (r.Successful && r.Capture.Present)
                {
                    return _mapper(r.Capture.Value).Parse(input);
                }
                return r.Cast<U>();
            }
            catch (Exception ex)
            {
                if (_canThrow) throw;
                return Failure(r.Position, ex.ToString(), r.CanBacktrack);
            }
        }
        public override string ToString()
        {
            return string.Format("FlatMap({0}, transform = {1})",
                _source, ParsecUtility.ObjectTypeToString(_mapper));
        }
    }
}
