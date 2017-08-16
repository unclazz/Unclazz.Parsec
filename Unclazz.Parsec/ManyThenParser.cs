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
                var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;
                if (rightResult.Successful)
                {
                    var leftCapture = leftResult.Capture;
                    var q = leftCapture.HasValue ? new Queue<T>(leftCapture.Value) : new Queue<T>();
                    rightResult.Capture.IfHasValue(q.Enqueue);
                    return ParseResult.OfSuccess<IEnumerable<T>>(p, q, canBacktrack);
                }
                return ParseResult.OfFailure<IEnumerable<T>>(p, rightResult.Message, canBacktrack);
            }
            return ParseResult.OfFailure<IEnumerable<T>>(p, leftResult.Message, leftResult.CanBacktrack);
        }
        public override string ToString()
        {
            return string.Format("Then({0}, {1})", _left, _right);
        }
    }
}
