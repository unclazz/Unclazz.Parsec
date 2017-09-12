namespace Unclazz.Parsec.CoreParsers
{
    /// <summary>
    /// 文字にマッチしその値をキャプチャするパーサーです。
    /// </summary>
    public sealed class CharCaptureParser : Parser<char>
    {
        internal CharCaptureParser(CharParser original) : base(original.Configuration)
        {
            _original = original;
        }
        readonly CharParser _original;
        /// <summary>
        /// パースを行います。
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override ResultCore<char> DoParse(Reader input)
        {
            var ch = input.Peek();
            var res = _original.Parse(input);
            return res.Successful ? Success((char)ch) : Failure(res.Message);
        }
        /// <summary>
        /// シーケンスを読み取るパーサーを生成します。
        /// <para>
        /// 4つの引数はいずれもオプションです。
        /// 何も指定しなかった場合、0回以上で上限なしの繰り返しを表します。
        /// <paramref name="exactly"/>を指定した場合はまさにその回数の繰り返しです。
        /// <paramref name="min"/>および/もしくは<paramref name="max"/>を指定した場合は下限および/もしくは上限付きの繰り返しです。
        /// </para>
        /// </summary>
        /// <param name="min">繰り返しの最小回数</param>
        /// <param name="max">繰り返しの最大回数</param>
        /// <param name="exactly">繰り返しの回数</param>
        /// <param name="sep">セパレーターのためのパーサー</param>
        /// <returns>繰り返しをサポートする新しいパーサー</returns>
        public new Parser<string> Repeat(int min = 0, int max = -1, int exactly = -1, Parser sep = null)
        {
            return new StringParser(this, new RepeatConfiguration(min, max, exactly, sep));
        }
        /// <summary>
        /// このインスタンスの文字列表現を返します。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Capture({0})", _original);
        }
    }
}
