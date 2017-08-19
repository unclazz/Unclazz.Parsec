using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class RelayParser<T, U> : Parser<T>
    {
        internal RelayParser(Parser<T> left, Parser<U> right)
        {
            _left = left ?? throw new ArgumentNullException(nameof(left));
            _right = right ?? throw new ArgumentNullException(nameof(right));
        }

        readonly Parser<T> _left;
        readonly Parser<U> _right;

        public override ParseResult<T> Parse(ParserInput input)
        {
            var p = input.Position;
            var leftResult = _left.Parse(input);
            if (leftResult.Successful)
            {
                var rightResult = _right.Parse(input);
                var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;

                if (rightResult.Successful)
                {
                    return Success(p, capture: leftResult.Capture, canBacktrack: canBacktrack);
                }
                return rightResult.AllowBacktrack(canBacktrack).Cast<T>();
            }
            return leftResult;
        }
        public override string ToString()
        {
            return string.Format("Relay({0}, {1})", _left, _right);
        }
    }
}
