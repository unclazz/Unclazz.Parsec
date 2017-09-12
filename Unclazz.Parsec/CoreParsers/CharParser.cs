namespace Unclazz.Parsec.CoreParsers
{
    /// <summary>
    /// 文字にマッチするパーサーです。
    /// </summary>
    public abstract class CharParser : Parser
    {
        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="conf"></param>
        protected CharParser(IParserConfiguration conf) : base(conf) { }
        /// <summary>
        /// 値をキャプチャするパーサーを返します。
        /// </summary>
        /// <returns></returns>
        public new CharCaptureParser Capture()
        {
            return new CharCaptureParser(this);
        }
    }
}
