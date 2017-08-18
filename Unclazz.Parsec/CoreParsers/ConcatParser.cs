using System;
using System.Collections.Generic;
using System.Text;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class ConcatParser : Parser<string>
    {
        internal ConcatParser(Parser<string> left, Parser<string> right)
        {
            _left = left ?? throw new ArgumentNullException(nameof(left));
            _right = right ?? throw new ArgumentNullException(nameof(right));
        }

        readonly Parser<string> _left;
        readonly Parser<string> _right;

        public override ParseResult<string> Parse(ParserInput input)
        {
            var p = input.Position;
            var leftResult = _left.Parse(input);
            if (leftResult.Successful)
            {
                var rightResult = _right.Parse(input);
                var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;

                var buff = new StringBuilder();
                leftResult.Capture.IfHasValue(a => buff.Append(a));
                rightResult.IfSuccessful(c => c.IfHasValue(a => buff.Append(a)));

                return rightResult.Successful
                    ? Success(p, new Capture<string>(buff.ToString()), canBacktrack)
                    : Failure(p, rightResult.Message, canBacktrack);
            }
            return Failure(p, leftResult.Message, leftResult.CanBacktrack);
        }
        public override string ToString()
        {
            return string.Format("Concat({0}, {1})", _left, _right);
        }
    }
    sealed class EnumerableConcatParser<T> : Parser<IEnumerable<T>>
    {
        internal EnumerableConcatParser(Parser<IEnumerable<T>> left, Parser<IEnumerable<T>> right)
        {
            _left = left ?? throw new ArgumentNullException(nameof(left));
            _right = right ?? throw new ArgumentNullException(nameof(right));
        }

        readonly Parser<IEnumerable<T>> _left;
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
                    var leftCap = leftResult.Capture;
                    var q = leftCap.HasValue ? new Queue<T>(leftCap.Value) : new Queue<T>();
                    rightResult.Capture.IfHasValue(es => {
                        foreach (var e in es) q.Enqueue(e);
                    });
                    return Success(p, new Capture<IEnumerable<T>>(q), canBacktrack);
                }

                return rightResult.AllowBacktrack(canBacktrack);
            }
            return Failure(p, leftResult.Message, leftResult.CanBacktrack);
        }
        public override string ToString()
        {
            return string.Format("Concat({0}, {1})", _left, _right);
        }
    }
}
