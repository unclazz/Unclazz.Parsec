using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unclazz.Parsec.Intrinsics;

namespace Unclazz.Parsec
{
    /// <summary>
    /// <see cref="Parser"/>と<see cref="Parser{T}"/>のベースクラスです。
    /// 2つのクラスに共通するロジックを実装します。
    /// </summary>
    public abstract class ParserBase
    {

        /// <summary>
        /// デフォルトのコンストラクタです。
        /// パーサー名<see cref="Name"/>には型名から導出された値が設定されます。
        /// </summary>
        internal ParserBase()
        {
            Name = Regex.Replace(GetType().Name, "Parser$", string.Empty);
        }
        /// <summary>
        /// 任意のパーサー名を指定できるコンストラクタです。
        /// </summary>
        /// <param name="name">パーサー名</param>
        internal ParserBase(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// パーサー名
        /// </summary>
        /// <value>パーサー名</value>
        protected internal string Name { get; }

        #region 定義済みパーサーを提供するプロパティの宣言
        static Parser _cachedBeginningOfFile;
        static Parser _cachedEndOfFile;
        static Parser _cachedWhileSpaceAndControls;
        static Parser<int> _hexDigits;
        static Parser<double> _number;

        /// <summary>
        /// データソースの先頭（BOF）にだけマッチするパーサーです。
        /// </summary>
        protected static Parser BeginningOfFile => _cachedEndOfFile ?? (_cachedBeginningOfFile = new BeginningOfFileParser());
        /// <summary>
        /// データソースの終端（EOF）にだけマッチするパーサーです。
        /// </summary>
        protected static Parser EndOfFile => _cachedEndOfFile ?? (_cachedEndOfFile = new EndOfFileParser());
        /// <summary>
        /// 0文字以上の空白文字(コードポイント<c>32</c>）と
        /// 制御文字（同<c>0</c>から<c>31</c>と<c>127</c>）にマッチするパーサーです。
        /// </summary>
        protected static Parser WhileSpaceAndControls => _cachedWhileSpaceAndControls
            ?? (_cachedWhileSpaceAndControls = new CharsWhileInParser(CharClass.SpaceAndControl, 0));
        /// <summary>
        /// 16進数リテラルを読み取ります。
        /// </summary>
        protected static Parser<int> HexDigits => _hexDigits ?? (_hexDigits = new HexDigitsParser());
        /// <summary>
        /// 数値リテラルを読み取ります。
        /// <para>
        /// 構文はJSONの<c>Number</c>型リテラルと同じです。
        /// オプションの符合で始まり、整数部、オプションの小数部、そしてオプションの指数部を含みます。
        /// </para>
        /// </summary>
        protected static Parser<double> Number => _number ?? (_number = new NumberParser());

        /// <summary>
        /// <c>"\\u"</c>もしくは任意の接頭辞から始まるUnicodeエスケープシーケンスを読み取ります。
        /// Unicode拡張領域の文字は上位サロゲートと下位サロゲートのそれぞれ単体でパースされます。
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        protected static Parser<char> Utf16UnicodeEscape(string prefix = "\\u")
        {
            return new Utf16UnicodeEscapeParser(prefix);
        }
        /// <summary>
        /// 制御文字のエスケープシーケンスを読み取ります。
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        protected static Parser<char> ControlEscape(char prefix = '\\')
        {
            return new ControlEscapeParser(prefix);
        }
        /// <summary>
        /// 任意の文字のエスケープシーケンスを読み取ります。
        /// 読み取り結果はその文字そのもの、つまりエスケープシーケンスから接頭辞を除去したものです。
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        protected static Parser<char> CharEscape(IEnumerable<char> chars, char prefix = '\\')
        {
            return new EscapeCharInParser(chars, prefix);
        }
        /// <summary>
        /// 引用符で囲われた文字列を読み取ります。
        /// デフォルトでは引用符自体を含めていかなるエスケープシーケンスも認識しません。
        /// パース対象文字列にエスケープシーケンスが含まれる場合は、
        /// 当該シーケンスを適切にハンドルするパーサーを引数で指定してください。
        /// </summary>
        /// <param name="quote"></param>
        /// <param name="escape"></param>
        /// <returns></returns>
        protected static Parser<string> QuotedString(char quote = '\"', Parser<char> escape = null)
        {
            return new QuotedStringParser(quote, escape);
        }

        /// <summary>
        /// パーサーのパース結果成否を反転させるパーサーを生成します。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        protected static Parser Not<T>(Parser<T> operand)
        {
            return new NotParser<T>(operand);
        }
        /// <summary>
        /// パーサーのパース結果成否を反転させるパーサーを生成します。
        /// </summary>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        protected static Parser Not(Parser operand)
        {
            return new NotParser(operand);
        }
        /// <summary>
        /// デリゲートをもとにパーサーを生成します。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="func">パースの実処理を行うデリゲート</param>
        /// <returns>新しいパーサー</returns>
        protected static Parser<T> For<T>(Func<Context, Result<T>> func)
        {
            return new DelegateParser<T>(func);
        }
        /// <summary>
        /// デリゲートをもとにパーサーを生成します。
        /// </summary>
        /// <param name="func">パースの実処理を行うデリゲート</param>
        /// <returns>新しいパーサー</returns>
        protected static Parser For(Func<Context, Result> func)
        {
            return new DelegateParser(func);
        }
        /// <summary>
        /// デリゲートを使用してパーサーを生成します。
        /// デリゲートはパースの直前になるまで実行されません。
        /// </summary>
        /// <typeparam name="T">パーサーが返す値の型</typeparam>
        /// <param name="factory">パーサーを生成するデリゲート</param>
        /// <returns>新しいパーサー</returns>
        protected static Parser<T> Lazy<T>(Func<Parser<T>> factory)
        {
            return new LazyParser<T>(factory);
        }
        /// <summary>
        /// デリゲートを使用してパーサーを生成します。
        /// デリゲートはパースの直前になるまで実行されません。
        /// </summary>
        /// <param name="factory">パーサーを生成するデリゲート</param>
        /// <returns>新しいパーサー</returns>
        protected static Parser Lazy(Func<Parser> factory)
        {
            return new LazyParser(factory);
        }
        /// <summary>
        /// 先読み（look-ahead）を行うパーサーを生成します。
        /// <para>このパーサーはその成否に関わらず文字位置を前進させません。</para>
        /// </summary>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        protected static Parser Lookahead(Parser operand)
        {
            return new LookaheadParser(operand);
        }
        /// <summary>
        /// 指定された文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="ch">文字</param>
        /// <returns>新しいパーサー</returns>
        protected static CharParser Char(char ch)
        {
            return new ExactCharParser(ch);
        }
        /// <summary>
        /// 指定された範囲に該当する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="start">範囲の開始</param>
        /// <param name="end">範囲の終了</param>
        /// <returns>新しいパーサー</returns>
        protected static CharParser CharBetween(char start, char end)
        {
            return new CharClassParser(CharClass.Between(start, end));
        }
        /// <summary>
        /// 指定された文字クラスに属する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="clazz">文字クラス</param>
        /// <returns>新しいパーサー</returns>
        protected static CharParser CharIn(CharClass clazz)
        {
            return new CharClassParser(clazz);
        }
        /// <summary>
        /// 指定された文字の集合に属する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="chars">文字集合</param>
        /// <returns>新しいパーサー</returns>
        protected static CharParser CharIn(IEnumerable<char> chars)
        {
            return new CharClassParser(CharClass.AnyOf(chars));
        }
        /// <summary>
        /// 文字範囲に該当する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="start">範囲の開始</param>
        /// <param name="end">範囲の終了</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        protected static Parser CharsWhileBetween(char start, char end, int min = 1)
        {
            return new CharsWhileBetweenParser(start, end, min);
        }
        /// <summary>
        /// 文字集合に属する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="chars">文字集合</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        protected static Parser CharsWhileIn(IEnumerable<char> chars, int min = 1)
        {
            return new CharsWhileInParser(CharClass.AnyOf(chars), min);
        }
        /// <summary>
        /// 文字クラスに属する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="clazz">文字クラス</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        protected static Parser CharsWhileIn(CharClass clazz, int min = 1)
        {
            return new CharsWhileInParser(clazz, min);
        }
        /// <summary>
        /// 指定したキーワードにのみマッチするパーサーを生成します。
        /// <para>
        /// <paramref name="cutIndex"/>によりカット（トラックバックの無効化）を行う文字位置を指定できます。
        /// パース処理がこの文字位置の以降に進んだ時、直前の<c>|</c>や<c>Or(...)</c>を起点とするトラックバックは無効になります。
        /// </para>
        /// </summary>
        /// <param name="keyword">キーワード</param>
        /// <param name="cutIndex">カットを行う文字位置</param>
        /// <returns>新しいパーサー</returns>
        protected static Parser Keyword(string keyword, int cutIndex = -1)
        {
            return new KeywordParser(keyword, cutIndex);
        }
        /// <summary>
        /// 指定したキーワードのいずれかにのみマッチするパーサーを生成します。
        /// </summary>
        /// <param name="keywords">キーワード</param>
        /// <returns>新しいパーサー</returns>
        protected static Parser KeywordIn(params string[] keywords)
        {
            return new KeywordInParser(keywords);
        }
        /// <summary>
        /// 指定した値をキャプチャ結果とするパーサーを生成します。
        /// <para>このパーサーは実際には読み取りは行わず、パース結果は必ず成功となります。
        /// キャプチャを行わないパーサーに<c>&amp;</c>や<c>Then(...)</c>で結合することで、
        /// そのパーサーの代わりにキャプチャ結果を作り出すように働きます。</para>
        /// </summary>
        /// <typeparam name="U">任意の型</typeparam>
        /// <param name="value">キャプチャ結果となる値</param>
        /// <returns>新しいパーサー</returns>
        protected static Parser<U> Yield<U>(U value)
        {
            return new YieldParser<U>(value);
        }
        /// <summary>
        /// 指定されたパーサーがマッチするまで入力データを1文字ずつ先に読み進めるパーサーを生成します。
        /// </summary>
        /// <returns>新しいパーサー</returns>
        /// <param name="end">パーサー</param>
        protected static Parser SkipTo(Parser end)
        {
            return new SkipToParser(end);
        }
        #endregion
    }
}
