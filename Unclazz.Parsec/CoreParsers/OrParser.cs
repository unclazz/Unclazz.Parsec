using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class OrParser<T> : Parser<T>
    {
        internal OrParser(Parser<T> left, Parser<T> right) : this(ParserFactory.Default, left, right) { }
        internal OrParser(IParserConfiguration conf, Parser<T> left, Parser<T> right) : base(conf)
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
    sealed class CharOrParser : CharParser
    {
        internal CharOrParser(CharParser left, CharParser right) : base(left.Configuration)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        internal CharParser Left { get; }
        internal CharParser Right { get; }

        protected override ResultCore DoParse(Reader input) => ParsecUtility.Or(input, Left, Right);
        public override string ToString() => string.Format("Or({0}, {1})", Left, Right);
    }
    sealed class OrParser : Parser
    {
        internal OrParser(Parser left, Parser right) : this(ParserFactory.Default, left, right) { }
        internal OrParser(IParserConfiguration conf, Parser left, Parser right) : base(conf)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        internal Parser Left { get; }
        internal Parser Right { get; }

        protected override ResultCore DoParse(Reader input) => ParsecUtility.Or(input, Left, Right);
        public override string ToString() => string.Format("Or({0}, {1})", Left, Right);
    }
}
