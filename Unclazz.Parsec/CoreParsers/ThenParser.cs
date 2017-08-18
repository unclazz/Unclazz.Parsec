using System;

namespace Unclazz.Parsec.CoreParsers
{
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
