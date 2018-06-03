using System;

namespace Unclazz.Parsec
{
    /// <summary>
    /// <see cref="Parser"/>の派生クラスのパース結果中核部となる構造体です。
    /// <para>
    /// <see cref="Parser"/>実装者は抽象メソッド<see cref="Parser.DoParse(Reader)"/>を実装するとき、
    /// このメソッドの戻り値として<see cref="ResultCore"/>構造体のインスタンスを返す必要があります。
    /// </para>
    /// </summary>
    public struct ResultCore
    {
        #region 静的メンバー
        /// <summary>
        /// 暗黙のキャストを行います。
        /// </summary>
        /// <param name="res"></param>
        public static implicit operator ResultCore(Result res)
        {
            return res.DetachPosition();
        }
        /// <summary>
        /// 静的ファクトリーメソッドです。
        /// </summary>
        /// <returns></returns>
        public static ResultCore OfSuccess()
        {
            return new ResultCore(true, null, true);
        }
        /// <summary>
        /// 静的ファクトリーメソッドです。
        /// </summary>
        /// <param name="canBacktrack"><c>true</c>の場合バックトラックは有効</param>
        /// <returns></returns>
        public static ResultCore OfSuccess(bool canBacktrack)
        {
            return new ResultCore(true, null, canBacktrack);
        }
        /// <summary>
        /// 静的ファクトリーメソッドです。
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <returns></returns>
        public static ResultCore OfFailure(string message)
        {
            return new ResultCore(false, message, true);
        }
        /// <summary>
        /// 静的ファクトリーメソッドです。
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        /// <param name="canBacktrack"><c>true</c>の場合バックトラックは有効</param>
        /// <returns></returns>
        public static ResultCore OfFailure(string message, bool canBacktrack)
        {
            return new ResultCore(false, message, canBacktrack);
        }

        #endregion

        ResultCore(bool successful, string message, bool canBacktrack)
        {
            Successful = successful;
            _message = message;
            CanBacktrack = canBacktrack;
        }

        readonly string _message;

        /// <summary>
        /// <c>true</c>の場合パース成功です。
        /// </summary>
        public bool Successful { get; }
        /// <summary>
        /// パース失敗の理由を示すメッセージです。
        /// </summary>
        /// <exception cref="InvalidOperationException">パースが失敗していない場合</exception>
        public string Message => !Successful ? _message : throw new InvalidOperationException();
        /// <summary>
        /// <c>true</c>の場合バックトラックは有効です。
        /// </summary>
        public bool CanBacktrack { get; }

        /// <summary>
        /// バックトラック設定を変更したインスタンスを返します。
        /// </summary>
        /// <param name="yesNo"><c>true</c>の場合バックトラック設定はON</param>
        /// <returns></returns>
        public ResultCore AllowBacktrack(bool yesNo)
        {
            return new ResultCore(Successful, _message, yesNo);
        }
        /// <summary>
        /// パース開始と終了の文字位置情報を付与します。
        /// </summary>
        /// <param name="start">開始の文字位置</param>
        /// <param name="end">終了の文字位置</param>
        /// <returns></returns>
        public Result AttachPosition(CharPosition start, CharPosition end)
        {
            if (Successful)
            {
                return Result.OfSuccess(start, end, CanBacktrack);
            }
            else
            {
                return Result.OfFailure(_message, start, end, CanBacktrack);
            }
        }
        /// <summary>
        /// このインスタンスの文字列表現を返します。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Successful)
            {
                return string.Format("ResultCore(Successful: {0})", Successful);
            }
            else
            {
                return string.Format("ResultCore(Successful: {0}, Message: {1})", Successful, _message);
            }
        }
    }
}
