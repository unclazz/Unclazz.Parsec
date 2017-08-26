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
        /// 多くのパーサーでは<see cref="ParseResult{T}.Capture"/>プロパティが返す<see cref="Optional{T}"/>は値を含みません。
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
            return new CaptureParser<Nil>(this);
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
        /// これらのパーサーのパース結果の<see cref="ParseResult{T}.Capture"/>プロパティが返す<see cref="Optional{T}"/>は値を含みません。
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
        /// <param name="capture">パースされた値を内包する可能性のある<see cref="Optional{T}"/>インスタンス</param>
        /// <param name="canBacktrack">直近の<see cref="Parser{T}.Or(Parser{T})"/>を
        /// 起点とするバックトラックを有効にするかどうか（デフォルトは<c>true</c>で、バックトラックは有効）</param>
        /// <returns>パース成功を表すインスタンス</returns>
        protected ParseResult<T> Success(CharacterPosition position,
            Optional<T> capture = new Optional<T>(), bool canBacktrack = true)
        {
            return ParseResult.OfSuccess(position, capture, canBacktrack);
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
            return new MapParser<T, U>(this, transform, canThrow);
        }
    }
}
