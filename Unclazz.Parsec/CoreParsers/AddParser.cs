using System;
using System.Collections.Generic;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class AdditiveParser<T> : Parser<T>
    {
        internal AdditiveParser(Parser<T> left, Parser<T> right)
        {
            Left = left;
            Right = right;
        }

        public Parser<T> Left { get; }
        public Parser<T> Right { get; }

        public override ParseResult<T> Parse(ParserInput input)
        {
            var p = input.Position;
            var leftResult = Left.Parse(input);
            if (leftResult.Successful)
            {
                var rightResult = Right.Parse(input);
                var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;

                if (rightResult.Successful)
                {
                    return Success(p, capture: leftResult.Capture + rightResult.Capture, 
                        canBacktrack: canBacktrack);
                }
                else
                {
                    return Failure(p, rightResult.Message, canBacktrack);
                }
            }
            return Failure(p, leftResult.Message, leftResult.CanBacktrack);
        }
    }
}
