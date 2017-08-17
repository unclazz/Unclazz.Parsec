using System;
using System.Collections.Generic;

namespace Unclazz.Parsec
{
    sealed class ThenManyParser<T> : Parser<IEnumerable<T>>
    {
        internal ThenManyParser(Parser<T> left, Parser<IEnumerable<T>> right)
        {
            _left = left ?? throw new ArgumentNullException(nameof(left));
            _right = right ?? throw new ArgumentNullException(nameof(right));
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
                var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;
                if (rightResult.Successful)
                {
                    var q = new Queue<T>();
                    leftResult.Capture.IfHasValue(q.Enqueue);
                    rightResult.Capture.IfHasValue(es =>
                    {
                        foreach (var e in es) q.Enqueue(e);
                    });
                    return Success(p, new Capture<IEnumerable<T>>(q), canBacktrack);
                }
                return Failure(p, rightResult.Message, canBacktrack);
            }
            return Failure(p, leftResult.Message, leftResult.CanBacktrack);
        }

        public override string ToString()
        {
            return string.Format("Then({0}, {1})", _left, _right);
        }
    }
}
