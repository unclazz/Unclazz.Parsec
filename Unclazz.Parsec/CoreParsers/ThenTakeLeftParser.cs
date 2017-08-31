using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class ThenTakeLeftParser<TLeft> : Parser<TLeft>
    {
        internal ThenTakeLeftParser(IParserConfiguration conf, Parser<TLeft> left, Parser right) : base(conf)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public Parser<TLeft> Left { get; }
        public Parser Right { get; }

        protected override ResultCore<TLeft> DoParse(Reader input)
        {
            var leftResult = Left.Parse(input);
            if (leftResult.Successful)
            {
                var rightResult = Right.Parse(input);
                var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;

                if (rightResult.Successful)
                {
                    return leftResult.AllowBacktrack(canBacktrack);
                }
                else
                {
                    return Failure(rightResult.Message, canBacktrack);
                }
            }
            return leftResult;
        }
        public override string ToString()
        {
            return string.Format("Then({0}, {1})", Left, Right);
        }
    }
}
