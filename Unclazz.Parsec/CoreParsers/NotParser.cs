using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class NotParser<T> : Parser
    {
        internal NotParser(IParserConfiguration conf, Parser<T> original) : base(conf)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }

        readonly Parser<T> _original;

        protected override ResultCore DoParse(Reader input)
        {
            input.Mark();
            var p = input.Position;
            var originalResult = _original.Parse(input);
            if (originalResult.Successful)
            {
                var m = string.Format("parsing with {0} must be failed but actualy be successful.", _original);
                return Failure(m, originalResult.CanBacktrack);
            }
            else
            {
                input.Reset();
                input.Unmark();
                return Success(originalResult.CanBacktrack);
            }
        }

        public override string ToString()
        {
            return string.Format("Not({0})", _original);
        }
    }
    sealed class NotParser : Parser
    {
        internal NotParser(IParserConfiguration conf, Parser original) : base(conf)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }

        readonly Parser _original;

        protected override ResultCore DoParse(Reader input)
        {
            input.Mark();
            var originalResult = _original.Parse(input);
            if (originalResult.Successful)
            {
                var m = string.Format("parsing with {0} must be failed but actualy be successful.", _original);
                return Failure(m, originalResult.CanBacktrack);
            }
            else
            {
                input.Reset();
                input.Unmark();
                return Success(originalResult.CanBacktrack);
            }
        }

        public override string ToString()
        {
            return string.Format("Not({0})", _original);
        }
    }
}
