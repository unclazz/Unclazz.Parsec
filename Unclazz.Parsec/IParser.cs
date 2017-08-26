namespace Unclazz.Parsec
{
    /// <summary>
    /// パーサーを表すインターフェースです。
    /// <para>
    /// このインターフェースの実装を宣言する2つの抽象クラスとそこから派生した多くの具象クラスが存在しています。
    /// 抽象クラスの1つ<see cref="Parser"/>はパース結果の型が<see cref="Nil"/>であるパーサーです。
    /// <see cref="Parser"/>はパースの成否判定だけを行うパーサーです。
    /// <see cref="Nil"/>は実際にはインスタンスを持たないクラスであり、
    /// パース結果<see cref="ParseResult{T}"/>はその成否にかかわらず常に値を持たないインスタンスです
    /// （<see cref="ParseResult{T}.Capture"/>が空のシーケンスを返す）。
    /// パース結果を文字列やその他の型のインスタンスとして取得する必要がある場合は<see cref="Parser{T}"/>のインスタンスを使用します。
    /// この型のインスタンスは<see cref="Parser.Capture"/>メソッドや<see cref="Parser"/>および<see cref="Parser{T}"/>が公開する
    /// 各種のメンバーを利用して得ることができます。
    /// </para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IParser<T>
    {
        /// <summary>
        /// パースを行います。
        /// <para>
        /// パーサーの具象クラスを実装する場合、このメソッドを実装する必要があります。
        /// パース成否は<see cref="ParseResult{T}"/>のインスタンスで表されます。
        /// このメソッドはいかなる場合も<c>null</c>を返してはなりません。
        /// またこのメソッドは原則として例外スローを行ってはなりません。
        /// 正常・異常を問わずこのメソッド内で起こったことはすべて
        /// <see cref="ParseResult{T}"/>を通じて呼び出し元に通知される必要があります。
        /// </para>
        /// </summary>
        /// <param name="input">入力データ</param>
        /// <returns>パース結果</returns>
        ParseResult<T> Parse(Reader input);
    }
}
