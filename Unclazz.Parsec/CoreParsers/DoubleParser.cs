using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class DoubleParser<T1, T2> : Parser<Tuple<T1, T2>>
    {
        internal DoubleParser(Parser<T1> left, Parser<T2> right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public Parser<T1> Left { get; }
        public Parser<T2> Right { get; }

        public override ParseResult<Tuple<T1, T2>> Parse(ParserInput input)
        {
            var p = input.Position;
            var leftResult = Left.Parse(input);
            if (!leftResult.Successful)
            {
                return leftResult.Cast<Tuple<T1, T2>>();
            }

            var rightResult = Right.Parse(input);
            var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;
            if (!rightResult.Successful)
            {
                return rightResult.Cast<Tuple<T1, T2>>().AllowBacktrack(canBacktrack);
            }

            var cap = new Tuple<T1, T2>(leftResult.Capture.OrElse(default(T1)), rightResult.Capture.OrElse(default(T2)));
            return Success(p, capture: new Capture<Tuple<T1, T2>>(cap), canBacktrack: canBacktrack);
        }
        public override string ToString()
        {
            return string.Format("Double({0}, {1})", Left, Right);
        }
    }
    sealed class AndDoubleParser<T1, T2, T3> : Parser<Tuple<T1, T2, T3>>
    {
        internal AndDoubleParser(Parser<T1> left, Parser<Tuple<T2, T3>> right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public Parser<T1> Left { get; }
        public Parser<Tuple<T2, T3>> Right { get; }

        public override ParseResult<Tuple<T1, T2, T3>> Parse(ParserInput input)
        {
            var p = input.Position;
            var leftResult = Left.Parse(input);
            if (!leftResult.Successful)
            {
                return leftResult.Cast<Tuple<T1, T2, T3>>();
            }

            var rightResult = Right.Parse(input);
            var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;
            if (!rightResult.Successful)
            {
                return rightResult.Cast<Tuple<T1, T2, T3>>().AllowBacktrack(canBacktrack);
            }

            var rightTuple = rightResult.Capture.OrElse(default(Tuple<T2, T3>));
            var finalTuple = new Tuple<T1, T2, T3>(leftResult.Capture.OrElse(default(T1)),
                rightTuple == null ? default(T2) : rightTuple.Item1,
                rightTuple == null ? default(T3) : rightTuple.Item2);
            return Success(p, finalTuple, canBacktrack: canBacktrack);
        }
        public override string ToString()
        {
            return string.Format("Triple({0}, {1})", Left, Right);
        }
    }
    sealed class DoubleAndParser<T1, T2, T3> : Parser<Tuple<T1, T2, T3>>
    {
        internal DoubleAndParser(Parser<Tuple<T1, T2>> left, Parser<T3> right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public Parser<Tuple<T1, T2>> Left { get; }
        public Parser<T3> Right { get; }

        public override ParseResult<Tuple<T1, T2, T3>> Parse(ParserInput input)
        {
            var p = input.Position;
            var leftResult = Left.Parse(input);
            if (!leftResult.Successful)
            {
                return leftResult.Cast<Tuple<T1, T2, T3>>();
            }

            var rightResult = Right.Parse(input);
            var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;
            if (!rightResult.Successful)
            {
                return rightResult.Cast<Tuple<T1, T2, T3>>().AllowBacktrack(canBacktrack);
            }

            var leftTuple = leftResult.Capture.OrElse(default(Tuple<T1, T2>));
            var finalTuple = new Tuple<T1, T2, T3>(
                leftTuple == null ? default(T1) : leftTuple.Item1,
                leftTuple == null ? default(T2) : leftTuple.Item2,
                rightResult.Capture.OrElse(default(T3)));
            return Success(p, finalTuple, canBacktrack: canBacktrack);
        }
        public override string ToString()
        {
            return string.Format("Triple({0}, {1})", Left, Right);
        }
    }
}
