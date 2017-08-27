using System;

namespace Unclazz.Parsec.CoreParsers
{
    abstract class TripleParser<T1,T2,T3>: Parser<Tuple<T1, T2, T3>>
    {
        public static TripleParser<T1,T2,T3> Create(Parser<T1> left, Parser<Tuple<T2, T3>> right)
        {
            return new ThenDoubleParser<T1, T2, T3>(left.Configuration, left, right);
        }
        public static TripleParser<T1,T2,T3> Create(Parser<Tuple<T1, T2>> left, Parser<T3> right)
        {
            return new DoubleThenParser<T1, T2, T3>(left.Configuration, left, right);
        }

        internal TripleParser(IParserConfiguration conf) : base(conf) { }

        sealed class ThenDoubleParser<T1, T2, T3> : TripleParser<T1, T2, T3>
        {
            internal ThenDoubleParser(IParserConfiguration conf, Parser<T1> left, Parser<Tuple<T2, T3>> right) : base(conf)
            {
                Left = left ?? throw new ArgumentNullException(nameof(left));
                Right = right ?? throw new ArgumentNullException(nameof(right));
            }

            public Parser<T1> Left { get; }
            public Parser<Tuple<T2, T3>> Right { get; }

            protected override ParseResult<Tuple<T1, T2, T3>> DoParse(Reader input)
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
        sealed class DoubleThenParser<T1, T2, T3> : TripleParser<T1, T2, T3>
        {
            internal DoubleThenParser(IParserConfiguration conf, Parser<Tuple<T1, T2>> left, Parser<T3> right) : base(conf)
            {
                Left = left ?? throw new ArgumentNullException(nameof(left));
                Right = right ?? throw new ArgumentNullException(nameof(right));
            }

            public Parser<Tuple<T1, T2>> Left { get; }
            public Parser<T3> Right { get; }

            protected override ParseResult<Tuple<T1, T2, T3>> DoParse(Reader input)
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
}
