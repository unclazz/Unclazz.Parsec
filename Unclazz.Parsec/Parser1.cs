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
        /// デリゲートをもとにパーサーを生成します。
        /// </summary>
        /// <param name="func"></param>
        public static implicit operator Parser<T>(Func<Reader, Result<T>> func)
        {
            return Parsers.For(func);
        }
        /// <summary>
        /// デリゲートを使用してパーサーを生成します。
        /// デリゲートはパースの直前になるまで実行されません。
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static implicit operator Parser<T>(Func<Parser<T>> factory)
        {
            return Parsers.Lazy(factory);
        }
        /// <summary>
        /// パーサーのパース結果成否を反転させるパーサーを生成します。
        /// </summary>
        /// <param name="operand">元になるパーサー</param>
        /// <returns></returns>
        public static Parser operator !(Parser<T> operand)
        {
            return Parsers.Not(operand);
        }
        /// <summary>
        /// <see cref="Or(Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns></returns>
        public static Parser<T> operator |(Parser<T> left, Parser<T> right)
        {
            return new OrParser<T>(left.Configuration, left, right);
        }
        /// <summary>
        /// <see cref="Or(Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns></returns>
        public static Parser operator |(Parser<T> left, Parser right)
        {
            return new OrParser(left.Configuration, left.Untyped(), right);
        }
        /// <summary>
        /// <see cref="Parser.Or(Parser)"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns></returns>
        public static Parser operator |(Parser left, Parser<T> right)
        {
            return new OrParser(left.Configuration, left, right.Untyped());
        }
        /// <summary>
        /// <see cref="Then{U}(Parser{U})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns></returns>
        public static Parser<Tuple<T, T>> operator &(Parser<T> left, Parser<T> right)
        {
            return new DoubleParser<T, T>(left._factory, left, right);
        }
        /// <summary>
        /// <see cref="Parser.Then{T}(Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns></returns>
        public static Parser<T> operator &(Parser left, Parser<T> right)
        {
            return new ThenTakeRightParser<T>(left.Configuration, left, right);
        }
        /// <summary>
        /// <see cref="Then(Parser)"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns></returns>
        public static Parser<T> operator &(Parser<T> left, Parser right)
        {
            return new ThenTakeLeftParser<T>(left._factory, left, right);
        }
        /// <summary>
        /// <see cref="Then(Parser)"/>と同義です。
        /// 右被演算子は<see cref="Keyword(string, int)"/>によりパーサーに変換されます。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Parser<T> operator &(Parser<T> left, string right)
        {
            return left.Then(new KeywordParser(right));
        }
        /// <summary>
        /// <see cref="Parser.Then{T}(Parser{T})"/>と同義です。
        /// 左被演算子は<see cref="Keyword(string, int)"/>によりパーサーに変換されます。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
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
        /// パース成否は<see cref="ResultCore{T}"/>のインスタンスで表されます。
        /// このメソッドはいかなる場合も<c>null</c>を返してはなりません。
        /// またこのメソッドは原則として例外スローを行ってはなりません。
        /// 正常・異常を問わずこのメソッド内で起こったことはすべて
        /// <see cref="ResultCore{T}"/>を通じて呼び出し元に通知される必要があります。
        /// </para>
        /// </summary>
        /// <param name="input">入力データ</param>
        /// <returns>パース結果</returns>
        protected abstract ResultCore<T> DoParse(Reader input);

        /// <summary>
        /// パースを行います。
        /// <para>
        /// パース成否は戻り値の<see cref="Result{T}"/>のインスタンスで表されます。
        /// このメソッドはいかなる場合も<c>null</c>を返しません。
        /// またこのメソッドは原則として例外スローも行いません。
        /// 正常・異常を問わずこのメソッド内で起こったことはすべて
        /// <see cref="Result{T}"/>を通じて呼び出し元に通知されます。
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
        /// パース成功を表す<see cref="ResultCore{T}"/>インスタンスを生成します。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected ResultCore<T> Success(T value)
        {
            return ResultCore<T>.OfSuccess(value, true);
        }
        /// <summary>
        /// パース成功を表す<see cref="ResultCore{T}"/>インスタンスを生成します。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="canBacktrack">直近の<c>|</c>や<c>Or(...)</c>を
        /// 起点とするバックトラックを有効にするかどうか（デフォルトは<c>true</c>で、バックトラックは有効）</param>
        /// <returns></returns>
        protected ResultCore<T> Success(T value, bool canBacktrack)
        {
            return ResultCore<T>.OfSuccess(value, canBacktrack);
        }
        /// <summary>
        /// パース失敗を表す<see cref="ResultCore{T}"/>インスタンスを生成します。
        /// </summary>
        /// <param name="message">パース失敗の理由を示すメッセージ</param>
        /// <returns></returns>
        protected ResultCore<T> Failure(string message)
        {
            return ResultCore<T>.OfFailure(message, true);
        }
        /// <summary>
        /// パース失敗を表す<see cref="ResultCore{T}"/>インスタンスを生成します。
        /// </summary>
        /// <param name="message">パース失敗の理由を示すメッセージ</param>
        /// <param name="canBacktrack">直近の<c>|</c>や<c>Or(...)</c>を
        /// 起点とするバックトラックを有効にするかどうか（デフォルトは<c>true</c>で、バックトラックは有効）</param>
        /// <returns></returns>
        protected ResultCore<T> Failure(string message, bool canBacktrack)
        {
            return ResultCore<T>.OfFailure(message, canBacktrack);
        }
        void LogPreParse(CharPosition pos, int peek)
        {
            WriteLine("##### Pre-Parse #####");
            WriteLine("Parser     : {0} ", ParsecUtility.ObjectTypeToString(this));
            WriteLine("Position   : {0} ", pos);
            WriteLine("Char       : {0} ", ParsecUtility.CharToString(peek));
        }
        void LogPostParse(CharPosition pos, int peek, ResultCore<T> result)
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
        /// <returns></returns>
        protected Parser Not<U>(Parser<U> operand) => _factory.Not(operand);
        /// <summary>
        /// パーサーのパース結果成否を反転させるパーサーを生成します。
        /// </summary>
        /// <param name="operand">元になるパーサー</param>
        /// <returns></returns>
        protected Parser Not(Parser operand) => _factory.Not(operand);
        /// <summary>
        /// デリゲートをもとにパーサーを生成します。
        /// </summary>
        /// <typeparam name="U">任意の型</typeparam>
        /// <param name="func">パースの実処理を行うデリゲート</param>
        /// <returns></returns>
        protected Parser<U> For<U>(Func<Reader, Result<U>> func) => _factory.For(func);
        /// <summary>
        /// デリゲートをもとにパーサーを生成します。
        /// </summary>
        /// <param name="func">パースの実処理を行うデリゲート</param>
        /// <returns></returns>
        protected Parser For(Func<Reader, Result> func) => _factory.For(func);
        /// <summary>
        /// デリゲートを使用してパーサーを生成します。
        /// デリゲートはパースの直前になるまで実行されません。
        /// </summary>
        /// <typeparam name="U">パーサーが返す値の型</typeparam>
        /// <param name="factory">パーサーを生成するデリゲート</param>
        /// <returns></returns>
        protected Parser<U> Lazy<U>(Func<Parser<U>> factory) => _factory.Lazy(factory);
        /// <summary>
        /// デリゲートを使用してパーサーを生成します。
        /// デリゲートはパースの直前になるまで実行されません。
        /// </summary>
        /// <param name="factory">パーサーを生成するデリゲート</param>
        /// <returns></returns>
        protected Parser Lazy(Func<Parser> factory) => _factory.Lazy(factory);
        /// <summary>
        /// 先読み（look-ahead）を行うパーサーを生成します。
        /// <para>このパーサーはその成否に関わらず文字位置を前進させません。</para>
        /// </summary>
        /// <param name="operand">元になるパーサー</param>
        /// <returns></returns>
        protected Parser Lookahead(Parser operand) => _factory.Lookahead(operand);
        /// <summary>
        /// 指定された文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="ch">文字</param>
        /// <returns></returns>
        protected Parser Char(char ch) => _factory.Char(ch);
        /// <summary>
        /// 指定された範囲に該当する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="start">範囲の開始</param>
        /// <param name="end">範囲の終了</param>
        /// <returns></returns>
        protected Parser CharBetween(char start, char end) => _factory.CharBetween(start, end);
        /// <summary>
        /// 指定された文字クラスに属する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="clazz">文字クラス</param>
        /// <returns></returns>
        protected Parser CharIn(CharClass clazz) => _factory.CharIn(clazz);
        /// <summary>
        /// 指定された文字の集合に属する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="chars">文字集合</param>
        /// <returns></returns>
        protected Parser CharIn(IEnumerable<char> chars) => _factory.CharIn(chars);
        /// <summary>
        /// 文字範囲に該当する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="start">範囲の開始</param>
        /// <param name="end">範囲の終了</param>
        /// <param name="min">最小の文字数</param>
        /// <returns></returns>
        protected Parser CharsWhileBetween(char start, char end, int min = 1) => _factory.CharsWhileBetween(start, end, min);
        /// <summary>
        /// 文字集合に属する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="chars">文字集合</param>
        /// <param name="min">最小の文字数</param>
        /// <returns></returns>
        protected Parser CharsWhileIn(IEnumerable<char> chars, int min = 1) => _factory.CharsWhileIn(chars, min);
        /// <summary>
        /// 文字クラスに属する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="clazz">文字クラス</param>
        /// <param name="min">最小の文字数</param>
        /// <returns></returns>
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
        /// <returns></returns>
        protected Parser Keyword(string keyword, int cutIndex = -1) => _factory.Keyword(keyword, cutIndex);
        /// <summary>
        /// 指定したキーワードのいずれかにのみマッチするパーサーを生成します。
        /// </summary>
        /// <param name="keywords">キーワード</param>
        /// <returns></returns>
        protected Parser KeywordIn(params string[] keywords) => _factory.KeywordIn(keywords);
        /// <summary>
        /// 指定した値をキャプチャ結果とするパーサーを生成します。
        /// <para>このパーサーは実際には読み取りは行わず、パース結果は必ず成功となります。
        /// キャプチャを行わないパーサーに<c>&amp;</c>や<c>Then(...)</c>で結合することで、
        /// そのパーサーの代わりにキャプチャ結果を作り出すように働きます。</para>
        /// </summary>
        /// <typeparam name="U">任意の型</typeparam>
        /// <param name="value">キャプチャ結果となる値</param>
        /// <returns></returns>
        protected Parser<U> Yield<U>(U value) => _factory.Yield(value);
        #endregion

        /// <summary>
        /// 直近の<c>|</c>や<c>Or(...)</c>を起点としたバックトラックを無効化します。
        /// <para>
        /// レシーバーのパーサーが成功したあと後続のパーサーが失敗した場合バックトラックは機能せず、
        /// <c>|</c>や<c>Or(...)</c>で連結された他のパーサーの実行が試行されることはありません。
        /// このメソッドを呼び出す以前のパーサーが失敗した場合は引き続きバックトラックが有効です。
        /// </para>
        /// </summary>
        /// <returns>新しいパーサー</returns>
        public Parser<T> Cut()
        {
            return new CutParser<T>(Configuration, this);
        }
        /// <summary>
        /// パース結果の値を元に動的にパーサーを構築するパーサーを返します。
        /// </summary>
        /// <typeparam name="U">読み取り結果を変換した後の型</typeparam>
        /// <param name="mapper">元のパーサーの読み取り結果から動的にパーサーを生成する関数</param>
        /// <returns>新しいパーサー</returns>
        public Parser<U> FlatMap<U>(Func<T, Parser<U>> mapper)
        {
            return new FlatMapParser<T, U>(Configuration, this, mapper, false);
        }
        /// <summary>
        /// <see cref="FlatMap{U}(Func{T, Parser{U}})"/>とほぼ同義ですが、
        /// <paramref name="canThrow"/>に<c>true</c>を指定することで
        /// <paramref name="mapper"/>適用時にスローされた例外をそのまま再スローします。
        /// </summary>
        /// <typeparam name="U">読み取り結果を変換した後の型</typeparam>
        /// <param name="mapper">元のパーサーの読み取り結果から動的にパーサーを生成する関数</param>
        /// <param name="canThrow"><c>true</c>の場合<paramref name="mapper"/>がスローした例外をそのまま再スローする</param>
        /// <returns>新しいパーサー</returns>
        public Parser<U> FlatMap<U>(Func<T, Parser<U>> mapper, bool canThrow)
        {
            return new FlatMapParser<T, U>(Configuration, this, mapper, canThrow);
        }
        /// <summary>
        /// 読み取り結果値に関数を適用するパーサーを生成します。
        /// <para>
        /// このメソッドが返すパーサーは関数<paramref name="func"/>が例外をスローした場合、
        /// そのメッセージを使用してパース失敗を表す<see cref="Result{T}"/>インスタンスを返します。
        /// </para>
        /// </summary>
        /// <typeparam name="U">読み取り結果を変換した後の型</typeparam>
        /// <param name="func">変換を行う関数</param>
        /// <returns>新しいパーサー</returns>
        public Parser<U> Map<U>(Func<T, U> func)
        {
            return new MapParser<T, U>(Configuration, this, func, false);
        }
        /// <summary>
        /// <see cref="Map{U}(Func{T,U})"/>とほぼ同義ですが、
        /// <paramref name="canThrow"/>に<c>true</c>を指定することで
        /// <paramref name="func"/>適用時にスローされた例外をそのまま再スローします。
        /// </summary>
        /// <typeparam name="U">読み取り結果を変換した後の型</typeparam>
        /// <param name="func">変換を行う関数</param>
        /// <param name="canThrow"><c>true</c>の場合<paramref name="func"/>がスローした例外をそのまま再スローする</param>
        /// <returns>新しいパーサー</returns>
        public Parser<U> Map<U>(Func<T, U> func, bool canThrow)
        {
            return new MapParser<T, U>(Configuration, this, func, canThrow);
        }
        /// <summary>
        /// このパーサーの読み取りが失敗したときに実行されるパーサーを指定します。
        /// <para>
        /// このパーサー（レシーバーとなるパーサー）の読み取りが成功した場合は、
        /// その結果がそのまま新しいパーサーの返す結果となります。
        /// 一方、このパーサーの読み取りが失敗した場合は、データソースの読み取り位置はリセットされ（バックトラック）、
        /// 引数で指定されたもう1つのパーサーの読み取りが試行され、その結果が新しいパーサーの返す結果となります。
        /// </para>
        /// <para>演算子<c>|</c>と<c>Or(...)</c>系メソッドはいずれも左結合です。
        /// つまり<c>p0 | p1 | p2</c>や<c>p0.Or(p1).Or(p2)</c>というコードは、概念的には<c>(p0 | p1) | p2</c>と解釈されます。
        /// もし仮に<c>p0</c>構築中のいずれかの地点で<see cref="Cut"/>が実行されており当該地点以降でトラックバックが無効化されている場合、
        /// これ以降の区間でパースが失敗すると当然<c>p1</c>は実行されないとしても、<c>p2</c>は引き続き実行されるということです。
        /// あえてこの挙動を変えるには<c>p0 | (p1 | p2)</c>や<c>p0.Or(p1.Or(p2))</c>というコードに変更する必要があります。
        /// </para>
        /// </summary>
        /// <param name="another">別のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public Parser<T> Or(Parser<T> another)
        {
            return new OrParser<T>(Configuration, this, another);
        }
        /// <summary>
        /// このパーサーのパースが失敗した時、代わりに指定した値を返すパーサーを返します。
        /// </summary>
        /// <param name="value">代替値</param>
        /// <returns></returns>
        public Parser<T> OrElse(T value)
        {
            return Or(Yield(value));
        }
        /// <summary>
        /// このパーサーのパースの結果成否にかかわらずパース成功とみなす新しいパーサーを返します。
        /// </summary>
        /// <returns>新しいパーサー</returns>
        public Parser<Optional<T>> OrNot()
        {
            return new OptionalParser<T>(Configuration, this);
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
        public Parser<Seq<T>> Repeat(int min = 0, int max = -1, int exactly = -1, Parser sep = null)
        {
            return RepeatParser<T>.Create(this, min, max, exactly, sep);
        }
        /// <summary>
        /// パース対象に先行する指定された文字クラスをスキップするパーサーを返します。
        /// <para>新しいパーサーを元に生成される他のパーサーもこの設定を引き継ぎます。</para>
        /// </summary>
        /// <param name="target">スキップ対象の文字クラス</param>
        /// <returns>新しいパーサー</returns>
        public Parser<T> AutoSkip(CharClass target)
        {
            if (target == null) throw new ArgumentNullException(nameof(target));
            return new SkipSpaceParser<T>(Configuration, this, true, target);
        }
        /// <summary>
        /// パース対象に先行する空白文字をスキップするパーサーを返します。
        /// <para>新しいパーサーを元に生成される他のパーサーもこの設定を引き継ぎます。</para>
        /// </summary>
        /// <returns>新しいパーサー</returns>
        public Parser<T> AutoSkip()
        {
            return new SkipSpaceParser<T>(Configuration, this, true, CharClass.SpaceAndControl);
        }
        /// <summary>
        /// このパーサーのパースが成功したあと引数で指定した別のパーサーのパースを行う新しいパーサーを返します。
        /// <para>
        /// 例えば<c>var p2 = p0.Then(p1); p2.Parse(...);</c>というコードがあったとき、
        /// p0のパースが成功した場合は、引き続きp1のパースが実行されます。
        /// p1が成功した場合はp2の結果も成功となります。p1が失敗した場合はp2の結果も失敗です。
        /// p0が失敗した場合はp1は実行されず、p2の結果は失敗となります。
        /// </para>
        /// </summary>
        /// <param name="another">別のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public Parser<T> Then(Parser another)
        {
            return new ThenTakeLeftParser<T>(Configuration, this, another);
        }
        /// <summary>
        /// このパーサーのパースが成功したあと引数で指定した別のパーサーのパースを行う新しいパーサーを返します。
        /// <para>
        /// 例えば<c>var p2 = p0.Then(p1); p2.Parse(...);</c>というコードがあったとき、
        /// p0のパースが成功した場合は、引き続きp1のパースが実行されます。
        /// p1が成功した場合はp2の結果も成功となります。p1が失敗した場合はp2の結果も失敗です。
        /// p0が失敗した場合はp1は実行されず、p2の結果は失敗となります。
        /// </para>
        /// </summary>
        /// <param name="another">別のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public Parser<Tuple<T, U>> Then<U>(Parser<U> another)
        {
            return new DoubleParser<T, U>(Configuration, this, another);
        }
        /// <summary>
        /// このパーサーのパースが成功したあと引数で指定した別のパーサーのパースを行う新しいパーサーを返します。
        /// <para>
        /// 例えば<c>var p2 = p0.Then(p1); p2.Parse(...);</c>というコードがあったとき、
        /// p0のパースが成功した場合は、引き続きp1のパースが実行されます。
        /// p1が成功した場合はp2の結果も成功となります。p1が失敗した場合はp2の結果も失敗です。
        /// p0が失敗した場合はp1は実行されず、p2の結果は失敗となります。
        /// </para>
        /// </summary>
        /// <param name="another">別のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public Parser<Tuple<T, T2, T3>> Then<T2, T3>(Parser<Tuple<T2, T3>> another)
        {
            return TripleParser<T, T2, T3>.Create(this, another);
        }
        /// <summary>
        /// パース結果型を持たないパーサーに変換します。
        /// 元のパーサーがキャプチャした値は破棄されます。
        /// </summary>
        /// <returns></returns>
        public Parser Untyped()
        {
            return new UntypedParser<T>(this);
        }
    }
}
