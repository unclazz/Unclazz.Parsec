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

        sealed class ThenDoubleParser<U1, U2, U3> : TripleParser<U1, U2, U3>
        {
            internal ThenDoubleParser(IParserConfiguration conf, Parser<U1> left, Parser<Tuple<U2, U3>> right) : base(conf)
            {
                Left = left ?? throw new ArgumentNullException(nameof(left));
                Right = right ?? throw new ArgumentNullException(nameof(right));
            }

            public Parser<U1> Left { get; }
            public Parser<Tuple<U2, U3>> Right { get; }

            protected override ResultCore<Tuple<U1, U2, U3>> DoParse(Reader input)
            {
                var leftResult = Left.Parse(input);
                if (!leftResult.Successful)
                {
                    return leftResult.Cast<Tuple<U1, U2, U3>>();
                }

                var rightResult = Right.Parse(input);
                var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;
                if (!rightResult.Successful)
                {
                    return rightResult.Cast<Tuple<U1, U2, U3>>().AllowBacktrack(canBacktrack);
                }

                var rightTuple = rightResult.Value;
                var finalTuple = new Tuple<U1, U2, U3>(leftResult.Value,
                    rightTuple == null ? default(U2) : rightTuple.Item1,
                    rightTuple == null ? default(U3) : rightTuple.Item2);
                return Success(finalTuple, canBacktrack);
            }
            public override string ToString()
            {
                return string.Format("Triple({0}, {1})", Left, Right);
            }
        }
        sealed class DoubleThenParser<U1, U2, U3> : TripleParser<U1, U2, U3>
        {
            internal DoubleThenParser(IParserConfiguration conf, Parser<Tuple<U1, U2>> left, Parser<U3> right) : base(conf)
            {
                Left = left ?? throw new ArgumentNullException(nameof(left));
                Right = right ?? throw new ArgumentNullException(nameof(right));
            }

            public Parser<Tuple<U1, U2>> Left { get; }
            public Parser<U3> Right { get; }

            protected override ResultCore<Tuple<U1, U2, U3>> DoParse(Reader input)
            {
                var leftResult = Left.Parse(input);
                if (!leftResult.Successful)
                {
                    return leftResult.Cast<Tuple<U1, U2, U3>>();
                }

                var rightResult = Right.Parse(input);
                var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;
                if (!rightResult.Successful)
                {
                    return rightResult.Cast<Tuple<U1, U2, U3>>().AllowBacktrack(canBacktrack);
                }

                var leftTuple = leftResult.Value;
                var finalTuple = new Tuple<U1, U2, U3>(
                    leftTuple == null ? default(U1) : leftTuple.Item1,
                    leftTuple == null ? default(U2) : leftTuple.Item2,
                    rightResult.Value);
                return Success(finalTuple, canBacktrack);
            }
            public override string ToString()
            {
                return string.Format("Triple({0}, {1})", Left, Right);
            }
        }
    }
}
