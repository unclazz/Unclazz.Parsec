using System;

namespace Unclazz.Parsec
{
    /// <summary>
    /// <see cref="Parser"/>の派生クラスのパース結果となる構造体です。
    /// <para>
    /// <see cref="Parser.DoParse(Reader)"/>の戻り値<see cref="ResultCore"/>に対して、
    /// パースの開始と終了の文字位置が付与されたものです。
    /// この文字位置の付与の操作は<see cref="Parser.Parse(Reader)"/>の中で自動的に行われます。
    /// </para>
    /// </summary>
    public struct Result
    {
#region 静的メンバー
        /// <summary>
        /// 左被演算子側が成功を示すインスタンスの場合はそれを返し、それ以外の場合は右被演算子側を返します。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Result operator |(Result left, Result right)
        {
            return left.Or(right);
        }

        /// <summary>
        /// 静的ファクトリーメソッドです。
        /// </summary>
        /// <param name="start">開始の文字位置</param>
        /// <param name="end">終了の文字位置</param>
        /// <returns></returns>
        public static Result OfSuccess(CharPosition start, CharPosition end)
        {
            return new Result(true, start, end, null, true);
        }
        /// <summary>
        /// 静的ファクトリーメソッドです。
        /// </summary>
        /// <param name="start">開始の文字位置</param>
        /// <param name="end">終了の文字位置</param>
        /// <param name="canBacktrack"><c>true</c>の場合バックトラックは有効</param>
        /// <returns></returns>
        public static Result OfSuccess(CharPosition start, CharPosition end, bool canBacktrack)
        {
            return new Result(true, start, end, null, canBacktrack);
        }
        /// <summary>
        /// 静的ファクトリーメソッドです。
        /// </summary>
        /// <param name="message">パース失敗の理由を示すメッセージ</param>
        /// <param name="start">開始の文字位置</param>
        /// <param name="end">終了の文字位置</param>
        /// <returns></returns>
        public static Result OfFailure(string message, CharPosition start, CharPosition end)
        {
            return new Result(false, start, end, message, true);
        }
        /// <summary>
        /// 静的ファクトリーメソッドです。
        /// </summary>
        /// <param name="message">パース失敗の理由を示すメッセージ</param>
        /// <param name="start">開始の文字位置</param>
        /// <param name="end">終了の文字位置</param>
        /// <param name="canBacktrack"><c>true</c>の場合バックトラックは有効</param>
        /// <returns></returns>
        public static Result OfFailure(string message, CharPosition start, CharPosition end, bool canBacktrack)
        {
            return new Result(false, start, end, message, canBacktrack);
        }
#endregion

        Result(bool successful, CharPosition start, CharPosition end, string message,bool canBacktrack)
        {
            Start = start;
            End = end;
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
        /// パースの開始の文字位置です。
        /// </summary>
        public CharPosition Start { get; }
        /// <summary>
        /// パースの終了の文字位置です。
        /// </summary>
        public CharPosition End { get; }
        /// <summary>
        /// <c>true</c>の場合バックトラックは有効です。
        /// </summary>
        public bool CanBacktrack { get; }

        /// <summary>
        /// バックトラック設定を変更したインスタンスを返します。
        /// </summary>
        /// <param name="yesNo"><c>true</c>の場合バックトラック設定はON</param>
        /// <returns></returns>
        public Result AllowBacktrack(bool yesNo)
        {
            return new Result(Successful, Start, End, _message, yesNo);
        }
        /// <summary>
        /// 結果値型を持つ<see cref="Result{T}"/>に変換します。
        /// <see cref="Result{T}.Capture"/>は<typeparamref name="T"/>のデフォルト値を返します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Result<T> Typed<T>()
        {
            return Typed(default(T));
        }
        /// <summary>
        /// 結果値型を持つ<see cref="Result{T}"/>に変換します。
        /// <see cref="Result{T}.Capture"/>は引数で指定された値を返します。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public Result<T> Typed<T>(T value)
        {
            if (Successful)
            {
                return Result<T>.OfSuccess(value, Start, End, CanBacktrack);
            }
            else
            {
                return Result<T>.OfFailure(_message, Start, End, CanBacktrack);
            }
        }
        /// <summary>
        /// 左被演算子（レシーバー）側が成功の場合はそれを返し、それ以外の場合は右被演算子（引数）側を返します。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Result Or(Result other)
        {
            return Successful ? this : other;
        }
        /// <summary>
        /// パースに成功している場合はアクションを実行します。
        /// </summary>
        /// <param name="act"></param>
        public void IfSuccessful(Action act)
        {
            if (Successful) act();
        }
        /// <summary>
        /// パースに成功している場合は第1引数のアクションを実行し、
        /// 失敗している場合は第2引数のアクションを実行します。
        /// </summary>
        /// <param name="act"></param>
        /// <param name="orElse"></param>
        public void IfSuccessful(Action act, Action<string> orElse)
        {
            if (Successful) act();
            else orElse(_message);
        }
        /// <summary>
        /// パースに失敗している場合はアクションを実行します。
        /// </summary>
        /// <param name="act"></param>
        public void IfFailed(Action<string> act)
        {
            if (!Successful) act(_message);
        }
        /// <summary>
        /// パース開始と終了の文字位置の情報を持たない<see cref="ResultCore"/>を返します。
        /// </summary>
        /// <returns></returns>
        public ResultCore DetachPosition()
        {
            if (Successful)
            {
                return ResultCore.OfSuccess(CanBacktrack);
            }
            else
            {
                return ResultCore.OfFailure(_message, CanBacktrack);
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
                return string.Format("Result(Successful: {0}, Start: {1}, End: {2})",
                    Successful, Start, End);
            }
            else
            {
                return string.Format("Result(Successful: {0}, Message: {1}, Start: {2}, End: {3})",
                    Successful, _message, Start, End);
            }
        }
    }
}
