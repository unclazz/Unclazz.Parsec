namespace Unclazz.Parsec.Intrinsics
{
    /// <summary>
    /// いついかなるときも指定された値を返すパーサーです。
    /// このパーサーはいつでもパース成功の結果を返します。
    /// パースにあたって文字位置の変更を行いません。
    /// </summary>
    /// <typeparam name="TResult">このパーサーの結果型</typeparam>
    sealed class YieldParser<TResult> : Parser<TResult>
    {
        internal YieldParser(TResult value) : base("Yield")
        {
            _value = value;
        }
        readonly TResult _value;
        protected override ResultCore<TResult> DoParse(Context ctx)
        {
            // いつでも成功
            return Success(_value);
        }
    }
}
