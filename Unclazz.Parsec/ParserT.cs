using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec.CharClasses;
using Unclazz.Parsec.CoreParsers;

namespace Unclazz.Parsec
{
    /// <summary>
    /// パーサーを表す抽象クラスです。
    /// </summary>
    /// <typeparam name="T">パース結果の型</typeparam>
    public abstract class Parser<T>
    {
        #region 演算子オーバーロードの宣言
        /// <summary>
        /// <see cref="Parsers.For{T}(Func{Reader, ParseResult{T}})"/>と同義です。
        /// </summary>
        /// <param name="func">パース処理を行うデリゲート</param>
        public static implicit operator Parser<T>(Func<Reader, ParseResult<T>> func)
        {
            return Parsers.For(func);
        }
        /// <summary>
        /// <see cref="Parsers.Lazy{T}(Func{Parser{T}})"/>と同義です。
        /// </summary>
        /// <param name="factory">パーサーを返すデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public static implicit operator Parser<T>(Func<Parser<T>> factory)
        {
            return Parsers.Lazy(factory);
        }
        /// <summary>
        /// <see cref="Parsers.Not{T}(Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser operator !(Parser<T> operand)
        {
            return Parsers.Not<T>(operand);
        }
        /// <summary>
        /// <see cref="ParserExtension.Or{T}(Parser{T}, Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> operator |(Parser<T> left, Parser<T> right)
        {
            return OrParser<T>.LeftAssoc(left, right);
        }
        /// <summary>
        /// <see cref="ParserExtension.Or{T}(Parser{T}, Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> operator |(Parser<T> left, Parser right)
        {
            return OrParser<T>.LeftAssoc(left, right.Cast<T>());
        }
        /// <summary>
        /// <see cref="ParserExtension.Or{T}(Parser{T}, Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> operator |(Parser left, Parser<T> right)
        {
            return OrParser<T>.LeftAssoc(left.Cast<T>(), right);
        }
        /// <summary>
        /// <see cref="ParserExtension.Or{T}(Parser{T}, Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサーのパースが失敗したとき新しいパーサーの返す値として使用される値</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> operator |(Parser<T> left, T right)
        {
            return OrParser<T>.LeftAssoc(left, new PassParser<T>(left._factory, right));
        }
        /// <summary>
        /// <see cref="ParserExtension.Then{T, U}(Parser{T}, Parser{U})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<Tuple<T, T>> operator &(Parser<T> left, Parser<T> right)
        {
            return new DoubleParser<T, T>(left._factory, left, right);
        }
        /// <summary>
        /// <see cref="ParserExtension.Then{T}(Parser, Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> operator &(Parser left, Parser<T> right)
        {
            return new ThenTakeRightParser<Nil, T>(left._factory, left, right);
        }
        /// <summary>
        /// <see cref="ParserExtension.Then{T}(Parser{T}, Parser)"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> operator &(Parser<T> left, Parser right)
        {
            return new ThenTakeLeftParser<T, Nil>(left._factory, left, right);
        }
        #endregion


        protected Parser()
        {
            _factory = ParserFactory.Default;
            _autoConsuming = _factory.NonSignificant != null;
            _parseLogging = _factory.Logger != null;
        }
        protected Parser(IParserConfiguration config)
        {
            _factory = new ParserFactory(config) ?? throw new ArgumentNullException(nameof(config));
            _autoConsuming = _factory.NonSignificant != null;
            _parseLogging = _factory.Logger != null;
        }

        readonly ParserFactory _factory;
        bool _autoConsuming;
        bool _parseLogging;

        public IParserConfiguration Configuration => _factory;

        /// <summary>
        /// パースを行います。
        /// <para>
        /// パーサーの具象クラスを実装する場合、このメソッドを実装する必要があります。
        /// パース成否は<see cref="ParseResult{T}"/>のインスタンスで表されます。
        /// このメソッドはいかなる場合も<c>null</c>を返してはなりません。
        /// またこのメソッドは原則として例外スローを行ってはなりません。
        /// 正常・異常を問わずこのメソッド内で起こったことはすべて
        /// <see cref="ParseResult{T}"/>を通じて呼び出し元に通知される必要があります。
        /// </para>
        /// </summary>
        /// <param name="input">入力データ</param>
        /// <returns>パース結果</returns>
        protected abstract ParseResult<T> DoParse(Reader input);

        public ParseResult<T> Parse(Reader input)
        {
            if (_autoConsuming) _factory.NonSignificant.Parse(input);
            if (_parseLogging)
            {
                LogPreParse(input.Position, input.Peek());
                var res = DoParse(input);
                LogPostParse(input.Position, input.Peek(), res);
                return res;
            }
            return DoParse(input);
        }
        public void Configure(Action<IParserConfigurer> act)
        {
            act(_factory);
            _autoConsuming = _factory.NonSignificant != null;
            _parseLogging = _factory.Logger != null;
        }
        void LogPreParse(CharacterPosition pos, int peek)
        {
            WriteLine("##### Pre-Parse #####");
            WriteLine("Parser     : {0} ", ParsecUtility.ObjectTypeToString(this));
            WriteLine("Position   : {0} ", pos);
            WriteLine("Char       : {0} ", ParsecUtility.CharToString(peek));
        }
        void LogPostParse(CharacterPosition pos, int peek, ParseResult<T> result)
        {
            WriteLine("##### Post-Parse #####");
            WriteLine("Parser     : {0} ", ParsecUtility.ObjectTypeToString(this));
            WriteLine("Successful : {0} ", result.Successful);
            if (result.Successful)
            WriteLine("Capture    : {0} ", result.Capture);
            else
            WriteLine("Message    : {0} ", result.Message);
            WriteLine("Position   : {0} ", pos);
            WriteLine("Char       : {0} ", ParsecUtility.CharToString(peek));
        }
        void WriteLine(string format, params object[] args)
        {
            _factory.Logger(string.Format(format, args));
        }
        /// <summary>
        /// パース成功を表す<see cref="ParseResult{T}"/>インスタンスを生成します。
        /// </summary>
        /// <param name="position">パース開始時の文字位置</param>
        /// <param name="canBacktrack">直近の<c>|</c>や<c>Or(...)</c>を
        /// 起点とするバックトラックを有効にするかどうか（デフォルトは<c>true</c>で、バックトラックは有効）</param>
        /// <returns>パース成功を表すインスタンス</returns>
        protected ParseResult<T> Success(CharacterPosition position, bool canBacktrack = true)
        {
            return ParseResult.OfSuccess<T>(position, canBacktrack: canBacktrack);
        }
        /// <summary>
        /// パース成功を表す<see cref="ParseResult{T}"/>インスタンスを生成します。
        /// </summary>
        /// <param name="position">パース開始時の文字位置</param>
        /// <param name="capture">パースされた値を内包する可能性のある<see cref="Optional{T}"/>インスタンス</param>
        /// <param name="canBacktrack">直近の<c>|</c>や<c>Or(...)</c>を
        /// 起点とするバックトラックを有効にするかどうか（デフォルトは<c>true</c>で、バックトラックは有効）</param>
        /// <returns>パース成功を表すインスタンス</returns>
        protected ParseResult<T> Success(CharacterPosition position, Optional<T> capture, bool canBacktrack = true)
        {
            return ParseResult.OfSuccess(position, capture, canBacktrack);
        }
        /// <summary>
        /// パース失敗を表す<see cref="ParseResult{T}"/>インスタンスを生成します。
        /// </summary>
        /// <param name="position">パース開始時の文字位置</param>
        /// <param name="message">パース失敗の理由を示すメッセージ</param>
        /// <param name="canBacktrack">直近の<c>|</c>や<c>Or(...)</c>を
        /// 起点とするバックトラックを有効にするかどうか（デフォルトは<c>true</c>で、バックトラックは有効）</param>
        /// <returns>パース成功を表すインスタンス</returns>
        protected ParseResult<T> Failure(CharacterPosition position, string message, bool canBacktrack = true)
        {
            return ParseResult.OfFailure<T>(position, message, canBacktrack);
        }
        /// <summary>
        /// 読み取り結果の<see cref="Optional{T}"/>が内包する各要素に関数を提供するパーサーを生成します。
        /// <para>
        /// このメソッドが返すパーサーは関数<paramref name="transform"/>が例外をスローした場合、
        /// そのメッセージを使用してパース失敗を表す<see cref="ParseResult{T}"/>インスタンスを返します。
        /// この挙動を変更し、関数がスローした例外をそのまま再スローさせたい場合は
        /// <paramref name="canThrow"/>に<c>true</c>を指定します。
        /// </para>
        /// </summary>
        /// <typeparam name="U">読み取り結果を変換した後の型</typeparam>
        /// <param name="transform">変換を行う関数</param>
        /// <param name="canThrow"><paramref name="transform"/>がスローした例外をそのまま再スローさせる場合<c>true</c></param>
        /// <returns>新しいパーサー</returns>
        public Parser<U> Map<U>(Func<T, U> transform, bool canThrow = false)
        {
            return new MapParser<T, U>(_factory, this, transform, canThrow);
        }

        #region Factory
        /// <summary>
        /// パーサーのパース結果成否を反転させるパーサーを生成します。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public Parser Not<T>(Parser<T> operand) => _factory.Not(operand);
        /// <summary>
        /// パーサーのパース結果成否を反転させるパーサーを生成します。
        /// </summary>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public Parser Not(Parser operand) => _factory.Not(operand);
        /// <summary>
        /// デリゲートをもとにパーサーを生成します。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="func">パースの実処理を行うデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public Parser<T> For<T>(Func<Reader, ParseResult<T>> func) => _factory.For(func);
        /// <summary>
        /// デリゲートをもとにパーサーを生成します。
        /// </summary>
        /// <param name="func">パースの実処理を行うデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public Parser For(Func<Reader, ParseResult<Nil>> func) => _factory.For(func);
        /// <summary>
        /// デリゲートを使用してパーサーを生成します。
        /// デリゲートはパースの直前になるまで実行されません。
        /// </summary>
        /// <typeparam name="T">パーサーが返す値の型</typeparam>
        /// <param name="factory">パーサーを生成するデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public Parser<T> Lazy<T>(Func<Parser<T>> factory) => _factory.Lazy(factory);
        /// <summary>
        /// デリゲートを使用してパーサーを生成します。
        /// デリゲートはパースの直前になるまで実行されません。
        /// </summary>
        /// <param name="factory">パーサーを生成するデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public Parser Lazy(Func<Parser> factory) => _factory.Lazy(factory);
        /// <summary>
        /// 指定された文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="ch">文字</param>
        /// <returns>新しいパーサー</returns>
        public Parser Char(char ch) => _factory.Char(ch);
        /// <summary>
        /// 指定された範囲に該当する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="start">範囲の開始</param>
        /// <param name="end">範囲の終了</param>
        /// <returns>新しいパーサー</returns>
        public Parser CharBetween(char start, char end) => _factory.CharBetween(start, end);
        /// <summary>
        /// 指定された文字クラスに属する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="clazz">文字クラス</param>
        /// <returns>新しいパーサー</returns>
        public Parser CharIn(CharClass clazz) => _factory.CharIn(clazz);
        /// <summary>
        /// 指定された文字の集合に属する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="chars">文字集合</param>
        /// <returns>新しいパーサー</returns>
        public Parser CharIn(IEnumerable<char> chars) => _factory.CharIn(chars);
        /// <summary>
        /// 文字範囲に該当する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="start">範囲の開始</param>
        /// <param name="end">範囲の終了</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        public Parser CharsWhileBetween(char start, char end, int min = 1) => _factory.CharsWhileBetween(start, end, min);
        /// <summary>
        /// 文字集合に属する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="chars">文字集合</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        public Parser CharsWhileIn(IEnumerable<char> chars, int min = 1) => _factory.CharsWhileIn(chars, min);
        /// <summary>
        /// 文字クラスに属する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="clazz">文字クラス</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        public Parser CharsWhileIn(CharClass clazz, int min = 1) => _factory.CharsWhileIn(clazz, min);
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
        public Parser Keyword(string keyword, int cutIndex = -1) => _factory.Keyword(keyword, cutIndex);
        /// <summary>
        /// 指定したキーワードのいずれかにのみマッチするパーサーを生成します。
        /// </summary>
        /// <param name="keywords">キーワード</param>
        /// <returns>新しいパーサー</returns>
        public Parser StringIn(params string[] keywords) => _factory.StringIn(keywords);
        #endregion
    }
}
