using System;

namespace Unclazz.Parsec.CoreParsers
{
    /// <summary>
    /// 文字列を読み取りキャプチャするパーサーです。
    /// <para>
    /// パース処理そのものはコンストラクタに渡されたパーサーに委譲されます。
    /// ただし元になるパーサーが返す値の型がなんであれ、
    /// パース開始から終了（パース成功）までの区間のデータはあくまでも文字列としてキャプチャされ、
    /// それがこのラッパーとなる新しいパーサーが返す値となります。</para>
    /// <para>
    /// 内部的な動作はおおよそ次のように進みます。
    /// パース処理本体が実行される前に<see cref="ParserInput.Mark"/>が呼び出されます。
    /// パース処理本体が成功した場合は<see cref="ParserInput.Capture(bool)"/>が呼び出されます。
    /// パース処理本体が失敗した場合は単に<see cref="ParserInput.Unmark"/>が呼び出されます。</para>
    /// <para>
    /// <see cref="Parser{T}.Cut"/>によるバックトラック可否設定は引き継がれます。
    /// </para>
    /// </summary>
    /// <typeparam name="T">任意の型</typeparam>
    sealed class CaptureParser<T> : Parser<string>
    {
        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="parse">パース・ロジックを提供するデリゲート</param>
        public CaptureParser(IParser<T> parse)
        {
            _parse = parse ?? throw new ArgumentNullException(nameof(parse));
        }

        readonly IParser<T> _parse;

        /// <summary>
        /// パースを行います。
        /// </summary>
        /// <param name="input">入力データ</param>
        /// <returns>パース結果</returns>
        public override ParseResult<string> Parse(ParserInput input)
        {
            input.Mark();
            var r = _parse.Parse(input);
            if (r.Successful)
            {
                return r.Cast<string>().Attach(input.Capture(true));
            }
            input.Unmark();
            return r.Cast<string>();
        }
        /// <summary>
        /// このパーサーの文字列表現を返します。
        /// </summary>
        /// <returns>文字列表現</returns>
        public override string ToString()
        {
            return string.Format("Capture({0})", _parse);
        }
    }
}
