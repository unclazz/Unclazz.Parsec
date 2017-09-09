namespace Unclazz.Parsec.CoreParsers
{
    /// <summary>
    /// EOFにのみパッチするパーサーです。
    /// このパーサーは文字位置の変更を行いません。
    /// </summary>
    sealed class EndOfFileParser : Parser
    {
        internal EndOfFileParser(IParserConfiguration conf) : base(conf) { }
        protected override ResultCore DoParse(Reader input)
        {
            return input.EndOfFile ? Success() 
                : Failure(string.Format("EOF expected but found {0}.", 
                ParsecUtility.CharToString(input.Peek())));
        }
        public override string ToString()
        {
            return string.Format("EndOfFile()");
        }
    }
}
