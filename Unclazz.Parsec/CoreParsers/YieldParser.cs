namespace Unclazz.Parsec.CoreParsers
{
    sealed class YieldParser<T> : Parser<T>
    {
        internal YieldParser(IParserConfiguration conf, T value) : base(conf)
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
            return string.Format("Yield({0})");
        }
    }
}
