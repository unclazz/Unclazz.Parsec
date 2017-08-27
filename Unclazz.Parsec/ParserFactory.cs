using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec.CharClasses;
using Unclazz.Parsec.CoreParsers;

namespace Unclazz.Parsec
{

    sealed class ParserFactory : IParserConfiguration, IParserConfigurer, IParserFactory
    {
        internal static ParserFactory Default { get; } = new ParserFactory();

        readonly static Action<string> _defaultLogger = Console.WriteLine;
        readonly static Parser _defaultNonSignificant = Parsers.WhileSpaceAndControls;

        ParserFactory() { }
        internal ParserFactory(IParserConfiguration original)
        {
            if (original == null) throw new ArgumentNullException(nameof(original));
            SetNonSignificant(original.NonSignificant);
            SetLogger(original.Logger);
        }

        #region IParserConfigurationメンバーの宣言
        Parser _nonSignificant = _defaultNonSignificant;
        Action<string> _logger = _defaultLogger;

        public Parser NonSignificant => _nonSignificant;
        public Action<string> Logger => _logger;

        public IParserConfiguration Copy()
        {
            return new ParserFactory(this);
        }
        #endregion


        #region IParserConfigurerメンバーの宣言
        public IParserConfiguration SetNonSignificant(Parser p)
        {
            _nonSignificant = p;
            _autoConsuming = p != null;
            return this;
        }
        public IParserConfiguration SetLogger(Action<string> l)
        {
            _logger = l;
            _parseLogging = l != null;
            return this;
        }
        #endregion

        #region IParserFactoryメンバーの宣言
        public IParserConfiguration Configuration { get; }
        bool _autoConsuming;
        bool _parseLogging;
        Parser _cachedBeginningOfFile;
        Parser _cachedEndOfFile;
        Parser _cachedWhileSpaceAndControls;

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
            ?? (_cachedWhileSpaceAndControls = new CharsWhileInParser(this, CharClass.Between((char)0, (char)32) + (char)127, 0));

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
            return new NotParser<Nil>(this, operand);
        }
        /// <summary>
        /// デリゲートをもとにパーサーを生成します。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="func">パースの実処理を行うデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public Parser<T> For<T>(Func<Reader, ParseResult<T>> func)
        {
            return new DelegateParser<T>(this, func);
        }
        /// <summary>
        /// デリゲートをもとにパーサーを生成します。
        /// </summary>
        /// <param name="func">パースの実処理を行うデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public Parser For(Func<Reader, ParseResult<Nil>> func)
        {
            return new DelegateParser<Nil>(this, func).Cast();
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
            return new LazyParser<Nil>(this, factory).Cast();
        }
        /// <summary>
        /// 指定された文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="ch">文字</param>
        /// <returns>新しいパーサー</returns>
        public Parser Char(char ch)
        {
            return new CharParser(this, ch);
        }
        /// <summary>
        /// 指定された範囲に該当する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="start">範囲の開始</param>
        /// <param name="end">範囲の終了</param>
        /// <returns>新しいパーサー</returns>
        public Parser CharBetween(char start, char end)
        {
            return new CharClassParser(this, CharClass.Between(start, end));
        }
        /// <summary>
        /// 指定された文字クラスに属する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="clazz">文字クラス</param>
        /// <returns>新しいパーサー</returns>
        public Parser CharIn(CharClass clazz)
        {
            return new CharClassParser(this, clazz);
        }
        /// <summary>
        /// 指定された文字の集合に属する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="chars">文字集合</param>
        /// <returns>新しいパーサー</returns>
        public Parser CharIn(IEnumerable<char> chars)
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
        public Parser StringIn(params string[] keywords)
        {
            return new StringInParser(this, keywords);
        }
        #endregion
    }
}