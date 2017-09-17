using Unclazz.Parsec.CoreParsers;

namespace Unclazz.Parsec
{
    /// <summary>
    /// 文字にマッチするパーサーです。
    /// </summary>
    public abstract class CharParser : Parser
    {
        /// <summary>
        /// <see cref="Or(CharParser)"/>と同義です。
        /// </summary>
        /// <param name="left">左辺</param>
        /// <param name="right">右辺</param>
        /// <returns></returns>
        public static CharParser operator |(CharParser left, CharParser right)
        {
            return left.Or(right);
        }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="name"></param>
        protected CharParser(string name) : base(name) { }
        /// <summary>
        /// 値をキャプチャするパーサーを返します。
        /// </summary>
        /// <returns></returns>
        public new Parser<char> Capture()
        {
            return new CharCaptureParser(this);
        }
        /// <summary>
        /// 左辺（レシーバ）のパーサーが成功すればその結果を、
        /// さもなくば右辺（引数）のパーサーの結果を返すパーサーを返します。
        /// </summary>
        /// <param name="other">右辺のパーサー</param>
        /// <returns></returns>
        public CharParser Or(CharParser other)
        {
            return new CharOrParser(this, other);
        }
    }
}
