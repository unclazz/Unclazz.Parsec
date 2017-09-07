using System;
using System.Linq;
using System.Text;
using Unclazz.Parsec.CoreParsers;

namespace Unclazz.Parsec
{
    /// <summary>
    /// <see cref="Parser{T}"/>のための拡張メソッドを提供する静的クラスです。
    /// </summary>
    public static class ParserExtension
    {
        /// <summary>
        /// <see cref="Seq{T}"/>型のパース結果に対して集約を行うパーサーを返します。
        /// <para>
        /// <paramref name="func"/>呼び出しの初回はリストの1番目の要素と2番目の要素が引数にアサインされます。
        /// 2回目は初回の呼び出し結果とリストの3番目の要素が引数にアサインされます。
        /// 3回目以降は前回の呼び出し結果とリストの次の要素が引数にアサインされます。
        /// 最終回の呼び出し結果が新しいパーサーの返す結果の値となります。
        /// </para>
        /// </summary>
        /// <typeparam name="TSource">要素の型</typeparam>
        /// <param name="self">レシーバー</param>
        /// <param name="func">集約の実処理を担う関数</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<TSource> Aggregate<TSource>(this Parser<Seq<TSource>> self, Func<TSource, TSource, TSource> func)
        {
            return self.Map(ls => ls.Aggregate(func));
        }
        /// <summary>
        /// <see cref="Seq{T}"/>型のパース結果に対して集約を行うパーサーを返します。
        /// <para>
        /// <paramref name="func"/>呼び出しの初回は<paramref name="seed"/>とリストの1番目の要素が引数にアサインされます。
        /// 2回目は初回の呼び出し結果とリストの2番目の要素が引数にアサインされます。
        /// 3回目以降は前回の呼び出し結果とリストの次の要素が引数にアサインされます。
        /// 最終回の呼び出し結果が新しいパーサーの返す結果の値となります。
        /// </para>
        /// </summary>
        /// <typeparam name="TSource">要素の型</typeparam>
        /// <typeparam name="TAccumulate">集約中/集約後の型</typeparam>
        /// <param name="self">レシーバー</param>
        /// <param name="seed">集約の起点となる値</param>
        /// <param name="func">集約の実処理を担う関数</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<TAccumulate> Aggregate<TSource, TAccumulate>
            (this Parser<Seq<TSource>> self, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            return self.Map(ls => ls.Aggregate(seed, func));
        }
        /// <summary>
        /// <see cref="Seq{T}"/>型のパース結果に対して集約を行うパーサーを返します。
        /// <para>
        /// <paramref name="func"/>呼び出しの初回は<paramref name="seed"/>とリストの1番目の要素が引数にアサインされます。
        /// 2回目は初回の呼び出し結果とリストの2番目の要素が引数にアサインされます。
        /// 3回目以降は前回の呼び出し結果とリストの次の要素が引数にアサインされます。
        /// 最終回の呼び出し結果は<paramref name="resultSelector"/>の引数にアサインされます。
        /// <paramref name="resultSelector"/>の呼び出し結果がこの新しいパーサーの返す結果の値となります。
        /// </para>
        /// </summary>
        /// <typeparam name="TSource">要素の型</typeparam>
        /// <typeparam name="TAccumulate">集約中の型</typeparam>
        /// <typeparam name="TResult">集約後の型</typeparam>
        /// <param name="self">レシーバー</param>
        /// <param name="seed">集約の起点となる値</param>
        /// <param name="func">集約の実処理を担う関数</param>
        /// <param name="resultSelector">集約結果を返す関数</param>
        /// <returns></returns>
        public static Parser<TResult> Aggregate<TSource, TAccumulate, TResult>
            (this Parser<Seq<TSource>> self, TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
        {
            return self.Map(ls => ls.Aggregate(seed, func, resultSelector));
        }
        /// <summary>
        /// <see cref="Seq{T}"/>型のパース結果に対してその各要素を連結して文字列にするパーサーを返します。
        /// </summary>
        /// <typeparam name="TSource">要素の型</typeparam>
        /// <param name="self">レシーバー</param>
        /// <returns></returns>
        public static Parser<string> Join<TSource>(this Parser<Seq<TSource>> self)
        {
            return self.Map(ls => ls.Aggregate(new StringBuilder(), (a, b) => a.Append(b)).ToString());
        }
        /// <summary>
        /// <see cref="Seq{T}"/>型のパース結果に対してその各要素を連結して文字列にするパーサーを返します。
        /// </summary>
        /// <typeparam name="TSource">要素の型</typeparam>
        /// <param name="self">レシーバー</param>
        /// <param name="sep">セパレーター</param>
        /// <returns></returns>
        public static Parser<string> Join<TSource>(this Parser<Seq<TSource>> self, object sep)
        {
            return self.Map(ls => ls.Aggregate(new StringBuilder(), (a, b) => a.Append(sep).Append(b), a => a.Remove(0, 1)).ToString());
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
        /// <param name="self">レシーバー</param>
        /// <param name="another">別のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<Tuple<T1, T2, T3>> Then<T1, T2, T3>(this Parser<Tuple<T1, T2>> self, Parser<T3> another)
        {
            return TripleParser<T1, T2, T3>.Create(self, another);
        }
    }
}
