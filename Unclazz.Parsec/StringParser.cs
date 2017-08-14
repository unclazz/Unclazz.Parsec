using System;

namespace Unclazz.Parsec
{
    /// <summary>
    /// 文字列を読み取るパーサーです。
    /// <para>パースのロジックそのものはデリゲートの形でパラーメータ化されています。</para>
    /// <para>このパーサーには自動キャプチャの機能が備わっています。
    /// <see cref="Capturable"/>を通じてこの機能がオンになったインスタンスが得られます。</para>
    /// <para>自動キャプチャ機能がオンの場合、パース処理本体が実行される前に<see cref="ParserInput.Mark"/>が呼び出されます。
    /// パース処理本体が成功した場合は<see cref="ParserInput.Capture(bool)"/>が呼び出されます。
    /// パース処理本体が失敗した場合は単に<see cref="ParserInput.Unmark"/>が呼び出されます。</para>
    /// </summary>
    public sealed class StringParser : Parser<string>
    {
        /// <summary>
        /// コンストラクタです。
        /// このコンストラクタにより生成されるインスタンスの自動キャプチャ機能はオフになっています。
        /// </summary>
        /// <param name="parse">パース・ロジックを提供するデリゲート</param>
        public StringParser(Func<ParserInput, ParseResult<string>> parse) : this(parse, false) { }
        StringParser(Func<ParserInput, ParseResult<string>> parse, bool capture)
        {
            _parse = parse ?? throw new ArgumentNullException(nameof(parse));
            _capture = capture;
        }

        readonly Func<ParserInput, ParseResult<string>> _parse;
        readonly bool _capture;

        /// <summary>
        /// 自動キャプチャ機能がオンになっているインスタンスです。
        /// </summary>
        public StringParser Capturable => new StringParser(_parse, true);

        /// <summary>
        /// パースを行います。
        /// このインスタンスが行うパース処理本体はコンストラクタの引数として渡されたデリゲートに任されます。
        /// </summary>
        /// <param name="input">入力データ</param>
        /// <returns>パース結果</returns>
        public override ParseResult<string> Parse(ParserInput input)
        {
            if (_capture) input.Mark();
            var r = _parse(input);
            if (r.Successful)
            {
                return _capture ? r.Attach(input.Capture(true)) : r.Detach();
            }
            if (_capture) input.Unmark();
            return r;
        }
    }
}
