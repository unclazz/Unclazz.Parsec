﻿using System;

namespace Unclazz.Parsec.Intrinsics
{
    sealed class OrParser<T> : Parser<T>
    {
        internal OrParser(Parser<T> left, Parser<T> right) : base("Or")
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        internal Parser<T> Left { get; }
        internal Parser<T> Right { get; }

        protected override ResultCore<T> DoParse(Reader src)
        {
            src.Mark();
            var leftResult = Left.Parse(src);
            if (leftResult.Successful || !leftResult.CanBacktrack)
            {
                src.Unmark();
                return leftResult.AllowBacktrack(true);
            }
            src.Reset(true);
            var rightResult = Right.Parse(src);
            return rightResult.AllowBacktrack(true);
        }
    }
    sealed class CharOrParser : CharParser
    {
        internal CharOrParser(CharParser left, CharParser right) : base("Or")
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        internal CharParser Left { get; }
        internal CharParser Right { get; }

        protected override ResultCore DoParse(Reader src) => ParsecUtility.Or(src, Left, Right);
    }
    sealed class OrParser : Parser
    {
        internal OrParser(Parser left, Parser right) : base("Or")
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        internal Parser Left { get; }
        internal Parser Right { get; }

        protected override ResultCore DoParse(Reader src) => ParsecUtility.Or(src, Left, Right);
    }
}
