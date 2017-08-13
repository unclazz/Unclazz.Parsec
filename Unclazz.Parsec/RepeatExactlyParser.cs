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
            var q = new Queue<T>();
            for (var i = 0; i < _exactly; i++)
            {
                var r = _original.Parse(input);
                if (r.Successful) q.Enqueue(r.Value);
                else ParseResult.OfFailure<IEnumerable<T>>(r.Position, r.Message);
            }
            return ParseResult.OfSuccess<IEnumerable<T>>(p, q);
        }
    }
}
