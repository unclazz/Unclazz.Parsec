namespace Unclazz.Parsec.CoreParsers
{
    sealed class YieldParser<T> : Parser<T>
    {
        internal YieldParser(IParserConfiguration conf, T value) : base(conf)
        {
            _value = value;
        }

        readonly T _value;

        protected override ResultCore<T> DoParse(Reader input)
        {
            return Success(_value);
        }

        public override string ToString()
        {
            return string.Format("Yield({0})");
        }
    }
}
