using System;
using System.Linq;
using System.Text;
using Unclazz.Parsec.CoreParsers;
using Unclazz.Parsec.CoreParsers.RepeatAggregate;

namespace Unclazz.Parsec
{
    /// <summary>
    /// <see cref="Parser{T}"/>のための拡張メソッドを提供する静的クラスです。
    /// </summary>
    public static class ParserExtension
    {
        /// <summary>
        /// シーケンス要素の集約を行います。
        /// <para>
        /// アキュームレーター関数ははじめにシーケンスの<c>0</c>番目の要素と<c>1</c>番目の要素に適用されます。
        /// その後、この適用結果と<c>2</c>番目の要素に適用されます。以降、順繰りに適用が繰り返され、
        /// 最後の適用結果がパーサーの読み取り結果型となります。
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="accumulator"></param>
        /// <returns></returns>
        public static Parser<T> Reduce<T>(this Parser<Seq<T>> self, Func<T, T, T> accumulator)
        {
            return self.Map(a => a.Aggregate(accumulator));
        }
        /// <summary>
        /// シーケンス要素の集約を行います。
        /// <para>
        /// はじめに<paramref name="seedFactory"/>により集約のシードが生成されます。
        /// アキュームレーター関数がこのシードとシーケンスの<c>0</c>番目の要素に適用されます。
        /// その後、この適用結果と<c>1</c>番目の要素に適用されます。以降、順繰りに適用が繰り返され、
        /// 最後の適用結果がパーサーの読み取り結果型となります。
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="self"></param>
        /// <param name="seedFactory"></param>
        /// <param name="accumulator"></param>
        /// <returns></returns>
        public static Parser<U> Reduce<T, U>(this Parser<Seq<T>> self,
            Func<U> seedFactory, Func<U, T, U> accumulator)
        {
            return self.Map(a => a.Aggregate(seedFactory(), accumulator));
        }
        /// <summary>
        /// シーケンス要素の集約を行います。
        /// <para>
        /// はじめにアキュームレーター関数がこの<paramref name="seed"/>とシーケンスの<c>0</c>番目の要素に適用されます。
        /// その後、この適用結果と<c>1</c>番目の要素に適用されます。以降、順繰りに適用が繰り返され、
        /// 最後の適用結果がパーサーの読み取り結果型となります。
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="self"></param>
        /// <param name="seed"></param>
        /// <param name="accumulator"></param>
        /// <returns></returns>
        public static Parser<U> Reduce<T, U>(this Parser<Seq<T>> self,
            U seed, Func<U, T, U> accumulator) where U : struct
        {
            return self.Map(a => a.Aggregate(seed, accumulator));
        }
        /// <summary>
        /// シーケンス要素の集約を行います。
        /// <para>
        /// はじめに<paramref name="seedFactory"/>により集約のシードが生成されます。
        /// アキュームレーター関数がこのシードとシーケンスの<c>0</c>番目の要素に適用されます。
        /// その後、この適用結果と<c>1</c>番目の要素に適用されます。以降、順繰りに適用が繰り返されます。
        /// 最後の適用結果に<paramref name="resultSelector"/>が適用され、
        /// この結果がパーサーの読み取り結果型となります。
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="self"></param>
        /// <param name="seedFactory"></param>
        /// <param name="accumulator"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static Parser<V> Reduce<T, U, V>(this Parser<Seq<T>> self, 
            Func<U> seedFactory, Func<U, T, U> accumulator, Func<U, V> resultSelector)
        {
            return self.Map(a => a.Aggregate(seedFactory(), accumulator, resultSelector));
        }
        /// <summary>
        /// シーケンス要素の集約を行います。
        /// <para>
        /// はじめにアキュームレーター関数がこの<paramref name="seed"/>とシーケンスの<c>0</c>番目の要素に適用されます。
        /// その後、この適用結果と<c>1</c>番目の要素に適用されます。以降、順繰りに適用が繰り返されます。
        /// 最後の適用結果に<paramref name="resultSelector"/>が適用され、
        /// この結果がパーサーの読み取り結果型となります。
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="self"></param>
        /// <param name="seed"></param>
        /// <param name="accumulator"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public static Parser<V> Reduce<T, U, V>(this Parser<Seq<T>> self, U seed,
            Func<U, T, U> accumulator, Func<U, V> resultSelector) where U : struct
        {
            return self.Map(a => a.Aggregate(seed, accumulator, resultSelector));
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
