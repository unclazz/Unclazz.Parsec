using System;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec.CoreParsers;
using static Unclazz.Parsec.Parsers;

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
    /// </summary>
    public abstract class Parser : IParser<Nil>
    {
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
            return For(func);
        }
        /// <summary>
        /// <see cref="Parser.Lazy{T}(Func{Parser{T}})"/>と同義です。
        /// </summary>
        /// <param name="factory">パーサーを返すデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public static implicit operator Parser<T>(Func<Parser<T>> factory)
        {
            return Lazy(factory);
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
            return Not<T>(operand);
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
