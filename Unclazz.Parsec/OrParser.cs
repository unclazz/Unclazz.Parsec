using System;

namespace Unclazz.Parsec
{
    sealed class OrParser<T> : Parser<T>
    {
        internal OrParser(Parser<T> left, Parser<T> right)
        {
            _left = left ?? throw new ArgumentNullException(nameof(left));
            _right = right ?? throw new ArgumentNullException(nameof(right));
        }

        readonly Parser<T> _left;
        readonly Parser<T> _right;

        public override ParseResult<T> Parse(ParserInput input)
        {
            input.Mark();
            var leftResult = _left.Parse(input);
            if (leftResult.Successful)
            {
                input.Unmark();
                return leftResult;
            }
            else if (!leftResult.CanBacktrack)
            {
                input.Unmark();
                return leftResult.AllowBacktrack(true);
            }
            input.Reset();
            var rightResult = _right.Parse(input);
            input.Unmark();
            return rightResult.AllowBacktrack(true);
        }
        public override string ToString()
        {
            return string.Format("Or({0}, {1})", _left, _right);
        }
    }
}
