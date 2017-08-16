using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec
{
    /// <summary>
    /// <see cref="ParseResult{T}"/>のコンパニオン・オブジェクトです。
    /// <see cref="ParseResult{T}"/>のインスタンスを生成するためのユーティリティとして機能します。
    /// </summary>
    public static class ParseResult
    {
        /// <summary>
        /// パース成功を表す<see cref="ParseResult{T}"/>インスタンスを返します。
        /// </summary>
        /// <typeparam name="T">パース結果の型</typeparam>
        /// <param name="position">パース開始時の文字位置</param>
        /// <param name="value">パース結果の値</param>
        /// <param name="canBacktrack">直近の<see cref="Parser{T}.Or(Parser{T})"/>を起点としたバックトラックを無効化する</param>
        /// <returns><see cref="ParseResult{T}"/>インスタンス</returns>
        public static ParseResult<T> OfSuccess<T>(CharacterPosition position, T value, bool canBacktrack = true)
        {
            return new ParseResult<T>(true, position, value, null, !canBacktrack);
        }
        /// <summary>
        /// パース成功を表す<see cref="ParseResult{T}"/>インスタンスを返します。
        /// </summary>
        /// <typeparam name="T">パース結果の型</typeparam>
        /// <param name="position">パース開始時の文字位置</param>
        /// <param name="capture">パース結果の値</param>
        /// <param name="canBacktrack">直近の<see cref="Parser{T}.Or(Parser{T})"/>
        /// を起点としたバックトラックを無効化する場合<c>false</c></param>
        /// <returns><see cref="ParseResult{T}"/>インスタンス</returns>
        public static ParseResult<T> OfSuccess<T>(CharacterPosition position, 
            Capture<T> capture = new Capture<T>(), bool canBacktrack = true)
        {
            return new ParseResult<T>(true, position, capture, null, !canBacktrack);
        }
        /// <summary>
        /// パース失敗を表す<see cref="ParseResult{T}"/>インスタンスを返します。
        /// </summary>
        /// <typeparam name="T">パース結果の型</typeparam>
        /// <param name="position">パース開始時の文字位置</param>
        /// <param name="message">パース失敗の理由を示すメッセージ</param>
        /// <param name="canBacktrack">直近の<see cref="Parser{T}.Or(Parser{T})"/>を起点としたバックトラックを無効化する場合<c>false</c></param>
        /// <returns><see cref="ParseResult{T}"/>インスタンス</returns>
        public static ParseResult<T> OfFailure<T>(CharacterPosition position, string message, bool canBacktrack = true)
        {
            return new ParseResult<T>(false, position, default(T), message, !canBacktrack);
        }
    }

    /// <summary>
    /// パース結果を表すクラスです。
    /// インスタンスはコンパニオン・オブジェクト<see cref="ParseResult"/>の提供する静的ファクトリーメソッドにより得られます。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct ParseResult<T> : IEnumerable<Capture<T>>
    {
        static readonly IEnumerable<Capture<T>> _empty = new Capture<T>[0];

        internal ParseResult(bool s, CharacterPosition p, string m, bool c) : this(s, p, new Capture<T>(), m, c) { }
        internal ParseResult(bool s, CharacterPosition p, T v, string m, bool c) : this(s, p, new Capture<T>(v), m, c) { }
        internal ParseResult(bool s, CharacterPosition p, Capture<T> c, string m, bool cut)
        {
            _capture = c;
            _message = m;
            Successful = s;
            Position = p;
            _cut = cut;
        }

        readonly Capture<T> _capture;
        readonly string _message;
        readonly bool _cut;

        /// <summary>
        /// パース結果を格納する<see cref="Capture{T}"/>インスタンスです。
        /// パースが失敗している場合は例外をスローします。
        /// </summary>
        public Capture<T> Capture
        {
            get
            {
                if (Successful) return _capture;
                else throw new InvalidOperationException("No capture. Because parsing has failed.");
            }
        }
        /// <summary>
        /// パース開始時の文字位置です。
        /// </summary>
        public CharacterPosition Position { get; }
        /// <summary>
        /// パース失敗の理由を示すメッセージです。
        /// パースが成功している場合は例外をスローします。
        /// </summary>
        public string Message
        {
            get
            {
                if (Successful) throw new InvalidOperationException("No message. Because parsing has benn successful.");
                else return _message;
            }
        }
        /// <summary>
        /// パースが成功している場合<c>true</c>です。
        /// </summary>
        public bool Successful { get; }
        /// <summary>
        /// 直近の<see cref="Parser{T}.Or(Parser{T})"/>を起点としたバックトラックが可能かどうかを示します。
        /// </summary>
        public bool CanBacktrack => !_cut;

        /// <summary>
        /// 指定された値を結び付けた新しいインスタンスを返します。
        /// </summary>
        /// <param name="value">値</param>
        /// <returns>新しいインスタンス</returns>
        public ParseResult<T> Attach(T value)
        {
            return new ParseResult<T>(Successful, Position, value, _message, _cut);
        }
        /// <summary>
        /// 値を除去した新しいインスタンスを返します。
        /// </summary>
        /// <returns>新しいインスタンス</returns>
        public ParseResult<T> Detach()
        {
            return new ParseResult<T>(Successful, Position, _message, _cut);
        }
        /// <summary>
        /// 直近の<see cref="Parser{T}.Or(Parser{T})"/>を起点とする
        /// バックトラックの設定を変更した新しいインスタンスを返します。
        /// </summary>
        /// <param name="allow">バックトラックを可能にする場合は<c>true</c></param>
        /// <returns>新しいインスタンス</returns>
        public ParseResult<T> AllowBacktrack(bool allow)
        {
            return new ParseResult<T>(Successful, Position, _capture, _message, !allow);
        }
        /// <summary>
        /// パースが成功している場合は引数で指定されたアクションを実行します。
        /// </summary>
        /// <param name="act">アクション</param>
        public void IfSuccessful(Action<Capture<T>> act)
        {
            if (Successful) (act ?? throw new ArgumentNullException(nameof(act)))(Capture);
        }
        /// <summary>
        /// パースが成功している場合は引数で指定されたアクションを実行します。
        /// </summary>
        /// <param name="act">アクション</param>
        public void IfSuccessful(Action<CharacterPosition, Capture<T>> act)
        {
            if (Successful) (act ?? throw new ArgumentNullException(nameof(act)))(Position, Capture);
        }
        /// <summary>
        /// パースが成功している場合は第1引数で指定されたアクションを実行します。
        /// さもなくば第2引数で指定されたアクションを実行します。
        /// </summary>
        /// <param name="act">成功している場合に実行されるアクション</param>
        /// <param name="orElse">失敗している場合に実行されるアクション</param>
        public void IfSuccessful(Action<Capture<T>> act, Action<string> orElse)
        {
            if (Successful) (act ?? throw new ArgumentNullException(nameof(act)))(Capture);
            else (orElse ?? throw new ArgumentNullException(nameof(orElse)))(Message);
        }
        /// <summary>
        /// パースが成功している場合は第1引数で指定されたアクションを実行します。
        /// さもなくば第2引数で指定されたアクションを実行します。
        /// </summary>
        /// <param name="act">成功している場合に実行されるアクション</param>
        /// <param name="orElse">失敗している場合に実行されるアクション</param>
        public void IfSuccessful(Action<CharacterPosition, Capture<T>> act, Action<CharacterPosition, string> orElse)
        {
            if (Successful) (act ?? throw new ArgumentNullException(nameof(act)))(Position, Capture);
            else (orElse ?? throw new ArgumentNullException(nameof(orElse)))(Position, Message);
        }
        /// <summary>
        /// パースが失敗している場合は引数で指定されたアクションを実行します。
        /// </summary>
        /// <param name="act">アクション</param>
        public void IfFailed(Action<string> act)
        {
            if (!Successful) (act ?? throw new ArgumentNullException(nameof(act)))(Message);
        }
        /// <summary>
        /// パースが失敗している場合は引数で指定されたアクションを実行します。
        /// </summary>
        /// <param name="act">アクション</param>
        public void IfFailed(Action<CharacterPosition, string> act)
        {
            if (!Successful) (act ?? throw new ArgumentNullException(nameof(act)))(Position, Message);
        }
        /// <summary>
        /// パース結果に引数で指定された関数を適用します。
        /// <see cref="Parser{T}.Cut"/>によるバックトラック可否設定は引き継がれます。
        /// </summary>
        /// <typeparam name="U">結果の型</typeparam>
        /// <param name="transform">関数</param>
        /// <returns>関数を適用した結果</returns>
        public ParseResult<U> Map<U>(Func<T, U> transform)
        {
            if (Successful)
            {
                return _capture.HasValue ? ParseResult.OfSuccess(Position, transform(_capture.Value), !_cut)
                    : ParseResult.OfSuccess<U>(Position, canBacktrack: !_cut);
            }
            return ParseResult.OfFailure<U>(Position, Message, !_cut);
        }
        /// <summary>
        /// 列挙子を返します。
        /// パースが成功している場合、列挙子は唯一の要素としてパース結果の値を列挙します。
        /// パースが失敗している場合、列挙子はいかなる値も列挙しません。
        /// </summary>
        /// <returns>列挙子</returns>
        public IEnumerator<Capture<T>> GetEnumerator()
        {
            if (Successful)
            {
                return ((IEnumerable<Capture<T>>)new Capture<T>[] { Capture }).GetEnumerator();
            }
            else
            {
                return _empty.GetEnumerator();
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        /// <summary>
        /// このインスタンスの文字列表現を返します。
        /// </summary>
        /// <returns>文字列表現</returns>
        public override string ToString()
        {
            if (Successful)
            {
                return string.Format("ParseResult(Successful = {0}, Positon = {1}, Capture = {2}, CanBacktrack = {3})",
                    true, Position, Capture, !_cut);
            }
            else
            {
                return string.Format("ParseResult(Successful = {0}, Positon = {1}, Message = {2}, CanBacktrack = {3})",
                    false, Position, Message, !_cut);
            }
        }
    }
}
