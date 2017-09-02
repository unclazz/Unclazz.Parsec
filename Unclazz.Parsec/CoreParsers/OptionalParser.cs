using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class OptionalParser : Parser
    {
        internal OptionalParser(Parser original) : this(original.Configuration, original) { }
        internal OptionalParser(IParserConfiguration conf, Parser original) : base(conf)
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
            input.Reset(true);
            return Success();
        }
        public override string ToString()
        {
            return string.Format("OrNot({0})", _original);
        }
    }
}
