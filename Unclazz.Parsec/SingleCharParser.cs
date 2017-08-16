namespace Unclazz.Parsec
{
    sealed class SingleCharParser : Parser<char>
    {
        internal SingleCharParser(char ch)
        {
            _ch = ch;
        }
        readonly char _ch;
        public override ParseResult<char> Parse(ParserInput input)
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
                    ParsecUtility.CharString(_ch), ParsecUtility.CharString(actual)));
            }
        }
        public override string ToString()
        {
            return string.Format("Char({0})", ParsecUtility.CharString(_ch));
        }
    }
}
