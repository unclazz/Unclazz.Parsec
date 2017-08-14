using System;
using System.Collections.Generic;

namespace Unclazz.Parsec
{
    sealed class ThenManyParser<T> : Parser<IEnumerable<T>>
    {
        internal ThenManyParser(Parser<T> right, Parser<IEnumerable<T>> left)
        {
            _left = right ?? throw new ArgumentNullException(nameof(right));
            _right = left ?? throw new ArgumentNullException(nameof(left));
        }

        readonly Parser<T> _left;
        readonly Parser<IEnumerable<T>> _right;

        public override ParseResult<IEnumerable<T>> Parse(ParserInput input)
        {
            var p = input.Position;
            var leftResult = _left.Parse(input);
            if (leftResult.Successful)
            {
                var rightResult = _right.Parse(input);
                if (rightResult.Successful)
                {
                    var q = new Queue<T>();
                    leftResult.Capture.IfHasValue(q.Enqueue);
                    rightResult.Capture.IfHasValue(es =>
                    {
                        foreach (var e in es) q.Enqueue(e);
                    });
                    return ParseResult.OfSuccess<IEnumerable<T>>(p, q);
                }
                return ParseResult.OfFailure<IEnumerable<T>>(p, rightResult.Message);
            }
            return ParseResult.OfFailure<IEnumerable<T>>(p, leftResult.Message);
        }
    }
}
