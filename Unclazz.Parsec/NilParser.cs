using Unclazz.Parsec.CoreParsers;

namespace Unclazz.Parsec
{
    /// <summary>
    /// <see cref="Nil"/>を読み取り結果型として宣言する<see cref="Parser{T}"/>の特殊な派生型です。
    /// <para>
    /// この抽象クラスのから派生した具象パーサー・クラスは値のキャプチャを一切行いません。
    /// パーサーはパースを行いその結果として<see cref="ParseResult{T}"/>を返しますが、
    /// パース結果の成否と関係なく、<see cref="ParseResult{T}.Capture"/>は必ず空のシーケンスになります。
    /// </para>
    /// </summary>
    public abstract class NilParser : Parser<Nil>
    {
        #region 演算子オーバーロードの宣言
        /// <summary>
        /// <see cref="Parsers.Not(NilParser)"/>と同義です。
        /// </summary>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいインスタンス</returns>
        public static NilParser operator !(NilParser operand)
        {
            return new NotParser<Nil>(operand);
        }
        /// <summary>
        /// <see cref="ParserExtension.Or(NilParser, NilParser)"/>と同義です。
        /// </summary>
        /// <param name="left">元になるパーサー</param>
        /// <param name="right">元になるパーサー</param>
        /// <returns>新しいインスタンス</returns>
        public static NilParser operator |(NilParser left, NilParser right)
        {
            return OrParser<Nil>.LeftAssoc(left, right).Cast();
        }
        /// <summary>
        /// <see cref="ParserExtension.Then(NilParser, NilParser)"/>と同義です。
        /// </summary>
        /// <param name="left">元になるパーサー</param>
        /// <param name="right">元になるパーサー</param>
        /// <returns>新しいインスタンス</returns>
        public static NilParser operator &(NilParser left, NilParser right)
        {
            return left.Then(right);
        }
        #endregion
    }
}
