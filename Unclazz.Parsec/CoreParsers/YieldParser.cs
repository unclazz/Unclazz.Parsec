namespace Unclazz.Parsec.CoreParsers
{
    /// <summary>
    /// いついかなるときも指定された値を返すパーサーです。
    /// このパーサーはいつでもパース成功の結果を返します。
    /// パースにあたって文字位置の変更を行いません。
    /// </summary>
    /// <typeparam name="TResult">このパーサーの結果型</typeparam>
    sealed class YieldParser<TResult> : Parser<TResult>
    {
        internal YieldParser(TResult value) : this(ParserFactory.Default, value) { }
        internal YieldParser(IParserConfiguration conf, TResult value) : base(conf)
        {
            _value = value;
        }
        readonly TResult _value;
        protected override ResultCore<TResult> DoParse(Reader input)
        {
            // いつでも成功
            return Success(_value);
        }
        public override string ToString()
        {
            return string.Format("Yield({0})");
        }
    }
}
