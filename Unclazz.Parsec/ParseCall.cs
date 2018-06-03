namespace Unclazz.Parsec
{
    /// <summary>
    /// パーサーの呼び出し階層を表すクラスです。
    /// </summary>
    public sealed class ParseCall
    {
        internal ParseCall(string parserName, CharPosition position, int depth)
        {
            ParserName = parserName;
            Position = position;
            Depth = depth;
        }

        string _toStringCache;

        /// <summary>
        /// パーサー名です。
        /// </summary>
        /// <value></value>
        public string ParserName { get; }
        /// <summary>
        /// パーサーの呼び出しが行われた時点の読み取り位置です。
        /// </summary>
        /// <value></value>
        public CharPosition Position { get; }
        /// <summary>
        /// 呼び出し階層の深さです。
        /// </summary>
        /// <value></value>
        public int Depth { get; }
        /// <summary>
        /// このオブジェクトの文字列表現を返します。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return _toStringCache ?? 
                (_toStringCache = string.Format("{0} at (ln={1}, col={2}, idx={3})",
                 ParserName, Position.Line, Position.Column, Position.Index));
        }
    }
}
