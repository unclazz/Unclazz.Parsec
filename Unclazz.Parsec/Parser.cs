using Unclazz.Parsec.CoreParsers;

namespace Unclazz.Parsec
{
    /// <summary>
    /// <see cref="Nil"/>を読み取り結果型として宣言する<see cref="Parser{T}"/>の特殊な派生型です。
    /// <para>
    /// <see cref="Nil"/>はインスタンスを持ちません。
    /// このパーサー抽象クラスから派生した具象パーサー・クラスは値のキャプチャを一切行いません。
    /// それらのパーサーはパースを行いその結果として<see cref="ParseResult{T}"/>を返しますが、パース結果の成否と関係なく、
    /// <see cref="ParseResult{T}.Capture"/>は必ず空の（値を持たない）インスタンスになります。
    /// </para>
    /// </summary>
    public abstract class Parser : Parser<Nil>
    {
        #region 演算子オーバーロードの宣言
        /// <summary>
        /// <see cref="Parsers.Not(Parser)"/>と同義です。
        /// </summary>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいインスタンス</returns>
        public static Parser operator !(Parser operand)
        {
            return new NotParser<Nil>(operand);
        }
        /// <summary>
        /// <see cref="ParserExtension.Or(Parser, Parser)"/>と同義です。
        /// </summary>
        /// <param name="left">元になるパーサー</param>
        /// <param name="right">元になるパーサー</param>
        /// <returns>新しいインスタンス</returns>
        public static Parser operator |(Parser left, Parser right)
        {
            return OrParser<Nil>.LeftAssoc(left, right).Cast();
        }
        /// <summary>
        /// <see cref="ParserExtension.Then(Parser, Parser)"/>と同義です。
        /// </summary>
        /// <param name="left">元になるパーサー</param>
        /// <param name="right">元になるパーサー</param>
        /// <returns>新しいインスタンス</returns>
        public static Parser operator &(Parser left, Parser right)
        {
            return left.Then(right);
        }
        #endregion
    }
}
