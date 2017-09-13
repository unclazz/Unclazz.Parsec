namespace Unclazz.Parsec.CoreParsers
{
    /// <summary>
    /// 文字にマッチしその値をキャプチャするパーサーです。
    /// </summary>
    sealed class CharCaptureParser : Parser<char>
    {
        internal CharCaptureParser(CharParser original) : base(original.Configuration)
        {
            _original = original;
        }
        readonly CharParser _original;
        /// <summary>
        /// パースを行います。
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override ResultCore<char> DoParse(Reader input)
        {
            var ch = input.Peek();
            var res = _original.Parse(input);
            return res.Successful ? Success((char)ch) : Failure(res.Message);
        }
        /// <summary>
        /// このインスタンスの文字列表現を返します。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Capture({0})", _original);
        }
    }
}
