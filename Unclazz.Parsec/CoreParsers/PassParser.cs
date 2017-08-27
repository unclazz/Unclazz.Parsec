namespace Unclazz.Parsec.CoreParsers
{
    sealed class PassParser<T> : Parser<T>
    {
        internal PassParser(IParserConfiguration conf, T value) : base(conf)
        {
            _value = value;
        }

        readonly T _value;

        protected override ParseResult<T> DoParse(Reader input)
        {
            return Success(input.Position, new Optional<T>(_value));
        }

        public override string ToString()
        {
            return string.Format("Pass({0})");
        }
    }
}
