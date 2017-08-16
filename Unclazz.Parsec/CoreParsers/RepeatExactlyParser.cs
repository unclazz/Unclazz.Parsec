using System;
using System.Collections.Generic;

namespace Unclazz.Parsec
{
    sealed class RepeatExactlyParser<T> : Parser<IEnumerable<T>>
    {
        internal RepeatExactlyParser(Parser<T> original, int exactly)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
            if (exactly < 2) throw new ArgumentOutOfRangeException(nameof(exactly));
            _exactly = exactly;
        }

        readonly Parser<T> _original;
        readonly int _exactly;

        public override ParseResult<IEnumerable<T>> Parse(ParserInput input)
        {
            var p = input.Position;
            for (var i = 0; i < _exactly; i++)
            {
                var r = _original.Parse(input);
                if (!r.Successful)
                {
                    return Failure(r.Position, r.Message);
                }
            }
            return Success(p);
        }
        public override string ToString()
        {
            return string.Format("Repeat({0}, exactly = {1})", _original, _exactly);
        }
    }
}
