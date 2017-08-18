using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec.CharClasses;
using Unclazz.Parsec.CoreParsers;

namespace Unclazz.Parsec
{
    /// <summary>
    /// <see cref="Parser{T}"/>のコンパニオン・オブジェクトです。
    /// <see cref="Parser{T}"/>のインスタンスを生成するためのユーティリティとして機能します。
    /// </summary>
    public static class Parser
    {
        #region 定義済みパーサーを提供するプロパティの宣言
        /// <summary>
        /// データソースの先頭（BOF）にだけマッチするパーサーです。
        /// </summary>
        public static Parser<string> BeginningOfFile { get; } = new BeginningOfFileParser();
        /// <summary>
        /// データソースの終端（EOF）にだけマッチするパーサーです。
        /// </summary>
        public static Parser<string> EndOfFile { get; } = new EndOfFileParser();
        /// <summary>
        /// 0文字以上の空白文字(コードポイント<c>32</c>）と
        /// 制御文字（同<c>0</c>から<c>31</c>と<c>127</c>）にマッチするパーサーです。
        /// </summary>
        public static Parser<string> WhileSpaceAndControls { get; } =
            new CharsWhileInParser(CharClass.Between((char)0, (char)32) + (char)127, 0);
        /// <summary>
        /// 0文字以上の制御文字（同<c>0</c>から<c>31</c>と<c>127</c>）にマッチするパーサーです。
        /// </summary>
        public static Parser<string> WhileControls { get; } =
            new CharsWhileInParser(CharClass.Between((char)0, (char)31) + (char)127, 0);
        #endregion

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
        /// 指定された文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="ch">文字</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<string> Char(char ch)
        {
            return new SingleCharParser(ch);
        }
        /// <summary>
        /// 指定された範囲に該当する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="start">範囲の開始</param>
        /// <param name="end">範囲の終了</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<string> CharBetween(char start, char end)
        {
            return new CharClassParser(CharClass.Between(start, end));
        }
        /// <summary>
        /// 指定された文字クラスに属する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="clazz">文字クラス</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<string> CharIn(CharClass clazz)
        {
            return new CharClassParser(clazz);
        }
        /// <summary>
        /// 指定された文字の集合に属する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="chars">文字集合</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<string> CharIn(IEnumerable<char> chars)
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
        public static Parser<string> CharsWhileBetween(char start, char end, int min = 1)
        {
            return new CharsWhileBetweenParser(start, end, min);
        }
        /// <summary>
        /// 文字集合に属する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="chars">文字集合</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<string> CharsWhileIn(IEnumerable<char> chars, int min = 1)
        {
            return new CharsWhileInParser(CharClass.AnyOf(chars), min);
        }
        /// <summary>
        /// 文字クラスに属する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="clazz">文字クラス</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<string> CharsWhileIn(CharClass clazz, int min = 1)
        {
            return new CharsWhileInParser(clazz, min);
        }
        /// <summary>
        /// パーサーのパース結果成否を反転させるパーサーを生成します。
        /// <para>
        /// <see cref="Parser{T}.Cut"/>によるバックトラック可否設定は引き継がれます。
        /// </para>
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> Not<T>(Parser<T> operand)
        {
            return new NotParser<T>(operand);
        }
        /// <summary>
        /// パーサーのパース失敗時に結果を反転させるパーサーを生成します。
        /// <para>
        /// <see cref="Parser{T}.Cut"/>によるバックトラック可否設定は引き継がれます。
        /// </para>
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="parser">元になるパーサー</param>
        /// <returns></returns>
        public static Parser<T> Optional<T>(Parser<T> parser)
        {
            return new OptionalParser<T>(parser);
        }
        /// <summary>
        /// いずれか片方のパースが成功すれば全体の結果も成功とするパーサーを生成します。
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
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="left">左被演算子</param>
        /// <param name="right">右被演算子</param>
        /// <returns></returns>
        public static Parser<T> Or<T>(Parser<T> left, Parser<T> right)
        {
            return OrParser<T>.LeftAssoc(left, right);
        }
        public static Parser<T> OrRightAssoc<T>(Parser<T> left, Parser<T> right)
        {
            return OrParser<T>.RightAssoc(left, right);
        }
        /// <summary>
        /// 指定した文字列にのみマッチするパーサーを生成します。
        /// </summary>
        /// <param name="keyword">文字列</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<string> Keyword(string keyword, int cutIndex = -1)
        {
            return new KeywordParser(keyword, cutIndex);
        }
        public static Parser<string> StringIn(params string[] keywords)
        {
            return new StringInParser(keywords);
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
    public abstract class Parser<T>
    {
        #region 演算子オーバーロードの宣言
        /// <summary>
        /// デリゲートをもとにパーサーを生成します。
        /// </summary>
        /// <param name="func">デリゲート</param>
        public static implicit operator Parser<T>(Func<ParserInput, ParseResult<T>> func)
        {
            return Parser.For(func);
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
        /// いずれか片方のパースが成功すれば全体の結果も成功とするパーサーを生成します。
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
        /// <param name="left">元になるパーサー</param>
        /// <param name="right"><paramref name="left"/>のパース失敗時のみ使用されるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> operator |(Parser<T> left, Parser<T> right)
        {
            return OrParser<T>.LeftAssoc(left, right);
        }
        /// <summary>
        /// 左側のパーサーのパースが失敗したら右側の値をパース結果とするパーサーを生成します。
        /// </summary>
        /// <param name="left">元になるパーサー</param>
        /// <param name="right"><paramref name="left"/>のパース失敗時にパース結果として使用される値</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> operator |(Parser<T> left, T right)
        {
            return OrParser<T>.LeftAssoc(left, new SuccessParser<T>(right));
        }
        public static Parser<IEnumerable<T>> operator +(Parser<T> left, Parser<T> right)
        {
            return new AddParser<T>(left, right);
        }
        public static Parser<IEnumerable<T>> operator +(Parser<IEnumerable<T>> left, Parser<T> right)
        {
            return new ManyThenParser<T>(left, right);
        }
        public static Parser<string> operator &(Parser<T> left, Parser<T> right)
        {
            var leftStr = left as Parser<string>;
            if (leftStr != null)
            {
                var rightStr = right as Parser<string>;
                return leftStr.Concat(rightStr);
            }
            return left.Then(right).Cast<string>();
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
        /// 多くのパーサーでは<see cref="ParseResult{T}.Capture"/>プロパティが返す<see cref="Capture{T}"/>は値を含みません。
        /// 例外は<c>Parser&lt;string&gt;.Map&lt;T&gt;(T)</c>と<c>Parser&lt;char&gt;.Map&lt;T&gt;(T)</c>で、
        /// これらのメソッドが返すパーサーのパース結果は値を含んでいます。
        /// それ以外で値のキャプチャが必要な場合は<c>Parser&lt;string&gt;.Capture()</c>を使用します。
        /// </para>
        /// </summary>
        /// <param name="input">入力データ</param>
        /// <returns>パース結果</returns>
        public abstract ParseResult<T> Parse(ParserInput input);
        /// <summary>
        /// パース成功を表す<see cref="ParseResult{T}"/>インスタンスを生成します。
        /// </summary>
        /// <param name="position">パース開始時の文字位置</param>
        /// <param name="capture">パース結果を内包する可能性のある<see cref="Capture{T}"/>インスタンス</param>
        /// <param name="canBacktrack">直近の<see cref="Parser{T}.Or(Parser{T})"/>を
        /// 起点とするバックトラックを有効にするかどうか（デフォルトは<c>true</c>で、バックトラックは有効）</param>
        /// <returns>パース成功を表すインスタンス</returns>
        protected ParseResult<T> Success(CharacterPosition position, 
            Capture<T> capture = new Capture<T>(), bool canBacktrack = true)
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
        /// このメソッドが返すパーサーは<see cref="Parser{T}.Map{U}(Func{string, U}, bool)"/>が返すパーサーと異なり、
        /// 値のキャプチャを行いません。仮に元になるパーサーがキャプチャをサポートするものであっても、
        /// このメソッドが返す新しいパーサーはその値を破棄したからの<see cref="Capture{T}"/>を返すものとなります。
        /// </para>
        /// </summary>
        /// <typeparam name="U">任意の型</typeparam>
        /// <returns>新しいパーサー</returns>
        public Parser<U> Cast<U>()
        {
            return new CastParser<T, U>(this);
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
        /// このパーサーの読み取り結果を任意の関数で変換して返すパーサーを生成します。
        /// <para>
        /// このメソッドが生成して返すパーサーは、その目的ゆえにパース成功時に値を返します。
        /// パース結果の型を変更することだけが目的で実際にはその値を利用しない場合は<see cref="Cast{U}"/>を利用します。
        /// </para>
        /// <para>
        /// パース成功時（元になるパーサーがパースに成功した時）はキャプチャした値を引数にして<paramref name="transform"/>を呼び出します。
        /// パース失敗時（元になるパーサーがパースに失敗した時）は<paramref name="transform"/>は呼び出されません。
        /// </para>
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
        /// <param name="another"></param>
        /// <returns>バックトラック機能をサポートする新しいパーサー</returns>
        public Parser<T> Or(Parser<T> another)
        {
            return OrParser<T>.LeftAssoc(this, another);
        }
        public Parser<T> Or(Parser<T> another, params Parser<T>[] andOthers)
        {
            return OrParser<T>.LeftAssoc(this, another, andOthers);
        }
        public Parser<T> OrRightAssoc(Parser<T> another)
        {
            return OrParser<T>.RightAssoc(this, another);
        }
        public Parser<T> OrRightAssoc(Parser<T> another, params Parser<T>[] andOthers)
        {
            return OrParser<T>.RightAssoc(this, another, andOthers);
        }
        public Parser<T> OrNot()
        {
            return new OptionalParser<T>(this);
        }
        /// <summary>
        /// シーケンスを読み取るパーサーを生成します。
        /// </summary>
        /// <param name="min">繰り返しの最小回数</param>
        /// <param name="max">繰り返しの最大回数</param>
        /// <returns>繰り返しをサポートする新しいパーサー</returns>
        public Parser<IEnumerable<T>> Repeat(int min, int max, Parser<string> sep = null)
        {
            return new RepeatMinMaxParser<T>(this, min, max, sep);
        }
        /// <summary>
        /// シーケンスを読み取るパーサーを生成します。
        /// </summary>
        /// <param name="min">繰り返しの最小回数</param>
        /// <returns>繰り返しをサポートする新しいパーサー</returns>
        public Parser<IEnumerable<T>> RepeatMin(int min, Parser<string> sep = null)
        {
            return new RepeatMinMaxParser<T>(this, min, -1, sep);
        }
        /// <summary>
        /// シーケンスを読み取るパーサーを生成します。
        /// </summary>
        /// <param name="max">繰り返しの最大回数</param>
        /// <returns>繰り返しをサポートする新しいパーサー</returns>
        public Parser<IEnumerable<T>> RepeatMax(int max, Parser<string> sep = null)
        {
            return new RepeatMinMaxParser<T>(this, 0, max, sep);
        }
        /// <summary>
        /// シーケンスを読み取るパーサーを生成します。
        /// </summary>
        /// <param name="exactly">繰り返しの回数</param>
        /// <returns>繰り返しをサポートする新しいパーサー</returns>
        public Parser<IEnumerable<T>> RepeatExactly(int exactly, Parser<string> sep = null)
        {
            return new RepeatExactlyParser<T>(this, exactly, sep);
        }
        public Parser<U> Then<U>(Parser<U> another, params Parser<U>[] andOthers)
        {
            Parser<U> tmp = new ThenParser<T, U>(this, another);
            if (andOthers == null || andOthers.Length == 0) return tmp;

            ThenParser<U, U> tmp2 = null;
            foreach (var other in andOthers)
            {
                tmp2 = new ThenParser<U, U>(tmp, other);
            }
            return tmp2;
        }
        /// <summary>
        /// このパーサーの読み取りが成功したあとに実行されるパーサーを指定します。
        /// </summary>
        /// <param name="another">次に実行されるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public Parser<IEnumerable<T>> And(Parser<T> another, params Parser<T>[] andOthers)
        {
            Parser<IEnumerable<T>> p = new AddParser<T>(this, another);
            foreach (var o in andOthers)
            {
                p = new ManyThenParser<T>(p, o);
            }
            return p;
        }
    }
}
