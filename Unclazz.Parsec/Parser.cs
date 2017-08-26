using System;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec.CoreParsers;
using static Unclazz.Parsec.Parsers;

namespace Unclazz.Parsec
{
    /// <summary>
    /// パーサーを表す抽象クラスです。
    /// <para>
    /// この抽象クラスから派生した多くの抽象クラスと具象クラスが存在しています。
    /// 抽象クラスの1つ<see cref="NilParser"/>はパース結果の型が<see cref="Nil"/>であるパーサーです。
    /// <see cref="NilParser"/>はパースの成否判定だけを行うパーサーです。
    /// <see cref="Nil"/>は実際にはインスタンスを持たないクラスであり、
    /// パース結果<see cref="ParseResult{T}"/>はその成否にかかわらず常に値を持たないインスタンスです
    /// （<see cref="ParseResult{T}.Capture"/>が空のシーケンスを返す）。
    /// パース結果を文字列やその他の型のインスタンスとして取得する必要がある場合はこれ以外を使用します。
    /// </para>
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
            return For(func);
        }
        /// <summary>
        /// <see cref="Parsers.Lazy{T}(Func{Parser{T}})"/>と同義です。
        /// </summary>
        /// <param name="factory">パーサーを返すデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public static implicit operator Parser<T>(Func<Parser<T>> factory)
        {
            return Lazy(factory);
        }
        /// <summary>
        /// <see cref="Parsers.Not{T}(Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static NilParser operator !(Parser<T> operand)
        {
            return Not<T>(operand);
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
        public static Parser<T> operator |(Parser<T> left, NilParser right)
        {
            return OrParser<T>.LeftAssoc(left, right.Cast<T>());
        }
        /// <summary>
        /// <see cref="ParserExtension.Or{T}(Parser{T}, Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> operator |(NilParser left, Parser<T> right)
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
            return OrParser<T>.LeftAssoc(left, new PassParser<T>(right));
        }
        /// <summary>
        /// <see cref="ParserExtension.Then{T, U}(Parser{T}, Parser{U})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<Tuple<T, T>> operator &(Parser<T> left, Parser<T> right)
        {
            return new DoubleParser<T, T>(left, right);
        }
        /// <summary>
        /// <see cref="ParserExtension.Then{T}(NilParser, Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> operator &(NilParser left, Parser<T> right)
        {
            return new ThenTakeRightParser<Nil, T>(left, right);
        }
        /// <summary>
        /// <see cref="ParserExtension.Then{T}(Parser{T}, NilParser)"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> operator &(Parser<T> left, NilParser right)
        {
            return new ThenTakeLeftParser<T, Nil>(left, right);
        }
        #endregion

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
        public abstract ParseResult<T> Parse(Reader input);
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
            return new MapParser<T, U>(this, transform, canThrow);
        }
    }
}
