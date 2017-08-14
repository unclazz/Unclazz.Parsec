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
                var q = new Queue<T>();
                leftResult.IfSuccessful(c => c.IfHasValue(q.Enqueue));
                rightResult.IfSuccessful(c => c.IfHasValue(q.Enqueue));
                return rightResult.Successful
                    ? ParseResult.OfSuccess<IEnumerable<T>>(p, q)
                    : ParseResult.OfFailure<IEnumerable<T>>(p, rightResult.Message);
            }
            return ParseResult.OfFailure<IEnumerable<T>>(p, leftResult.Message);
        }
        public override string ToString()
        {
            return string.Format("Then({0}, {1})", _left, _right);
        }
    }
}
