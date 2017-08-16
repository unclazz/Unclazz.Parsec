using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec.CharClasses;

namespace Unclazz.Parsec
{
    /// <summary>
    /// <see cref="Parser{T}"/>のコンパニオン・オブジェクトです。
    /// <see cref="Parser{T}"/>のインスタンスを生成するためのユーティリティとして機能します。
    /// </summary>
    public static class Parser
    {
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
        /// <paramref name="left"/>のパース失敗時のみ<paramref name="right"/>のパースが試みられます。
        /// 成功したパーサーのパース結果が全体のパース結果となります。
        /// <see cref="Parser{T}.Cut"/>によるバックトラック可否設定は引き継がれます。
        /// </para>
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Parser<T> Or<T>(Parser<T> left, Parser<T> right)
        {
            return new OrParser<T>(left, right);
        }
        public static Parser<char> CharBetween(char start, char end)
        {
            return new CharClassParser(CharClass.Between(start, end));
        }
        public static Parser<char> CharIn(CharClass clazz)
        {
            return new CharClassParser(clazz);
        }
        public static Parser<char> CharIn(params char[] chars)
        {
            if (chars != null && chars.Length == 1) return new SingleCharParser(chars[0]);
            return new CharClassParser(CharClass.AnyOf(chars));
        }
        public static Parser<char> CharIn(IEnumerable<char> chars)
        {
            return new CharClassParser(CharClass.AnyOf(chars));
        }
        public static Parser<char> Char(char ch)
        {
            return new SingleCharParser(ch);
        }
        public static Parser<string> CharsWhileIn(IEnumerable<char> chars, int min = 1)
        {
            return new CharsWhileInParser(CharClass.AnyOf(chars), min);
        }
        public static Parser<string> CharsWhileIn(CharClass clazz, int min = 1)
        {
            return new CharsWhileInParser(clazz, min);
        }
        public static Parser<string> CharsWhileBetween(char start, char end, int min = 1)
        {
            return new CharsWhileBetweenParser(start, end, min);
        }
        /// <summary>
        /// 指定した文字列にのみマッチするパーサーを生成します。
        /// </summary>
        /// <param name="word">文字列</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<string> Word(string word)
        {
            return new WordParser(word);
        }

        /// <summary>
        /// データソースの先頭（BOF）にだけマッチするパーサーです。
        /// </summary>
        public static Parser<string> BeginningOfFile { get; } = new BeginningOfFileParser();
        /// <summary>
        /// データソースの終端（EOF）にだけマッチするパーサーです。
        /// </summary>
        public static Parser<string> EndOfFile { get; } = new EndOfFileParser();
        /// <summary>
        /// 0文字以上の空白文字(コードポイント<c>32</c>）と制御文字（同<c>0</c>から<c>31</c>と<c>127</c>）にマッチするパーサーです。
        /// </summary>
        public static Parser<string> WhileSpaceAndControls { get; } =
            new WhileCharClassParser(CharClass.Between((char)0, (char)32) + (char)127);
        /// <summary>
        /// 0文字以上の制御文字（同<c>0</c>から<c>31</c>と<c>127</c>）にマッチするパーサーです。
        /// </summary>
        public static Parser<string> WhileControls { get; } =
            new WhileCharClassParser(CharClass.Between((char)0, (char)31) + (char)127);

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
        /// <paramref name="left"/>のパース失敗時のみ<paramref name="right"/>のパースが試みられます。
        /// 成功したパーサーのパース結果が全体のパース結果となります。
        /// <see cref="Parser{T}.Cut"/>によるバックトラック可否設定は引き継がれます。
        /// </para>
        /// </summary>
        /// <param name="left">元になるパーサー</param>
        /// <param name="right"><paramref name="left"/>のパース失敗時のみ使用されるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> operator |(Parser<T> left, Parser<T> right)
        {
            return Parser.Or<T>(left, right);
        }
        /// <summary>
        /// 左側のパーサーのパースが失敗したら右側の値をパース結果とするパーサーを生成します。
        /// </summary>
        /// <param name="left">元になるパーサー</param>
        /// <param name="right"><paramref name="left"/>のパース失敗時にパース結果として使用される値</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> operator |(Parser<T> left, T right)
        {
            return left.Or(new SuccessParser<T>(right));
        }
        /// <summary>
        /// 左側のパーサーのパース成功時に右側のパーサーのパースを行うパーサーを生成します。
        /// <para>
        /// 新しいパーサーがパース結果として返す値は、
        /// それぞれのパーサーのパース結果を内容とするシーケンスになります。
        /// </para>
        /// </summary>
        /// <param name="left">元になるパーサー</param>
        /// <param name="right"><paramref name="left"/>のパース成功時に実行されるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<IEnumerable<T>> operator +(Parser<T> left, Parser<T> right)
        {
            return left.Then(right);
        }
        /// <summary>
        /// 左側のパーサーのパース成功時に右側のパーサーのパースを行うパーサーを生成します。
        /// <para>
        /// 新しいパーサーがパース結果として返す値は、
        /// それぞれのパーサーのパース結果を内容とするシーケンスになります。
        /// </para>
        /// </summary>
        /// <param name="left">元になるパーサー</param>
        /// <param name="right"><paramref name="left"/>のパース成功時に実行されるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<IEnumerable<T>> operator +(Parser<IEnumerable<T>> left, Parser<T> right)
        {
            return new ManyThenParser<T>(left, right);
        }
        /// <summary>
        /// 左側のパーサーのパース成功時に右側のパーサーのパースを行うパーサーを生成します。
        /// <para>
        /// 新しいパーサーがパース結果として返す値は、
        /// それぞれのパーサーのパース結果を内容とするシーケンスになります。
        /// </para>
        /// </summary>
        /// <param name="left">元になるパーサー</param>
        /// <param name="right"><paramref name="left"/>のパース成功時に実行されるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<IEnumerable<T>> operator +(Parser<T> left, Parser<IEnumerable<T>> right)
        {
            return new ThenManyParser<T>(left, right);
        }

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

        public Parser<IEnumerable<T>> Repeat(int min, int max)
        {
            return new RepeatMinMaxParser<T>(this, min, max);
        }
        public Parser<IEnumerable<T>> RepeatMin(int min)
        {
            return new RepeatMinMaxParser<T>(this, min, -1);
        }
        public Parser<IEnumerable<T>> RepeatMax(int max)
        {
            return new RepeatMinMaxParser<T>(this, 0, max);
        }
        public Parser<IEnumerable<T>> RepeatExactly(int exactly)
        {
            return new RepeatExactlyParser<T>(this, exactly);
        }
        public Parser<T> Or(Parser<T> another)
        {
            return new OrParser<T>(this, another);
        }
        public Parser<IEnumerable<T>> Then(Parser<T> another)
        {
            return new ThenParser<T>(this, another);
        }
        public Parser<T> Cut()
        {
            return new CutParser<T>(this);
        }
    }
}
