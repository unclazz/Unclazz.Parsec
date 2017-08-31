using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class ThenTakeRightParser<TRight> : Parser<TRight>
    {
        internal ThenTakeRightParser(IParserConfiguration conf, Parser left, Parser<TRight> right) :base(conf)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public Parser Left { get; }
        public Parser<TRight> Right { get; }

        protected override ResultCore<TRight> DoParse(Reader input)
        {
            var leftResult = Left.Parse(input);
            if (leftResult.Successful)
            {
                var rightResult = Right.Parse(input);
                var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;

                return rightResult.AllowBacktrack(canBacktrack);
            }
            return leftResult.Cast<TRight>();
        }
        public override string ToString()
        {
            return string.Format("Then({0}, {1})", Left, Right);
        }
    }
}
