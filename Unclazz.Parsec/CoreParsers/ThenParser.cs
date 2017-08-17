using System;
using System.Collections.Generic;
using System.Text;

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
                var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;
                leftResult.IfSuccessful(c => c.IfHasValue(q.Enqueue));
                rightResult.IfSuccessful(c => c.IfHasValue(q.Enqueue));
                return rightResult.Successful
                    ? Success(p, new Capture<IEnumerable<T>>(q), canBacktrack)
                    : Failure(p, rightResult.Message, canBacktrack);
            }
            return Failure(p, leftResult.Message, leftResult.CanBacktrack);
        }
        public override string ToString()
        {
            return string.Format("Then({0}, {1})", _left, _right);
        }
    }
    sealed class StringThenParser : Parser<string>
    {
        internal StringThenParser(Parser<string> left, Parser<string> right)
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

                var buff = new StringBuilder();
                leftResult.Capture.IfHasValue(a => buff.Append(a));
                rightResult.IfSuccessful(c => c.IfHasValue(a => buff.Append(a)));

                var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;
                return rightResult.Successful
                    ? Success(p, new Capture<string>(buff.ToString()), canBacktrack)
                    : Failure(p, rightResult.Message, canBacktrack);
            }
            return Failure(p, leftResult.Message, leftResult.CanBacktrack);
        }
        public override string ToString()
        {
            return string.Format("Then({0}, {1})", _left, _right);
        }
    }
    sealed class ThenParser<T,U> : Parser<U>
    {
        internal ThenParser(Parser<T> left, Parser<U> right)
        {
            _left = left ?? throw new ArgumentNullException(nameof(left));
            _right = right ?? throw new ArgumentNullException(nameof(right));
        }

        readonly Parser<T> _left;
        readonly Parser<U> _right;

        public override ParseResult<U> Parse(ParserInput input)
        {
            var p = input.Position;
            var leftResult = _left.Parse(input);
            if (leftResult.Successful)
            {
                var rightResult = _right.Parse(input);
                var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;
                return rightResult.AllowBacktrack(canBacktrack);
            }
            return Failure(p, leftResult.Message, leftResult.CanBacktrack);
        }
        public override string ToString()
        {
            return string.Format("Then({0}, {1})", _left, _right);
        }
    }
}
