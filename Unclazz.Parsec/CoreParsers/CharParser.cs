namespace Unclazz.Parsec.CoreParsers
{
    sealed class CharParser : Parser
    {
        internal CharParser(IParserConfiguration conf, char ch) : base(conf)
        {
            _ch = ch;
        }
        readonly char _ch;
        protected override ResultCore DoParse(Reader input)
        {
            var actual = input.Read();
            if (_ch == actual)
            {
                return Success();
            }
            else
            {
                return Failure(string.Format("expected {0} but found {1}.",
                    ParsecUtility.CharToString(_ch), ParsecUtility.CharToString(actual)));
            }
        }
        public override string ToString()
        {
            return string.Format("Char({0})", ParsecUtility.CharToString(_ch));
        }
    }
}
