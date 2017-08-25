namespace Unclazz.Parsec.CoreParsers
{
    sealed class PassParser<T> : Parser<T>
    {
        internal PassParser(T value)
        {
            _value = value;
        }

        readonly T _value;

        public override ParseResult<T> Parse(ParserInput input)
        {
            return Success(input.Position, new Capture<T>(_value));
        }

        public override string ToString()
        {
            return string.Format("Pass({0})");
        }
    }
}
