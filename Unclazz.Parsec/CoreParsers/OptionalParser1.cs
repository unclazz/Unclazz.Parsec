using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class OptionalParser<T> : Parser<Optional<T>>
    {
        internal OptionalParser(Parser<T> original) : this(original.Configuration, original) { }
        internal OptionalParser(IParserConfiguration conf, Parser<T> original) : base(conf)
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
            input.Reset(true);
            return Success(new Optional<T>());
        }
        public override string ToString()
        {
            return string.Format("OrNot({0})", _original);
        }
    }
}
