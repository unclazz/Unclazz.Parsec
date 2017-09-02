using System;

namespace Unclazz.Parsec
{
    /// <summary>
    /// <see cref="Parser{T}"/>の派生クラスのパース結果中核部となる構造体です。
    /// <para>
    /// <see cref="Parser{T}"/>実装者は抽象メソッド<see cref="Parser{T}.DoParse(Reader)"/>を実装するとき、
    /// このメソッドの戻り値として<see cref="ResultCore{T}"/>構造体のインスタンスを返す必要があります。
    /// </para>
    /// </summary>
    public struct ResultCore<T>
    {
        /// <summary>
        /// 暗黙のキャストを行います。
        /// </summary>
        /// <param name="res"></param>
        public static implicit operator ResultCore<T>(Result<T> res)
        {
            return res.DetachPosition();
        }

        /// <summary>
        /// 静的ファクトリーメソッドです。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ResultCore<T> OfSuccess(T value)
        {
            return new ResultCore<T>(true, null, value, true);
        }
        /// <summary>
        /// 静的ファクトリーメソッドです。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="canBacktrack"></param>
        /// <returns></returns>
        public static ResultCore<T> OfSuccess(T value, bool canBacktrack)
        {
            return new ResultCore<T>(true, null, value, canBacktrack);
        }
        /// <summary>
        /// 静的ファクトリーメソッドです。
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResultCore<T> OfFailure(string message)
        {
            return new ResultCore<T>(false, message, default(T), true);
        }
        /// <summary>
        /// 静的ファクトリーメソッドです。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="canBacktrack"></param>
        /// <returns></returns>
        public static ResultCore<T> OfFailure(string message, bool canBacktrack)
        {
            return new ResultCore<T>(false, message, default(T), canBacktrack);
        }

        ResultCore(bool successful, string message, T value, bool canBacktrack)
        {
            Successful = successful;
            _message = message;
            _value = value;
            CanBacktrack = canBacktrack;
        }

        readonly T _value;
        readonly string _message;

        /// <summary>
        /// <c>true</c>の場合パース成功です。
        /// </summary>
        public bool Successful { get; }
        /// <summary>
        /// パースによりキャプチャされた値です。
        /// </summary>
        /// <exception cref="InvalidOperationException">パースが成功していない場合</exception>
        public T Capture => Successful ? _value : throw new InvalidOperationException();
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
        public ResultCore<T> AllowBacktrack(bool yesNo)
        {
            return new ResultCore<T>(Successful, _message, _value, yesNo);
        }
        /// <summary>
        /// パース開始と終了の文字位置情報を付与します。
        /// </summary>
        /// <param name="start">開始の文字位置</param>
        /// <param name="end">終了の文字位置</param>
        /// <returns></returns>
        public Result<T> AttachPosition(CharPosition start, CharPosition end)
        {
            if (Successful)
            {
                return Result<T>.OfSuccess(_value, start, end, CanBacktrack);
            }
            else
            {
                return Result<T>.OfFailure(_message, start, end, CanBacktrack);
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
                return string.Format("ResultCore<{2}>(Successful: {0}, Value: {1})",
                    Successful, _value, ParsecUtility.TypeToString(typeof(T)));
            }
            else
            {
                return string.Format("ResultCore<{2}>(Successful: {0}, Message: {1})",
                    Successful, _message, ParsecUtility.TypeToString(typeof(T)));
            }
        }
    }
}
