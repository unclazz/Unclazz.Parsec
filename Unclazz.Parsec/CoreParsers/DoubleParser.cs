using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class DoubleParser<T1, T2> : Parser<Tuple<T1, T2>>
    {
        internal DoubleParser(IParserConfiguration conf, Parser<T1> left, Parser<T2> right) : base(conf)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public Parser<T1> Left { get; }
        public Parser<T2> Right { get; }

        protected override ResultCore<Tuple<T1, T2>> DoParse(Reader input)
        {
            var leftResult = Left.Parse(input);
            if (!leftResult.Successful)
            {
                return leftResult.Retyped<Tuple<T1, T2>>();
            }

            var rightResult = Right.Parse(input);
            var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;
            if (!rightResult.Successful)
            {
                return rightResult.Retyped<Tuple<T1, T2>>().AllowBacktrack(canBacktrack);
            }

            var cap = new Tuple<T1, T2>(leftResult.Value, rightResult.Value);
            return Success(cap, canBacktrack);
        }
        public override string ToString()
        {
            return string.Format("Double({0}, {1})", Left, Right);
        }
    }
}
