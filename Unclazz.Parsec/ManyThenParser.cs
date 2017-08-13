using System;
using System.Collections.Generic;

namespace Unclazz.Parsec
{
    sealed class ManyThenParser<T> : Parser<IEnumerable<T>>
    {
        internal ManyThenParser(Parser<IEnumerable<T>> left, Parser<T> right)
        {
            _left = left ?? throw new ArgumentNullException(nameof(left));
            _right = right ?? throw new ArgumentNullException(nameof(right));
        }

        readonly Parser<IEnumerable<T>> _left;
        readonly Parser<T> _right;

        public override ParseResult<IEnumerable<T>> Parse(ParserInput input)
        {
            var p = input.Position;
            var leftResult = _left.Parse(input);
            if (leftResult.Successful)
            {
                var rightResult = _right.Parse(input);
                if (rightResult.Successful)
                {
                    var q = new Queue<T>(leftResult.Value);
                    q.Enqueue(rightResult.Value);
                    return ParseResult.OfSuccess<IEnumerable<T>>(p, q);
                }
                return ParseResult.OfFailure<IEnumerable<T>>(p, rightResult.Message);
            }
            return ParseResult.OfFailure<IEnumerable<T>>(p, leftResult.Message);
        }
    }
}
