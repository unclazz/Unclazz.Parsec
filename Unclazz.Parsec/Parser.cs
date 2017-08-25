using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec.CharClasses;
using Unclazz.Parsec.CoreParsers;

namespace Unclazz.Parsec
{
    /// <summary>
    /// パーサーを表すインターフェースです。
    /// <para>
    /// このインターフェースの実装を宣言する2つの抽象クラスとそこから派生した多くの具象クラスが存在しています。
    /// 抽象クラスの1つ<see cref="Parser"/>はパース結果の型が<see cref="Nil"/>であるパーサーです。
    /// <see cref="Parser"/>はパースの成否判定だけを行うパーサーです。
    /// <see cref="Nil"/>は実際にはインスタンスを持たないクラスであり、
    /// <see cref="Parser.Parse(ParserInput)"/>が返す<see cref="ParseResult{T}"/>は
    /// パース結果の成否にかかわらず常に値を持たないインスタンスです（<see cref="ParseResult{T}.Capture"/>が空のシーケンスを返す）。
    /// パース結果を文字列やその他の型のインスタンスとして取得する必要がある場合は<see cref="Parser{T}"/>のインスタンスを使用します。
    /// この型のインスタンスは<see cref="Parser.Capture"/>メソッドや<see cref="Parser"/>および<see cref="Parser{T}"/>が公開する
    /// 各種のメンバーを利用して得ることができます。
    /// </para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IParser<T>
    {
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
        /// <para>
        /// <see cref="ParseResult{T}.Position"/>はパース開始時の文字位置を返します。
        /// 多くのパーサーでは<see cref="ParseResult{T}.Capture"/>プロパティが返す<see cref="Capture{T}"/>は値を含みません。
        /// 例外は<c>Parser&lt;string&gt;.Map&lt;T&gt;(T)</c>と<c>Parser&lt;char&gt;.Map&lt;T&gt;(T)</c>で、
        /// これらのメソッドが返すパーサーのパース結果は値を含んでいます。
        /// それ以外で値のキャプチャが必要な場合は<c>Parser&lt;string&gt;.Capture()</c>を使用します。
        /// </para>
        /// </summary>
        /// <param name="input">入力データ</param>
        /// <returns>パース結果</returns>
        ParseResult<T> Parse(ParserInput input);
    }

    /// <summary>
    /// <see cref="Parser{T}"/>のコンパニオン・オブジェクトです。
    /// <para>
    /// この抽象クラスのから派生した具象パーサー・クラスは値のキャプチャを一切行いません。
    /// メソッド<see cref="Parser.Parse(ParserInput)"/>はパースを行いその結果として<see cref="ParseResult{T}"/>を返しますが、
    /// パース結果の成否と関係なく、<see cref="ParseResult{T}.Capture"/>は必ず空のシーケンスになります。
    /// </para>
    /// <para>
    /// このクラスはまた<see cref="Parser"/>および<see cref="Parser{T}"/>の
    /// 派生クラスのインスタンスを生成するためのユーティリティとして機能します。
    /// </para>
    /// </summary>
    public abstract class Parser : IParser<Nil>
    {

        #region 定義済みパーサーを提供するプロパティの宣言
        /// <summary>
        /// データソースの先頭（BOF）にだけマッチするパーサーです。
        /// </summary>
        public static Parser BeginningOfFile { get; } = new BeginningOfFileParser();
        /// <summary>
        /// データソースの終端（EOF）にだけマッチするパーサーです。
        /// </summary>
        public static Parser EndOfFile { get; } = new EndOfFileParser();
        /// <summary>
        /// 0文字以上の空白文字(コードポイント<c>32</c>）と
        /// 制御文字（同<c>0</c>から<c>31</c>と<c>127</c>）にマッチするパーサーです。
        /// </summary>
        public static Parser WhileSpaceAndControls { get; } =
            new CharsWhileInParser(CharClass.Between((char)0, (char)32) + (char)127, 0);
        /// <summary>
        /// 0文字以上の制御文字（同<c>0</c>から<c>31</c>と<c>127</c>）にマッチするパーサーです。
        /// </summary>
        public static Parser WhileControls { get; } =
            new CharsWhileInParser(CharClass.Between((char)0, (char)31) + (char)127, 0);
        #endregion

        #region 演算子オーバーロードの宣言
        /// <summary>
        /// <see cref="Parser.Not{T}(Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいインスタンス</returns>
        public static Parser operator !(Parser operand)
        {
            return new NotParser<Nil>(operand).Cast();
        }
        /// <summary>
        /// <see cref="Parser.Then(Parser)"/>と同義です。
        /// </summary>
        /// <param name="left">元になるパーサー</param>
        /// <param name="right">元になるパーサー</param>
        /// <returns>新しいインスタンス</returns>
        public static Parser operator &(Parser left, Parser right)
        {
            return left.Then(right);
        }
        /// <summary>
        /// <see cref="Parser.Or(Parser)"/>と同義です。
        /// </summary>
        /// <param name="left">元になるパーサー</param>
        /// <param name="right">元になるパーサー</param>
        /// <returns>新しいインスタンス</returns>
        public static Parser operator |(Parser left, Parser right)
        {
            return OrParser<Nil>.LeftAssoc(left, right).Cast();
        }
        #endregion

        #region 静的ファクトリーメソッドの宣言
        /// <summary>
        /// パーサーのパース結果成否を反転させるパーサーを生成します。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> Not<T>(Parser<T> operand)
        {
            return new NotParser<T>(operand);
        }
        /// <summary>
        /// パーサーのパース結果成否を反転させるパーサーを生成します。
        /// </summary>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser Not(Parser operand)
        {
            return new NotParser<Nil>(operand).Cast();
        }
        /// <summary>
        /// デリゲートをもとにパーサーを生成します。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="func">パースの実処理を行うデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> For<T>(Func<ParserInput, ParseResult<T>> func)
        {
            return new DelegateParser<T>(func);
        }
        /// <summary>
        /// デリゲートをもとにパーサーを生成します。
        /// </summary>
        /// <param name="func">パースの実処理を行うデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public static Parser For(Func<ParserInput, ParseResult<Nil>> func)
        {
            return new DelegateParser<Nil>(func).Cast();
        }
        /// <summary>
        /// デリゲートを使用してパーサーを生成します。
        /// デリゲートはパースの直前になるまで実行されません。
        /// </summary>
        /// <typeparam name="T">パーサーが返す値の型</typeparam>
        /// <param name="factory">パーサーを生成するデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> Lazy<T>(Func<Parser<T>> factory)
        {
            return new LazyParser<T>(factory);
        }
        /// <summary>
        /// デリゲートを使用してパーサーを生成します。
        /// デリゲートはパースの直前になるまで実行されません。
        /// </summary>
        /// <param name="factory">パーサーを生成するデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public static Parser Lazy(Func<Parser> factory)
        {
            return new LazyParser<Nil>(factory).Cast();
        }
        /// <summary>
        /// パーサーのパース失敗時に結果を反転させるパーサーを生成します。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="parser">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> OrNot<T>(Parser<T> parser)
        {
            return new OrNotParser<T>(parser);
        }
        /// <summary>
        /// <see cref="OrNot{T}(Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="parser">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser OrNot(Parser parser)
        {
            return new OrNotParser<Nil>(parser).Cast();
        }
        /// <summary>
        /// <see cref="Parser{T}.Or(Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> Or<T>(Parser<T> left, Parser<T> right)
        {
            return OrParser<T>.LeftAssoc(left, right);
        }
        /// <summary>
        /// <see cref="Parser{T}.Or(Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="left">元になるパーサー</param>
        /// <param name="right">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser Or(Parser left, Parser right)
        {
            return OrParser<Nil>.LeftAssoc(left, right).Cast();
        }
        /// <summary>
        /// 指定された文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="ch">文字</param>
        /// <returns>新しいパーサー</returns>
        public static Parser Char(char ch)
        {
            return new CharParser(ch);
        }
        /// <summary>
        /// 指定された範囲に該当する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="start">範囲の開始</param>
        /// <param name="end">範囲の終了</param>
        /// <returns>新しいパーサー</returns>
        public static Parser CharBetween(char start, char end)
        {
            return new CharClassParser(CharClass.Between(start, end));
        }
        /// <summary>
        /// 指定された文字クラスに属する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="clazz">文字クラス</param>
        /// <returns>新しいパーサー</returns>
        public static Parser CharIn(CharClass clazz)
        {
            return new CharClassParser(clazz);
        }
        /// <summary>
        /// 指定された文字の集合に属する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="chars">文字集合</param>
        /// <returns>新しいパーサー</returns>
        public static Parser CharIn(IEnumerable<char> chars)
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
        public static Parser CharsWhileBetween(char start, char end, int min = 1)
        {
            return new CharsWhileBetweenParser(start, end, min);
        }
        /// <summary>
        /// 文字集合に属する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="chars">文字集合</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        public static Parser CharsWhileIn(IEnumerable<char> chars, int min = 1)
        {
            return new CharsWhileInParser(CharClass.AnyOf(chars), min);
        }
        /// <summary>
        /// 文字クラスに属する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="clazz">文字クラス</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        public static Parser CharsWhileIn(CharClass clazz, int min = 1)
        {
            return new CharsWhileInParser(clazz, min);
        }
        /// <summary>
        /// 指定したキーワードにのみマッチするパーサーを生成します。
        /// オプションのパラメータによりカット（トラックバックの無効化）を行う文字位置を指定できます。
        /// パース処理がこの文字位置の以降に進んだ時、
        /// 直前の<see cref="Parser{T}.Or(Parser{T})"/>を起点とするトラックバックは無効になります。
        /// </summary>
        /// <param name="keyword">キーワード</param>
        /// <param name="cutIndex">カットを行う文字位置</param>
        /// <returns>新しいパーサー</returns>
        public static Parser Keyword(string keyword, int cutIndex = -1)
        {
            return new KeywordParser(keyword, cutIndex);
        }
        /// <summary>
        /// 指定したキーワードのいずれかにのみマッチするパーサーを生成します。
        /// </summary>
        /// <param name="keywords">キーワード</param>
        /// <returns>新しいパーサー</returns>
        public static Parser StringIn(params string[] keywords)
        {
            return new StringInParser(keywords);
        }
        #endregion

        #region 具象クラス実装者のためのメンバーの宣言
        /// <summary>
        /// パースを行います。
        /// </summary>
        /// <param name="input">入力データ</param>
        /// <returns>パース結果</returns>
        public abstract ParseResult<Nil> Parse(ParserInput input);
        /// <summary>
        /// パース成功を表す<see cref="ParseResult{T}"/>インスタンスを生成します。
        /// </summary>
        /// <param name="position">パース開始時の文字位置</param>
        /// <param name="canBacktrack">直近の<see cref="Parser{T}.Or(Parser{T})"/>を
        /// 起点とするバックトラックを有効にするかどうか（デフォルトは<c>true</c>で、バックトラックは有効）</param>
        /// <returns>パース成功を表すインスタンス</returns>
        protected ParseResult<Nil> Success(CharacterPosition position, bool canBacktrack = true)
        {
            return ParseResult.OfSuccess<Nil>(position, canBacktrack: canBacktrack);
        }
        /// <summary>
        /// パース失敗を表す<see cref="ParseResult{T}"/>インスタンスを生成します。
        /// </summary>
        /// <param name="position">パース開始時の文字位置</param>
        /// <param name="message">パース失敗の理由を示すメッセージ</param>
        /// <param name="canBacktrack">直近の<see cref="Parser{T}.Or(Parser{T})"/>を
        /// 起点とするバックトラックを有効にするかどうか（デフォルトは<c>true</c>で、バックトラックは有効）</param>
        /// <returns>パース成功を表すインスタンス</returns>
        protected ParseResult<Nil> Failure(CharacterPosition position,
            string message, bool canBacktrack = true)
        {
            return ParseResult.OfFailure<Nil>(position, message, canBacktrack);
        }
        #endregion

        /// <summary>
        /// <see cref="Parser{T}.Capture"/>と同義です。
        /// </summary>
        /// <returns>新しいパーサー</returns>
        public Parser<string> Capture()
        {
            return new CaptureParser<Nil>(this);
        }
        /// <summary>
        /// <see cref="Parser{T}.Cast{U}"/>と同義です。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <returns>新しいパーサー</returns>
        public Parser<T> Cast<T>()
        {
            return new CastParser<Nil, T>(this);
        }
        /// <summary>
        /// <see cref="Parser{T}.Cut"/>と同義です。
        /// </summary>
        /// <returns>新しいパーサー</returns>
        public Parser Cut()
        {
            return new CutParser<Nil>(this).Cast();
        }
        /// <summary>
        /// <see cref="Parser{T}.Or(Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="another">別のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public Parser Or(Parser another)
        {
            return OrParser<Nil>.LeftAssoc(this, another).Cast();
        }
        /// <summary>
        /// <see cref="Parser{T}.Or(Parser{T}, Parser{T}[])"/>と同義です。
        /// </summary>
        /// <param name="another">別のパーサー</param>
        /// <param name="andOthers">その他のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public Parser Or(Parser another, params Parser[] andOthers)
        {
            return OrParser<Nil>.LeftAssoc(this, another, andOthers).Cast();
        }
        /// <summary>
        /// <see cref="Parser{T}.OrNot"/>と同義です。
        /// </summary>
        /// <returns>新しいパーサー</returns>
        public Parser OrNot()
        {
            return new OrNotParser<Nil>(this).Cast();
        }
        /// <summary>
        /// <see cref="Parser{T}.Repeat(int, int, int, Parser)"/>と同義です。
        /// </summary>
        /// <param name="min">繰り返しの最小回数</param>
        /// <param name="max">繰り返しの最大回数</param>
        /// <param name="exactly">繰り返しの回数</param>
        /// <param name="sep">セパレーターのためのパーサー</param>
        /// <returns>繰り返しをサポートする新しいパーサー</returns>
        public Parser Repeat(int min = 0, int max = -1, int exactly = -1, Parser sep = null)
        {
            return RepeatParser<Nil>.Create(this, min, max, exactly, sep).Cast();
        }
        /// <summary>
        /// <see cref="Parser{T}.Then{U}(Parser{U})"/>と同義です。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="another">別のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public Parser<T> Then<T>(Parser<T> another)
        {
            return new ThenTakeRightParser<Nil, T>(this, another);
        }
        /// <summary>
        /// <see cref="Parser{T}.Then{U}(Parser{U})"/>と同義です。
        /// </summary>
        /// <param name="another">別のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public Parser Then(Parser another)
        {
            return new ThenTakeRightParser<Nil, Nil>(this, another).Cast();
        }
        /// <summary>
        /// デバッグたのめパース処理前後の情報をログ出力するパーサーを返します。
        /// </summary>
        /// <param name="logger">ログ出力そのものを行うアクション</param>
        /// <returns>新しいパーサー</returns>
        public Parser Log(Action<string> logger)
        {
            return new LogParser<Nil>(this, logger).Cast();
        }
    }

    /// <summary>
    /// パーサーを表す抽象クラスです。
    /// <para>
    /// この抽象クラスを継承した具象クラスを独自実装することもできますが、
    /// <see cref="Parser"/>コンパニオン・オブジェクトの静的メンバーを通じて、
    /// 各種の定義済みパーサーのインスタンスを入手可能です。
    /// </para>
    /// </summary>
    /// <typeparam name="T">パース結果の型</typeparam>
    public abstract class Parser<T> : IParser<T>
    {
        #region 演算子オーバーロードの宣言
        /// <summary>
        /// <see cref="Parser.For{T}(Func{ParserInput, ParseResult{T}})"/>と同義です。
        /// </summary>
        /// <param name="func">パース処理を行うデリゲート</param>
        public static implicit operator Parser<T>(Func<ParserInput, ParseResult<T>> func)
        {
            return Parser.For(func);
        }
        /// <summary>
        /// <see cref="Parser.Lazy{T}(Func{Parser{T}})"/>と同義です。
        /// </summary>
        /// <param name="factory">パーサーを返すデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public static implicit operator Parser<T>(Func<Parser<T>> factory)
        {
            return Parser.Lazy(factory);
        }
        /// <summary>
        /// パーサーのパース失敗時に結果を反転させるパーサーを生成します。
        /// </summary>
        /// <para>
        /// <see cref="Parser{T}.Cut"/>によるバックトラック可否設定は引き継がれます。
        /// </para>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> operator !(Parser<T> operand)
        {
            return Parser.Not<T>(operand);
        }
        /// <summary>
        /// <see cref="Parser{T}.Or(Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> operator |(Parser<T> left, Parser<T> right)
        {
            return OrParser<T>.LeftAssoc(left, right);
        }
        /// <summary>
        /// <see cref="Parser{T}.Or(Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> operator |(Parser<T> left, Parser right)
        {
            return OrParser<T>.LeftAssoc(left, right.Cast<T>());
        }
        /// <summary>
        /// <see cref="Parser{T}.Or(Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> operator |(Parser left, Parser<T> right)
        {
            return OrParser<T>.LeftAssoc(left.Cast<T>(), right);
        }
        /// <summary>
        /// <see cref="Parser{T}.Or(Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> operator |(Parser<T> left, T right)
        {
            return OrParser<T>.LeftAssoc(left, new PassParser<T>(right));
        }
        /// <summary>
        /// <see cref="Parser{T}.Then{U}(Parser{U})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<Tuple<T, T>> operator &(Parser<T> left, Parser<T> right)
        {
            return new DoubleParser<T, T>(left, right);
        }
        /// <summary>
        /// <see cref="Parser{T}.Then{U}(Parser{U})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> operator &(Parser left, Parser<T> right)
        {
            return new ThenTakeRightParser<Nil, T>(left, right);
        }
        /// <summary>
        /// <see cref="Parser{T}.Then{U}(Parser{U})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> operator &(Parser<T> left, Parser right)
        {
            return new ThenTakeLeftParser<T, Nil>(left, right);
        }
        #endregion

        #region 具象クラス実装者のためのメンバーの宣言
        /// <summary>
        /// パースを行います。
        /// <para>
        /// パーサーの具象クラスを実装する場合、このメソッドを実装する必要があります。
        /// パース成否は<see cref="ParseResult{T}"/>のインスタンスで表されます。
        /// このメソッドはいかなる場合も<c>null</c>を返してはなりません。
        /// またこのメソッドはいかなる場合も例外スローを行ってはなりません。
        /// 正常・異常を問わずこのメソッド内で起こったことはすべて
        /// <see cref="ParseResult{T}"/>を通じて呼び出し元に通知される必要があります。
        /// </para>
        /// <para>
        /// <see cref="ParseResult{T}.Position"/>はパース開始時の文字位置を返します。
        /// 多くのパーサーはパースした値をキャプチャしません。
        /// これらのパーサーのパース結果の<see cref="ParseResult{T}.Capture"/>プロパティが返す<see cref="Capture{T}"/>は値を含みません。
        /// <see cref="Parser{T}.Map{U}(Func{T, U}, bool)"/>は、元になるパーサー（レシーバー）が
        /// 値をキャプチャするものである場合のみ、値を返すパーサーを生成して返します。
        /// 値のキャプチャが必要な場合は<see cref="Parser{T}.Capture"/>を使用します。
        /// </para>
        /// </summary>
        /// <param name="input">入力データ</param>
        /// <returns>パース結果</returns>
        public abstract ParseResult<T> Parse(ParserInput input);
        /// <summary>
        /// パース成功を表す<see cref="ParseResult{T}"/>インスタンスを生成します。
        /// </summary>
        /// <param name="position">パース開始時の文字位置</param>
        /// <param name="capture">パースされた値を内包する可能性のある<see cref="Capture{T}"/>インスタンス</param>
        /// <param name="canBacktrack">直近の<see cref="Parser{T}.Or(Parser{T})"/>を
        /// 起点とするバックトラックを有効にするかどうか（デフォルトは<c>true</c>で、バックトラックは有効）</param>
        /// <returns>パース成功を表すインスタンス</returns>
        protected ParseResult<T> Success(CharacterPosition position,
            Capture<T> capture = new Capture<T>(), bool canBacktrack = true)
        {
            return ParseResult.OfSuccess(position, capture, canBacktrack);
        }
        /// <summary>
        /// パース成功を表す<see cref="ParseResult{T}"/>インスタンスを生成します。
        /// </summary>
        /// <param name="position">パース開始時の文字位置</param>
        /// <param name="value">パースされた値</param>
        /// <param name="canBacktrack">直近の<see cref="Parser{T}.Or(Parser{T})"/>を
        /// 起点とするバックトラックを有効にするかどうか（デフォルトは<c>true</c>で、バックトラックは有効）</param>
        /// <returns>パース成功を表すインスタンス</returns>
        protected ParseResult<T> Success(CharacterPosition position, T value, bool canBacktrack = true)
        {
            return ParseResult.OfSuccess(position, value, canBacktrack);
        }
        /// <summary>
        /// パース失敗を表す<see cref="ParseResult{T}"/>インスタンスを生成します。
        /// </summary>
        /// <param name="position">パース開始時の文字位置</param>
        /// <param name="message">パース失敗の理由を示すメッセージ</param>
        /// <param name="canBacktrack">直近の<see cref="Parser{T}.Or(Parser{T})"/>を
        /// 起点とするバックトラックを有効にするかどうか（デフォルトは<c>true</c>で、バックトラックは有効）</param>
        /// <returns>パース成功を表すインスタンス</returns>
        protected ParseResult<T> Failure(CharacterPosition position, 
            string message, bool canBacktrack = true)
        {
            return ParseResult.OfFailure<T>(position, message, canBacktrack);
        }
        #endregion

        /// <summary>
        /// このパーサーの読み取り結果をキャプチャするパーサーを生成します。
        /// <para>
        /// パース処理そのものはこのパーサー（レシーバー）に委譲されます。
        /// ただしこのパーサーが本来返す値の型がなんであれ、パース開始から終了（パース成功）までの区間のデータはあくまでも
        /// <see cref="string"/>としてキャプチャされ、それがラッパーとなる新しいパーサーが返す値となります。</para>
        /// <para>
        /// 内部的な動作はおおよそ次のように進みます。
        /// パース処理本体が実行される前に<see cref="ParserInput.Mark"/>が呼び出されます。
        /// パース処理本体が成功した場合は<see cref="ParserInput.Capture(bool)"/>が呼び出されます。
        /// パース処理本体が失敗した場合は単に<see cref="ParserInput.Unmark"/>が呼び出されます。</para>
        /// </summary>
        /// <returns>キャプチャ機能をサポートする新しいパーサー</returns>
        public Parser<string> Capture()
        {
            return new CaptureParser<T>(this);
        }
        /// <summary>
        /// このパーサーの読み取り結果型を変更した新しいパーサーを生成します。
        /// <para>
        /// このメソッドが返すパーサーは<see cref="Parser{T}.Map{U}(Func{T, U}, bool)"/>が返すパーサーと異なり、
        /// 値のキャプチャを行いません。仮に元になるパーサーがキャプチャをサポートするものであっても、
        /// このメソッドが返す新しいパーサーは空の<see cref="Capture{T}"/>を返すものとなります。
        /// </para>
        /// </summary>
        /// <typeparam name="U">任意の型</typeparam>
        /// <returns>新しいパーサー</returns>
        public Parser<U> Cast<U>()
        {
            return new CastParser<T, U>(this);
        }
        /// <summary>
        /// <see cref="Cast{U}"/>と同義です。
        /// </summary>
        /// <returns>新しいパーサー</returns>
        public Parser Cast()
        {
            return new CastParser<T>(this);
        }
        /// <summary>
        /// 直近の<see cref="Parser{T}.Or(Parser{T})"/>を起点としたバックトラックを無効化します。
        /// <para>
        /// このパーサーが成功したあと後続のパーサーが失敗した場合バックトラックは機能せず、
        /// <see cref="Parser{T}.Or(Parser{T})"/>で連結された他のパーサーの実行が試行されることはありません。
        /// もちろんこのメソッドを呼び出す以前のパーサーが失敗した場合は引き続きバックトラックが有効です。
        /// </para>
        /// </summary>
        /// <returns>バックトラック機能が無効化された新しいパーサー</returns>
        public Parser<T> Cut()
        {
            return new CutParser<T>(this);
        }
        /// <summary>
        /// 読み取り結果の<see cref="Capture{T}"/>が内包する各要素に関数を提供するパーサーを生成します。
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
            return new MapParser<T, U>(this, transform, canThrow);
        }
        /// <summary>
        /// このパーサーの読み取りが失敗したときに実行されるパーサーを指定します。
        /// <para>
        /// このパーサー（レシーバーとなるパーサー）の読み取りが成功した場合は、
        /// その結果がそのまま新しいパーサーの返す結果となります。
        /// 一方、このパーサーの読み取りが失敗した場合は、データソースの読み取り位置はリセットされ（バックトラック）、
        /// 引数で指定されたもう1つのパーサーの読み取りが試行され、その結果が新しいパーサーの返す結果となります。
        /// </para>
        /// <para>演算子<c>|</c>とインスタンス・メソッド<see cref="Parser{T}.Or(Parser{T})"/>のグループと
        /// 静的メソッド<see cref="Parser.Or{T}(Parser{T}, Parser{T})"/>はいずれも右結合です。
        /// つまり<c>p0 | p1 | p2</c>や<c>p0.Or(p1).Or(p2)</c>というコードは、概念的には<c>(p0 | p1) | p2</c>と解釈されます。
        /// もし仮に<c>p0</c>構築中のいずれかの地点で<see cref="Cut"/>が実行されており当該地点以降でトラックバックが無効化されている場合、
        /// これ以降の区間でパースが失敗すると当然<c>p1</c>は実行されないとしても、<c>p2</c>は引き続き実行されるということです。
        /// あえてこの挙動を変えるには<c>p0 | (p1 | p2)</c>や<c>p0.Or(p1.Or(p2))</c>というコードに変更する必要があります。
        /// </para>
        /// </summary>
        /// <param name="another">別のパーサー</param>
        /// <returns>バックトラック機能をサポートする新しいパーサー</returns>
        public Parser<T> Or(Parser<T> another)
        {
            return OrParser<T>.LeftAssoc(this, another);
        }
        /// <summary>
        /// <see cref="Parser{T}.Or(Parser{T})"/>と同義ですが、
        /// 複数のパーサーを一括指定することができます。
        /// </summary>
        /// <param name="another">別のパーサー</param>
        /// <param name="andOthers">その他のパーサー</param>
        /// <returns>バックトラック機能をサポートする新しいパーサー</returns>
        public Parser<T> Or(Parser<T> another, params Parser<T>[] andOthers)
        {
            return OrParser<T>.LeftAssoc(this, another, andOthers);
        }
        /// <summary>
        /// このパーサーのパースの結果成否にかかわらずパース成功とみなす新しいパーサーを返します。
        /// </summary>
        /// <returns>新しいパーサー</returns>
        public Parser<T> OrNot()
        {
            return new OrNotParser<T>(this);
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
        public Parser<IList<T>> Repeat(int min = 0, int max = -1, int exactly = -1, Parser sep = null)
        {
            return RepeatParser<T>.Create(this, min, max, exactly, sep);

        }
        /// <summary>
        /// このパーサーのパースが成功したあと引数で指定した別のパーサーのパースを行う新しいパーサーを返します。
        /// <para>
        /// 例えば<c>var p2 = p0.Then(p1); p2.Parse(...);</c>というコードがあったとき、
        /// p0のパースが成功した場合は、引き続きp1のパースが実行されます。
        /// p1が成功した場合はp2の結果も成功となります。p1が失敗した場合はp2の結果も失敗です。
        /// p0が失敗した場合はp1は実行されず、p2の結果は失敗となります。
        /// </para>
        /// <para>
        /// 元になるパーサー（p0やp1）が値をキャプチャしないパーサーであれば
        /// 新しいパーサー（p2）もまた値をキャプチャしないパーサーとなります。
        /// 元になるパーサーが値をキャプチャするパーサーであれば
        /// 新しいパーサーもまた値をキャプチャするパーサーとなります。
        /// </para>
        /// </summary>
        /// <param name="another">別のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public Parser<Tuple<T,U>> Then<U>(Parser<U> another)
        {
            return new DoubleParser<T,U>(this, another);
        }
        public Parser<Tuple<T, T2, T3>> Then<T2, T3>(Parser<Tuple<T2, T3>> another)
        {
            return new AndDoubleParser<T, T2, T3>(this, another);
        }
        /// <summary>
        /// <see cref="Parser{T}.Then{U}(Parser{U})"/>と同義です。
        /// </summary>
        /// <param name="another">別のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public Parser<T> Then(Parser another)
        {
            return new ThenTakeLeftParser<T, Nil>(this, another);
        }
        /// <summary>
        /// デバッグたのめパース処理前後の情報をログ出力するパーサーを返します。
        /// </summary>
        /// <param name="logger">ログ出力そのものを行うアクション</param>
        /// <returns>新しいパーサー</returns>
        public Parser<T> Log(Action<string> logger)
        {
            return new LogParser<T>(this, logger);
        }
    }

    public static class ParserExtension
    {
        public static Parser<Tuple<T1, T2, T3>> Then<T1, T2, T3>(this Parser<Tuple<T1, T2>> self, Parser<T3> another)
        {
            return new DoubleAndParser<T1, T2, T3>(self, another);
        }
    }
}
