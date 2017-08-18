namespace Unclazz.Parsec.CoreParsers
{
    sealed class SingleCharParser : Parser<string>
    {
        internal SingleCharParser(char ch)
        {
            _ch = ch;
        }
        readonly char _ch;
        public override ParseResult<string> Parse(ParserInput input)
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
