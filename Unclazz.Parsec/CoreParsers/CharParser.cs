namespace Unclazz.Parsec.CoreParsers
{
    sealed class CharParser : Parser
    {
        internal CharParser(IParserConfiguration conf, char ch) : base(conf)
        {
            _ch = ch;
        }
        readonly char _ch;
        protected override ParseResult<Nil> DoParse(Reader input)
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
