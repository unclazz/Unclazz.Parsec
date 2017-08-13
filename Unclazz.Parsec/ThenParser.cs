using System;
using System.Collections.Generic;

namespace Unclazz.Parsec
{
    sealed class ThenParser<T> : Parser<IEnumerable<T>>
    {
        internal ThenParser(Parser<T> left, Parser<T> right)
        {
            _left = left ?? throw new ArgumentNullException(nameof(left));
            _right = right ?? throw new ArgumentNullException(nameof(right));
        }

        readonly Parser<T> _left;
        readonly Parser<T> _right;

        public override ParseResult<IEnumerable<T>> Parse(ParserInput input)
        {
            var p = input.Position;
            var leftResult = _left.Parse(input);
            if (leftResult.Successful)
            {
                var rightResult = _right.Parse(input);
                IEnumerable<T> pair = new[] { leftResult.Value, default(T) };
                return rightResult.Successful
                    ? ParseResult.OfSuccess(p, pair)
                    : ParseResult.OfFailure<IEnumerable<T>>(p, rightResult.Message);
            }
            return ParseResult.OfFailure<IEnumerable<T>>(p, leftResult.Message);
        }
    }
}
