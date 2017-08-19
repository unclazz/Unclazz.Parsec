namespace Unclazz.Parsec.CoreParsers
{
    sealed class CharParser : Parser
    {
        internal CharParser(char ch)
        {
            _ch = ch;
        }
        readonly char _ch;
        public override ParseResult<X> Parse(ParserInput input)
        {
            var p = input.Position;
            var actual = input.Read();
            if (_ch == actual)
            {
                return Success(p);
            }
            else
            {
                return Failure(p, string.Format("expected {0} but found {1}.",
                    ParsecUtility.CharToString(_ch), ParsecUtility.CharToString(actual)));
            }
        }
        public override string ToString()
        {
            return string.Format("Char({0})", ParsecUtility.CharToString(_ch));
        }
    }
}
