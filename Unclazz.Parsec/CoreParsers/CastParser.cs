using System;
using System.Linq;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class CastParser<T, U> : Parser<U>
    {
        internal CastParser(IParser<T> original)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
            _hasDefault = false;
        }
        internal CastParser(IParser<T> original, U defaultValue)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
            _hasDefault = true;
            _default = defaultValue;
        }
        readonly IParser<T> _original;
        readonly bool _hasDefault;
        readonly U _default;
        public override ParseResult<U> Parse(Reader input)
        {
            return _hasDefault 
                ? _original.Parse(input).Cast<U>().Attach(_default) 
                :  _original.Parse(input).Cast<U>();
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
        public override ParseResult<Nil> Parse(Reader input)
        {
            return _original.Parse(input).Cast<Nil>();
        }
        public override string ToString()
        {
            return string.Format("Cast({0})", _original);
        }
    }
}
