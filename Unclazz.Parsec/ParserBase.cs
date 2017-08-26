namespace Unclazz.Parsec
{
    /// <summary>
    /// <see cref="Parser"/>と<see cref="Parser{T}"/>およびその派生クラスで共通するロジックを実装するクラスです。
    /// </summary>
    /// <typeparam name="T">任意の型</typeparam>
    public abstract class ParserBase<T> : IParser<T>
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
        public abstract ParseResult<T> Parse(ParserInput input);

        /// <summary>
        /// パース成功を表す<see cref="ParseResult{T}"/>インスタンスを生成します。
        /// </summary>
        /// <param name="position">パース開始時の文字位置</param>
        /// <param name="canBacktrack">直近の<c>|</c>や<c>Or(...)</c>を
        /// 起点とするバックトラックを有効にするかどうか（デフォルトは<c>true</c>で、バックトラックは有効）</param>
        /// <returns>パース成功を表すインスタンス</returns>
        protected ParseResult<T> Success(CharacterPosition position, bool canBacktrack = true)
        {
            return ParseResult.OfSuccess<T>(position, canBacktrack: canBacktrack);
        }
        /// <summary>
        /// パース成功を表す<see cref="ParseResult{T}"/>インスタンスを生成します。
        /// </summary>
        /// <param name="position">パース開始時の文字位置</param>
        /// <param name="capture">パースされた値を内包する可能性のある<see cref="Optional{T}"/>インスタンス</param>
        /// <param name="canBacktrack">直近の<c>|</c>や<c>Or(...)</c>を
        /// 起点とするバックトラックを有効にするかどうか（デフォルトは<c>true</c>で、バックトラックは有効）</param>
        /// <returns>パース成功を表すインスタンス</returns>
        protected ParseResult<T> Success(CharacterPosition position, Optional<T> capture, bool canBacktrack = true)
        {
            return ParseResult.OfSuccess(position, capture, canBacktrack);
        }
        /// <summary>
        /// パース失敗を表す<see cref="ParseResult{T}"/>インスタンスを生成します。
        /// </summary>
        /// <param name="position">パース開始時の文字位置</param>
        /// <param name="message">パース失敗の理由を示すメッセージ</param>
        /// <param name="canBacktrack">直近の<c>|</c>や<c>Or(...)</c>を
        /// 起点とするバックトラックを有効にするかどうか（デフォルトは<c>true</c>で、バックトラックは有効）</param>
        /// <returns>パース成功を表すインスタンス</returns>
        protected ParseResult<T> Failure(CharacterPosition position, string message, bool canBacktrack = true)
        {
            return ParseResult.OfFailure<T>(position, message, canBacktrack);
        }
    }
}
