namespace Unclazz.Parsec
{
    sealed class SuccessParser<T> : Parser<T>
    {
        internal SuccessParser(T value)
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
            return string.Format("Success({0})");
        }
    }
}
