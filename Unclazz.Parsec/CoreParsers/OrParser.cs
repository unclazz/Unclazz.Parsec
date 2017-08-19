﻿using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class OrParser<T> : Parser<T>
    {
        internal static OrParser<T> RightAssoc(IParser<T> left, IParser<T> right, params IParser<T>[] others)
        {
            OrParser<T> tmpOr = RightAssoc(left, right);
            foreach (var other in others)
            {
                tmpOr = new OrParser<T>(tmpOr.Left, new OrParser<T>(tmpOr.Right, right));
            }
            return tmpOr;
        }
        internal static OrParser<T> RightAssoc(IParser<T> left, IParser<T> right)
        {
            var leftOr = left as OrParser<T>;
            if (leftOr == null)
            {
                return new OrParser<T>(left, right);
            }
            else
            {
                return new OrParser<T>(leftOr.Left, new OrParser<T>(leftOr.Right, right));
            }
        }
        internal static OrParser<T> LeftAssoc(IParser<T> left, IParser<T> right, params IParser<T>[] others)
        {
            OrParser<T> tmpOr = new OrParser<T>(left, right);
            foreach (var other in others)
            {
                tmpOr = new OrParser<T>(tmpOr, right);
            }
            return tmpOr;
        }
        internal static OrParser<T> LeftAssoc(Parser<T> left, Parser<T> right)
        {
            return new OrParser<T>(left, right);
        }


        OrParser(IParser<T> left, IParser<T> right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        internal IParser<T> Left { get; }
        internal IParser<T> Right { get; }

        public override ParseResult<T> Parse(ParserInput input)
        {
            input.Mark();
            var leftResult = Left.Parse(input);
            if (leftResult.Successful || !leftResult.CanBacktrack)
            {
                input.Unmark();
                return leftResult.AllowBacktrack(true);
            }
            input.Reset();
            var rightResult = Right.Parse(input);
            input.Unmark();
            return rightResult.AllowBacktrack(true);
        }
        public override string ToString()
        {
            return string.Format("Or({0}, {1})", Left, Right);
        }
    }
}
