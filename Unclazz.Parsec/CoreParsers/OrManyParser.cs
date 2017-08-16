﻿using System;
using System.Collections.Generic;

namespace Unclazz.Parsec
{
    sealed class OrManyParser<T> : Parser<IEnumerable<T>>
    {
        internal OrManyParser(Parser<T> left, Parser<IEnumerable<T>> right)
        {
            _left = left ?? throw new ArgumentNullException(nameof(left));
            _right = right ?? throw new ArgumentNullException(nameof(right));
        }

        readonly Parser<T> _left;
        readonly Parser<IEnumerable<T>> _right;

        public override ParseResult<IEnumerable<T>> Parse(ParserInput input)
        {
            input.Mark();
            var leftResult = _left.Parse(input);
            if (leftResult.Successful || !leftResult.CanBacktrack)
            {
                input.Unmark();
                return leftResult.Map(a => (IEnumerable<T>)new T[] { a });
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