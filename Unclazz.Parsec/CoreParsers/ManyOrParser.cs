using System;
using System.Collections.Generic;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class ManyOrParser<T> : Parser<IEnumerable<T>>
    {
        internal ManyOrParser(Parser<IEnumerable<T>> left, Parser<T> right)
        {
            _left = left ?? throw new ArgumentNullException(nameof(left));
            _right = right ?? throw new ArgumentNullException(nameof(right));
        }

        readonly Parser<IEnumerable<T>> _left;
        readonly Parser<T> _right;

        public override ParseResult<IEnumerable<T>> Parse(ParserInput input)
        {
            input.Mark();
            var leftResult = _left.Parse(input);
            if (leftResult.Successful || !leftResult.CanBacktrack)
            {
                input.Unmark();
                return leftResult;
            }
            input.Reset();
            var rightResult = _right.Parse(input);
            input.Unmark();
            return rightResult
                .Map(a => (IEnumerable<T>)new T[] { a })
                .AllowBacktrack(true);
        }
        public override string ToString()
        {
            return string.Format("Or({0}, {1})", _left, _right);
        }
    }
}
