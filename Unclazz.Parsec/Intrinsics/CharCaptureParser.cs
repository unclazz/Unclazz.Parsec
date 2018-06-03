namespace Unclazz.Parsec.Intrinsics
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
        /// <param name="src"></param>
        /// <returns></returns>
        protected override ResultCore<char> DoParse(Reader src)
        {
            var ch = src.Peek();
            var res = _original.Parse(src);
            return res.Successful ? Success((char)ch) : Failure(res.Message);
        }
    }
}
