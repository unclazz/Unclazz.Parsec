namespace Unclazz.Parsec.Intrinsics
{
    /// <summary>
    /// 文字にマッチするパーサーです。
    /// </summary>
    sealed class ExactCharParser : CharParser
    {
        internal ExactCharParser(char ch) : base("Char")
        {
            _expected = ch;
        }
        readonly char _expected;
        /// <summary>
        /// パースを行います。
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        protected override ResultCore DoParse(Context ctx)
        {
            var actual = ctx.Source.Read();
            if (_expected == actual)
            {
                return Success();
            }
            else
            {
                return Failure(string.Format("expected {0} but found {1}.",
                    ParsecUtility.CharToString(_expected), ParsecUtility.CharToString(actual)));
            }
        }
    }
}
