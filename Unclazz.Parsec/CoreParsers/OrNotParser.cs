using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class OrNotParser<T> : Parser<Optional<T>>
    {
        internal OrNotParser(IParserConfiguration conf, Parser<T> original) : base(conf)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }

        readonly Parser<T> _original;

        protected override ResultCore<Optional<T>> DoParse(Reader input)
        {
            input.Mark();
            var result = _original.Parse(input);
            if (result.Successful)
            {
                input.Unmark();
                return result.Map(a => new Optional<T>(a));
            }
            input.Reset();
            input.Unmark();
            return Success(new Optional<T>());
        }
        public override string ToString()
        {
            return string.Format("OrNot({0})", _original);
        }
    }
    sealed class OrNotParser : Parser
    {
        internal OrNotParser(IParserConfiguration conf, Parser original) : base(conf)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }

        readonly Parser _original;

        protected override ResultCore DoParse(Reader input)
        {
            input.Mark();
            var result = _original.Parse(input);
            if (result.Successful)
            {
                input.Unmark();
                return result;
            }
            input.Reset();
            input.Unmark();
            return Success();
        }
        public override string ToString()
        {
            return string.Format("OrNot({0})", _original);
        }
    }
}
