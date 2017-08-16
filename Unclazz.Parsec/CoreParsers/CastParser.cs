using System;
using System.Linq;

namespace Unclazz.Parsec
{
    sealed class CastParser<T, U> : Parser<U>
    {
        internal CastParser(Parser<T> original)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }
        readonly Parser<T> _original;
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
}
