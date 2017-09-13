using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec.CoreParsers;

namespace Unclazz.Parsec
{

    sealed class ParserFactory : IParserConfiguration, IParserConfigurer, IParserFactory
    {
        internal static ParserFactory Default => new ParserFactory();

        ParserFactory() { }
        internal ParserFactory(IParserConfiguration original)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));
            SetAutoSkip(original.AutoSkip);
            SetParseLogging(original.ParseLogging);
            SetSkipTarget(original.SkipTarget);
            SetParseLogger(original.ParseLogger);
        }

        #region IParserConfigurationメンバーの宣言
        CharClass _skipTarget = CharClass.SpaceAndControl;
        Action<string> _parseLogger = Console.WriteLine;
        bool _autoSkip;
        bool _parseLogging;

        public Action<string> ParseLogger => _parseLogger;
        public bool ParseLogging => _parseLogging;
        public CharClass SkipTarget => _skipTarget;
        public bool AutoSkip => _autoSkip;

        public IParserConfiguration Copy()
        {
            return new ParserFactory(this);
        }
        #endregion


        #region IParserConfigurerメンバーの宣言
        public IParserConfigurer SetSkipTarget(CharClass clazz)
        {
            _skipTarget = clazz ?? throw new ArgumentNullException(nameof(clazz));
            return this;
        }
        public IParserConfigurer SetParseLogger(Action<string> logger)
        {
            _parseLogger = logger ?? throw new ArgumentNullException(nameof(logger));
            return this;
        }
        public IParserConfigurer SetAutoSkip(bool onOff)
        {
            _autoSkip = onOff;
            return this;
        }
        public IParserConfigurer SetParseLogging(bool onOff)
        {
            _parseLogging = onOff;
            return this;
        }
        #endregion

        #region IParserFactoryメンバーの宣言
        public IParserConfiguration Configuration { get; }
        Parser _cachedBeginningOfFile;
        Parser _cachedEndOfFile;
        Parser _cachedWhileSpaceAndControls;
        Parser<int> _hexDigits;

        /// <summary>
        /// データソースの先頭（BOF）にだけマッチするパーサーです。
        /// </summary>
        public Parser BeginningOfFile => _cachedEndOfFile ?? (_cachedBeginningOfFile = new BeginningOfFileParser(this));
        /// <summary>
        /// データソースの終端（EOF）にだけマッチするパーサーです。
        /// </summary>
        public Parser EndOfFile => _cachedEndOfFile ?? (_cachedEndOfFile = new EndOfFileParser(this));
        /// <summary>
        /// 0文字以上の空白文字(コードポイント<c>32</c>）と
        /// 制御文字（同<c>0</c>から<c>31</c>と<c>127</c>）にマッチするパーサーです。
        /// </summary>
        public Parser WhileSpaceAndControls => _cachedWhileSpaceAndControls 
            ?? (_cachedWhileSpaceAndControls = new CharsWhileInParser(this, CharClass.SpaceAndControl, 0));

        public Parser<int> HexDigits => _hexDigits ?? (_hexDigits = new HexDigitsParser());
        public Parser<char> Utf16UnicodeEscape(string prefix = "\\u", int cutIndex = -1)
        {
            return new Utf16UnicodeEscapeParser(prefix, cutIndex);
        }
        public Parser<char> ControlEscape (char prefix = '\\')
        {
            return new ControlEscapeParser(prefix);
        }
        public Parser<char> CharEscape(IEnumerable<char> chars, char prefix = '\\')
        {
            return new EscapeCharInParser(chars, prefix);
        }
        public Parser<string> QuotedString(char quote = '\"', Parser<char> escape = null)
        {
            return new QuotedStringParser(quote, escape);
        }

        /// <summary>
        /// パーサーのパース結果成否を反転させるパーサーを生成します。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public Parser Not<T>(Parser<T> operand)
        {
            return new NotParser<T>(this, operand);
        }
        /// <summary>
        /// パーサーのパース結果成否を反転させるパーサーを生成します。
        /// </summary>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public Parser Not(Parser operand)
        {
            return new NotParser(this, operand);
        }
        /// <summary>
        /// デリゲートをもとにパーサーを生成します。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="func">パースの実処理を行うデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public Parser<T> For<T>(Func<Reader, Result<T>> func)
        {
            return new DelegateParser<T>(this, func);
        }
        /// <summary>
        /// デリゲートをもとにパーサーを生成します。
        /// </summary>
        /// <param name="func">パースの実処理を行うデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public Parser For(Func<Reader, Result> func)
        {
            return new DelegateParser(this, func);
        }
        /// <summary>
        /// デリゲートを使用してパーサーを生成します。
        /// デリゲートはパースの直前になるまで実行されません。
        /// </summary>
        /// <typeparam name="T">パーサーが返す値の型</typeparam>
        /// <param name="factory">パーサーを生成するデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public Parser<T> Lazy<T>(Func<Parser<T>> factory)
        {
            return new LazyParser<T>(this, factory);
        }
        /// <summary>
        /// デリゲートを使用してパーサーを生成します。
        /// デリゲートはパースの直前になるまで実行されません。
        /// </summary>
        /// <param name="factory">パーサーを生成するデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public Parser Lazy(Func<Parser> factory)
        {
            return new LazyParser(this, factory);
        }
        /// <summary>
        /// 先読み（look-ahead）を行うパーサーを生成します。
        /// <para>このパーサーはその成否に関わらず文字位置を前進させません。</para>
        /// </summary>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public Parser Lookahead(Parser operand)
        {
            return new LookaheadParser(operand);
        }
        /// <summary>
        /// 指定された文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="ch">文字</param>
        /// <returns>新しいパーサー</returns>
        public CharParser Char(char ch)
        {
            return new ExactCharParser(this, ch);
        }
        /// <summary>
        /// 指定された範囲に該当する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="start">範囲の開始</param>
        /// <param name="end">範囲の終了</param>
        /// <returns>新しいパーサー</returns>
        public CharParser CharBetween(char start, char end)
        {
            return new CharClassParser(this, CharClass.Between(start, end));
        }
        /// <summary>
        /// 指定された文字クラスに属する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="clazz">文字クラス</param>
        /// <returns>新しいパーサー</returns>
        public CharParser CharIn(CharClass clazz)
        {
            return new CharClassParser(this, clazz);
        }
        /// <summary>
        /// 指定された文字の集合に属する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="chars">文字集合</param>
        /// <returns>新しいパーサー</returns>
        public CharParser CharIn(IEnumerable<char> chars)
        {
            return new CharClassParser(this, CharClass.AnyOf(chars));
        }
        /// <summary>
        /// 文字範囲に該当する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="start">範囲の開始</param>
        /// <param name="end">範囲の終了</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        public Parser CharsWhileBetween(char start, char end, int min = 1)
        {
            return new CharsWhileBetweenParser(this, start, end, min);
        }
        /// <summary>
        /// 文字集合に属する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="chars">文字集合</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        public Parser CharsWhileIn(IEnumerable<char> chars, int min = 1)
        {
            return new CharsWhileInParser(this, CharClass.AnyOf(chars), min);
        }
        /// <summary>
        /// 文字クラスに属する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="clazz">文字クラス</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        public Parser CharsWhileIn(CharClass clazz, int min = 1)
        {
            return new CharsWhileInParser(this, clazz, min);
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
        public Parser Keyword(string keyword, int cutIndex = -1)
        {
            return new KeywordParser(this, keyword, cutIndex);
        }
        /// <summary>
        /// 指定したキーワードのいずれかにのみマッチするパーサーを生成します。
        /// </summary>
        /// <param name="keywords">キーワード</param>
        /// <returns>新しいパーサー</returns>
        public Parser KeywordIn(params string[] keywords)
        {
            return new KeywordInParser(this, keywords);
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
        public Parser<U> Yield<U>(U value)
        {
            return new YieldParser<U>(this, value);
        }
        #endregion
    }
}