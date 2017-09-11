namespace Unclazz.Parsec.CoreParsers
{
    /// <summary>
    /// 文字にマッチするパーサーです。
    /// </summary>
    sealed class ExactCharParser : CharParser
    {
        internal ExactCharParser(IParserConfiguration conf, char ch) : base(conf)
        {
            Character = ch;
        }
        internal char Character { get; }
        /// <summary>
        /// パースを行います。
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override ResultCore DoParse(Reader input)
        {
            var actual = input.Read();
            if (Character == actual)
            {
                return Success();
            }
            else
            {
                return Failure(string.Format("expected {0} but found {1}.",
                    ParsecUtility.CharToString(Character), ParsecUtility.CharToString(actual)));
            }
        }
        /// <summary>
        /// このインスタンスの文字列表現を返します。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Char({0})", ParsecUtility.CharToString(Character));
        }
    }
}
