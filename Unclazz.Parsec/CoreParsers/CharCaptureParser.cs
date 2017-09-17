namespace Unclazz.Parsec.CoreParsers
{
    /// <summary>
    /// 文字にマッチしその値をキャプチャするパーサーです。
    /// </summary>
    sealed class CharCaptureParser : Parser<char>
    {
        internal CharCaptureParser(CharParser original) : base("Or")
        {
            _original = original;
        }
        readonly CharParser _original;
        /// <summary>
        /// パースを行います。
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override ResultCore<char> DoParse(Context ctx)
        {
            var ch = ctx.Source.Peek();
            var res = _original.Parse(ctx);
            return res.Successful ? Success((char)ch) : Failure(res.Message);
        }
    }
}
