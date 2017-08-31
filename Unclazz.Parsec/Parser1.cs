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
        public static implicit operator Parser<T>(Func<Reader, Result<T>> func)
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
            return Parsers.Not(operand);
        }
        /// <summary>
        /// <see cref="ParserExtension.Or{T}(Parser{T}, Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> operator |(Parser<T> left, Parser<T> right)
        {
            return new OrParser<T>(left.Configuration, left, right);
        }
        /// <summary>
        /// <see cref="ParserExtension.Or{T}(Parser{T}, Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser operator |(Parser<T> left, Parser right)
        {
            return new OrParser(left.Configuration, left.Untyped(), right);
        }
        /// <summary>
        /// <see cref="ParserExtension.Or{T}(Parser{T}, Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser operator |(Parser left, Parser<T> right)
        {
            return new OrParser(left.Configuration, left, right.Untyped());
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
            return new ThenTakeRightParser<T>(left.Configuration, left, right);
        }
        /// <summary>
        /// <see cref="ParserExtension.Then{T}(Parser{T}, Parser)"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> operator &(Parser<T> left, Parser right)
        {
            return new ThenTakeLeftParser<T>(left._factory, left, right);
        }
        public static Parser<T> operator &(Parser<T> left, string right)
        {
            return left.Then(new KeywordParser(right));
        }
        public static Parser<T> operator &(string left, Parser<T> right)
        {
            return new KeywordParser(left).Then(right);
        }
        #endregion

        /// <summary>
        /// デフォルトのコンフィギュレーションを使用するコンストラクタです。
        /// </summary>
        protected Parser()
        {
            _factory = ParserFactory.Default;
            _autoSkip = _factory.AutoSkip;
            _parseLogging = _factory.ParseLogging;
        }
        /// <summary>
        /// 引数で指定されたコンフィギュレーションを使用するコンストラクタです。
        /// </summary>
        /// <param name="config"></param>
        protected Parser(IParserConfiguration config)
        {
            _factory = new ParserFactory(config) ?? throw new ArgumentNullException(nameof(config));
            _autoSkip = _factory.AutoSkip;
            _parseLogging = _factory.ParseLogging;
        }

        readonly ParserFactory _factory;
        bool _autoSkip;
        bool _parseLogging;

        /// <summary>
        /// このパーサーのコンフィギュレーションです。
        /// </summary>
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
        protected abstract ResultCore<T> DoParse(Reader input);

        /// <summary>
        /// パースを行います。
        /// <para>
        /// パース成否は戻り値の<see cref="ParseResult{T}"/>のインスタンスで表されます。
        /// このメソッドはいかなる場合も<c>null</c>を返しません。
        /// またこのメソッドは原則として例外スローも行いません。
        /// 正常・異常を問わずこのメソッド内で起こったことはすべて
        /// <see cref="ParseResult{T}"/>を通じて呼び出し元に通知されます。
        /// </para>
        /// <para>
        /// このメソッドは事前処理の後、具象クラスが実装する<see cref="DoParse(Reader)"/>を呼び出します。
        /// その後事後処理を終えてから、呼び出し元に結果を返します。
        /// </para>
        /// </summary>
        /// <param name="input">入力データ</param>
        /// <returns>パース結果</returns>
        public Result<T> Parse(Reader input)
        {
            if (_autoSkip) SkipWhileIn(input, _factory.SkipTarget);
            var start = input.Position;
            if (_parseLogging)
            {
                LogPreParse(input.Position, input.Peek());
                var res = DoParse(input);
                LogPostParse(input.Position, input.Peek(), res);
                return res.AttachPosition(start, input.Position);
            }
            return DoParse(input).AttachPosition(start, input.Position);
        }
        /// <summary>
        /// このパーサーのコンフィギュレーションを変更します。
        /// </summary>
        /// <param name="act">変更を行うアクション</param>
        public void Configure(Action<IParserConfigurer> act)
        {
            act(_factory);
            _autoSkip = _factory.AutoSkip;
            _parseLogging = _factory.ParseLogging;
        }
        /// <summary>
        /// パース成功を表す<see cref="ParseResult{T}"/>インスタンスを生成します。
        /// </summary>
        /// <param name="position">パース開始時の文字位置</param>
        /// <param name="capture">パースされた値を内包する可能性のある<see cref="Optional{T}"/>インスタンス</param>
        /// <param name="canBacktrack">直近の<c>|</c>や<c>Or(...)</c>を
        /// 起点とするバックトラックを有効にするかどうか（デフォルトは<c>true</c>で、バックトラックは有効）</param>
        /// <returns>パース成功を表すインスタンス</returns>
        protected ResultCore<T> Success(T value)
        {
            return ResultCore<T>.OfSuccess(value, true);
        }
        protected ResultCore<T> Success(T value, bool canBacktrack)
        {
            return ResultCore<T>.OfSuccess(value, canBacktrack);
        }
        /// <summary>
        /// パース失敗を表す<see cref="ParseResult{T}"/>インスタンスを生成します。
        /// </summary>
        /// <param name="position">パース開始時の文字位置</param>
        /// <param name="message">パース失敗の理由を示すメッセージ</param>
        /// <param name="canBacktrack">直近の<c>|</c>や<c>Or(...)</c>を
        /// 起点とするバックトラックを有効にするかどうか（デフォルトは<c>true</c>で、バックトラックは有効）</param>
        /// <returns>パース成功を表すインスタンス</returns>
        protected ResultCore<T> Failure(string message)
        {
            return ResultCore<T>.OfFailure(message, true);
        }
        protected ResultCore<T> Failure(string message, bool canBacktrack)
        {
            return ResultCore<T>.OfFailure(message, canBacktrack);
        }
        void LogPreParse(CharacterPosition pos, int peek)
        {
            WriteLine("##### Pre-Parse #####");
            WriteLine("Parser     : {0} ", ParsecUtility.ObjectTypeToString(this));
            WriteLine("Position   : {0} ", pos);
            WriteLine("Char       : {0} ", ParsecUtility.CharToString(peek));
        }
        void LogPostParse(CharacterPosition pos, int peek, ResultCore<T> result)
        {
            WriteLine("##### Post-Parse #####");
            WriteLine("Parser     : {0} ", ParsecUtility.ObjectTypeToString(this));
            WriteLine("Successful : {0} ", result.Successful);
            if (result.Successful)
                WriteLine("Capture    : {0} ", result.Value);
            else
                WriteLine("Message    : {0} ", result.Message);
            WriteLine("Position   : {0} ", pos);
            WriteLine("Char       : {0} ", ParsecUtility.CharToString(peek));
        }
        void WriteLine(string format, params object[] args)
        {
            _factory.ParseLogger(string.Format(format, args));
        }
        void SkipWhileIn(Reader r, CharClass c)
        {
            while (!r.EndOfFile)
            {
                var ch = (char)r.Peek();
                if (!c.Contains(ch)) break;
                r.Read();
            }
        }


        #region 定義済みパーサーを提供するプロパティの宣言
        /// <summary>
        /// データソースの先頭（BOF）にだけマッチするパーサーです。
        /// </summary>
        protected Parser BeginningOfFile => _factory.BeginningOfFile;
        /// <summary>
        /// データソースの終端（EOF）にだけマッチするパーサーです。
        /// </summary>
        public Parser EndOfFile => _factory.EndOfFile;
        /// <summary>
        /// 0文字以上の空白文字(コードポイント<c>32</c>）と
        /// 制御文字（同<c>0</c>から<c>31</c>と<c>127</c>）にマッチするパーサーです。
        /// </summary>
        public Parser WhileSpaceAndControls => _factory.WhileSpaceAndControls;
        #endregion

        #region 定義済みパーサーを提供するメソッドの宣言
        /// <summary>
        /// パーサーのパース結果成否を反転させるパーサーを生成します。
        /// </summary>
        /// <typeparam name="U">任意の型</typeparam>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        protected Parser Not<U>(Parser<U> operand) => _factory.Not(operand);
        /// <summary>
        /// パーサーのパース結果成否を反転させるパーサーを生成します。
        /// </summary>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        protected Parser Not(Parser operand) => _factory.Not(operand);
        /// <summary>
        /// デリゲートをもとにパーサーを生成します。
        /// </summary>
        /// <typeparam name="U">任意の型</typeparam>
        /// <param name="func">パースの実処理を行うデリゲート</param>
        /// <returns>新しいパーサー</returns>
        protected Parser<U> For<U>(Func<Reader, Result<U>> func) => _factory.For(func);
        /// <summary>
        /// デリゲートをもとにパーサーを生成します。
        /// </summary>
        /// <param name="func">パースの実処理を行うデリゲート</param>
        /// <returns>新しいパーサー</returns>
        protected Parser For(Func<Reader, Result> func) => _factory.For(func);
        /// <summary>
        /// デリゲートを使用してパーサーを生成します。
        /// デリゲートはパースの直前になるまで実行されません。
        /// </summary>
        /// <typeparam name="U">パーサーが返す値の型</typeparam>
        /// <param name="factory">パーサーを生成するデリゲート</param>
        /// <returns>新しいパーサー</returns>
        protected Parser<U> Lazy<U>(Func<Parser<U>> factory) => _factory.Lazy(factory);
        /// <summary>
        /// デリゲートを使用してパーサーを生成します。
        /// デリゲートはパースの直前になるまで実行されません。
        /// </summary>
        /// <param name="factory">パーサーを生成するデリゲート</param>
        /// <returns>新しいパーサー</returns>
        protected Parser Lazy(Func<Parser> factory) => _factory.Lazy(factory);
        /// <summary>
        /// 指定された文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="ch">文字</param>
        /// <returns>新しいパーサー</returns>
        protected Parser Char(char ch) => _factory.Char(ch);
        /// <summary>
        /// 指定された範囲に該当する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="start">範囲の開始</param>
        /// <param name="end">範囲の終了</param>
        /// <returns>新しいパーサー</returns>
        protected Parser CharBetween(char start, char end) => _factory.CharBetween(start, end);
        /// <summary>
        /// 指定された文字クラスに属する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="clazz">文字クラス</param>
        /// <returns>新しいパーサー</returns>
        protected Parser CharIn(CharClass clazz) => _factory.CharIn(clazz);
        /// <summary>
        /// 指定された文字の集合に属する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="chars">文字集合</param>
        /// <returns>新しいパーサー</returns>
        protected Parser CharIn(IEnumerable<char> chars) => _factory.CharIn(chars);
        /// <summary>
        /// 文字範囲に該当する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="start">範囲の開始</param>
        /// <param name="end">範囲の終了</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        protected Parser CharsWhileBetween(char start, char end, int min = 1) => _factory.CharsWhileBetween(start, end, min);
        /// <summary>
        /// 文字集合に属する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="chars">文字集合</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        protected Parser CharsWhileIn(IEnumerable<char> chars, int min = 1) => _factory.CharsWhileIn(chars, min);
        /// <summary>
        /// 文字クラスに属する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="clazz">文字クラス</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        protected Parser CharsWhileIn(CharClass clazz, int min = 1) => _factory.CharsWhileIn(clazz, min);
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
        protected Parser Keyword(string keyword, int cutIndex = -1) => _factory.Keyword(keyword, cutIndex);
        /// <summary>
        /// 指定したキーワードのいずれかにのみマッチするパーサーを生成します。
        /// </summary>
        /// <param name="keywords">キーワード</param>
        /// <returns>新しいパーサー</returns>
        protected Parser StringIn(params string[] keywords) => _factory.StringIn(keywords);
        /// <summary>
        /// 指定した値をキャプチャ結果とするパーサーを生成します。
        /// <para>このパーサーは実際には読み取りは行わず、パース結果は必ず成功となります。
        /// キャプチャを行わないパーサーに<c>&amp;</c>や<c>Then(...)</c>で結合することで、
        /// そのパーサーの代わりにキャプチャ結果を作り出すように働きます。</para>
        /// </summary>
        /// <typeparam name="U">任意の型</typeparam>
        /// <param name="value">キャプチャ結果となる値</param>
        /// <returns>新しいパーサー</returns>
        protected Parser<U> Yield<U>(U value) => _factory.Yield(value);
        #endregion
    }
}
