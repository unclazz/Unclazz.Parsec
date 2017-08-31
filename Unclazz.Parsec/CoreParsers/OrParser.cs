using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class OrParser<T> : Parser<T>
    {
        internal static OrParser<T> RightAssoc(Parser<T> left, Parser<T> right, params Parser<T>[] others)
        {
            OrParser<T> tmpOr = RightAssoc(left, right);
            foreach (var other in others)
            {
                tmpOr = new OrParser<T>(tmpOr.Configuration, tmpOr.Left,
                    new OrParser<T>(tmpOr.Configuration, tmpOr.Right, right));
            }
            return tmpOr;
        }
        internal static OrParser<T> RightAssoc(Parser<T> left, Parser<T> right)
        {
            var leftOr = left as OrParser<T>;
            if (leftOr == null)
            {
                return new OrParser<T>(left.Configuration, left, right);
            }
            else
            {
                return new OrParser<T>(left.Configuration, leftOr.Left,
                    new OrParser<T>(left.Configuration, leftOr.Right, right));
            }
        }
        internal static OrParser<T> LeftAssoc(Parser<T> left, Parser<T> right, params Parser<T>[] others)
        {
            OrParser<T> tmpOr = new OrParser<T>(left.Configuration, left, right);
            foreach (var other in others)
            {
                tmpOr = new OrParser<T>(tmpOr.Configuration, tmpOr, right);
            }
            return tmpOr;
        }
        internal static OrParser<T> LeftAssoc(Parser<T> left, Parser<T> right)
        {
            return new OrParser<T>(left.Configuration, left, right);
        }


        OrParser(IParserConfiguration conf, Parser<T> left, Parser<T> right) : base(conf)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        internal Parser<T> Left { get; }
        internal Parser<T> Right { get; }

        protected override ResultCore<T> DoParse(Reader input)
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
    sealed class OrParser : Parser
    {
        internal OrParser(IParserConfiguration conf, Parser left, Parser right) : base(conf)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        internal Parser Left { get; }
        internal Parser Right { get; }

        protected override ResultCore DoParse(Reader input)
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
