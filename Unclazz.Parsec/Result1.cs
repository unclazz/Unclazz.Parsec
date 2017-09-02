using System;

namespace Unclazz.Parsec
{
    /// <summary>
    /// <see cref="Parser{T}"/>の派生クラスのパース結果となる構造体です。
    /// <para>
    /// <see cref="Parser{T}.DoParse(Reader)"/>の戻り値<see cref="ResultCore{T}"/>に対して、
    /// パースの開始と終了の文字位置が付与されたものです。
    /// この文字位置の付与の操作は<see cref="Parser{T}.Parse(Reader)"/>の中で自動的に行われます。
    /// </para>
    /// </summary>
    public struct Result<T>
    {
        /// <summary>
        /// 左被演算子側が成功を示すインスタンスの場合はそれを返し、それ以外の場合は右被演算子側を返します。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Result<T> operator |(Result<T> left, Result<T> right)
        {
            return left.Or(right);
        }
        /// <summary>
        /// 暗黙のキャストを行います。
        /// </summary>
        /// <param name="operand"></param>
        public static implicit operator Result(Result<T> operand)
        {
            return operand.Untyped();
        }

        /// <summary>
        /// 静的ファクトリーメソッドです。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Result<T> OfSuccess(T value, CharPosition start, CharPosition end)
        {
            return new Result<T>(true, start, end, null, value, true);
        }
        /// <summary>
        /// 静的ファクトリーメソッドです。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="canBacktrack"></param>
        /// <returns></returns>
        public static Result<T> OfSuccess(T value, CharPosition start, CharPosition end, bool canBacktrack)
        {
            return new Result<T>(true, start, end, null, value, canBacktrack);
        }
        /// <summary>
        /// 静的ファクトリーメソッドです。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static Result<T> OfFailure(string message, CharPosition start, CharPosition end)
        {
            return new Result<T>(false, start, end, message, default(T), true);
        }
        /// <summary>
        /// 静的ファクトリーメソッドです。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="canBacktrack"></param>
        /// <returns></returns>
        public static Result<T> OfFailure(string message, CharPosition start, CharPosition end, bool canBacktrack)
        {
            return new Result<T>(false, start, end, message, default(T), canBacktrack);
        }

        Result(bool successful, CharPosition start, CharPosition end, string message, T value, bool canBacktrack)
        {
            Start = start;
            End = end;
            Successful = successful;
            _message = message;
            _value = value;
            CanBacktrack = canBacktrack;
        }

        readonly string _message;
        readonly T _value;

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
        public Result<T> AllowBacktrack(bool yesNo)
        {
            return new Result<T>(Successful, Start, End, _message, _value, yesNo);
        }
        /// <summary>
        /// パースによりキャプチャされた値に関数を適用します。
        /// <para>このインスタンスがパース失敗を示すものである場合は関数は呼び出されません。</para>
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public Result<U> Map<U>(Func<T, U> func)
        {
            return new Result<U>(Successful, Start, End, _message,
                Successful ? func(_value) : default(U), CanBacktrack);
        }
        /// <summary>
        /// 結果値型を持たない<see cref="Result"/>に変換します。
        /// </summary>
        /// <returns></returns>
        public Result Untyped()
        {
            if (Successful)
            {
                return Result.OfSuccess(Start, End, CanBacktrack);
            }
            else
            {
                return Result.OfFailure(_message, Start, End, CanBacktrack);
            }
        }
        /// <summary>
        /// 異なる結果値型を持つ<see cref="Result{U}"/>に変換します。
        /// <see cref="Result{U}.Capture"/>は<typeparamref name="U"/>のデフォルト値を返します。
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <returns></returns>
        public Result<U> Retyped<U>()
        {
            return Retyped(default(U));
        }
        /// <summary>
        /// 異なる結果値型を持つ<see cref="Result{U}"/>に変換します。
        /// <see cref="Result{U}.Capture"/>は引数で指定された値を返します。
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public Result<U> Retyped<U>(U value)
        {
            if (Successful)
            {
                return Result<U>.OfSuccess(value, Start, End, CanBacktrack);
            }
            else
            {
                return Result<U>.OfFailure(_message, Start, End, CanBacktrack);
            }
        }
        /// <summary>
        /// 左被演算子（レシーバー）側が成功の場合はそれを返し、それ以外の場合は右被演算子（引数）側を返します。
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Result<T> Or(Result<T> other)
        {
            return Successful ? this : other;
        }
        /// <summary>
        /// パースに成功している場合はアクションを実行します。
        /// </summary>
        /// <param name="act"></param>
        public void IfSuccessful(Action<T> act)
        {
            if (Successful) act(_value);
        }
        /// <summary>
        /// パースに成功している場合は第1引数のアクションを実行し、
        /// 失敗している場合は第2引数のアクションを実行します。
        /// </summary>
        /// <param name="act"></param>
        /// <param name="orElse"></param>
        public void IfSuccessful(Action<T> act, Action<string> orElse)
        {
            if (Successful) act(_value);
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
        /// パース開始と終了の文字位置の情報を持たない<see cref="ResultCore{T}"/>を返します。
        /// </summary>
        /// <returns></returns>
        public ResultCore<T> DetachPosition()
        {
            if (Successful)
            {
                return ResultCore<T>.OfSuccess(_value, CanBacktrack);
            }
            else
            {
                return ResultCore<T>.OfFailure(_message, CanBacktrack);
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
                return string.Format("Result<{4}>(Successful: {0}, Value: {1}, Start: {2}, End: {3})",
                    Successful, _value, Start, End, ParsecUtility.TypeToString(typeof(T)));
            }
            else
            {
                return string.Format("Result<{4}>(Successful: {0}, Message: {1}, Start: {2}, End: {3})",
                    Successful, _message, Start, End, ParsecUtility.TypeToString(typeof(T)));
            }
        }
    }
}
