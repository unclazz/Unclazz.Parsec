using System;
using System.Linq;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class CastParser<T, U> : Parser<U>
    {
        internal CastParser(IParser<T> original)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }
        readonly IParser<T> _original;
        public override ParseResult<U> Parse(ParserInput input)
        {
            return _original.Parse(input).Cast<U>();
        }
        public override string ToString()
        {
            return string.Format("Cast({0}, type = {1})", _original,
                ParsecUtility.TypeToString(typeof(U)));
        }
    }
    sealed class CastParser<T> : Parser
    {
        internal CastParser(IParser<T> original)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }
        readonly IParser<T> _original;
        public override ParseResult<X> Parse(ParserInput input)
        {
            return _original.Parse(input).Cast<X>();
        }
        public override string ToString()
        {
            return string.Format("Cast({0})", _original);
        }
    }
}
