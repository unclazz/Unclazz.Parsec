namespace Unclazz.Parsec.Intrinsics
{
    /// <summary>
    /// EOFにのみパッチするパーサーです。
    /// このパーサーは文字位置の変更を行いません。
    /// </summary>
    sealed class EndOfFileParser : Parser
    {
        internal EndOfFileParser() : base("EOF") { }
        protected override ResultCore DoParse(Reader src)
        {
            return src.EndOfFile ? Success() 
                : Failure(string.Format("EOF expected but found {0}.", 
                ParsecUtility.CharToString(src.Peek())));
        }
    }
}
