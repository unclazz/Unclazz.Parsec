using System;
using System.Collections.Generic;
using System.Text;
using Unclazz.Parsec.CoreParsers.RepeatAggregate;

namespace Unclazz.Parsec
{
    /// <summary>
    /// パターンの繰り返しを読み取るパーサーです。
    /// </summary>
    /// <typeparam name="T">読み取り結果のシーケンスの要素型</typeparam>
    public sealed class RepeatParser<T> : Parser<Seq<T>>
    {
        internal RepeatParser(Parser<T> original, int min, int max, int exactly, Parser sep)
        {
            _inner = new SeqParser<T>(original, new RepeatConfiguration(min, max, exactly, sep));
        }
        readonly SeqParser<T> _inner;
        /// <summary>
        /// パースを行います。
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        protected override ResultCore<Seq<T>> DoParse(Reader input)
        {
            return _inner.Parse(input);
        }
        /// <summary>
        /// シーケンス要素の集約を行います。
        /// <para>
        /// アキュームレーター関数ははじめにシーケンスの<c>0</c>番目の要素と<c>1</c>番目の要素に適用されます。
        /// その後、この適用結果と<c>2</c>番目の要素に適用されます。以降、順繰りに適用が繰り返され、
        /// 最後の適用結果がパーサーの読み取り結果型となります。
        /// </para>
        /// </summary>
        /// <param name="accumulator"></param>
        /// <returns></returns>
        public Parser<T> Aggregate(Func<T, T, T> accumulator)
        {
            return _inner.ReAggregate(accumulator);
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
        /// <typeparam name="TAccumulate"></typeparam>
        /// <param name="seedFactory"></param>
        /// <param name="accumulator"></param>
        /// <returns></returns>
        public Parser<TAccumulate> Aggregate<TAccumulate>(Func<TAccumulate> 
            seedFactory, Func<TAccumulate, T, TAccumulate> accumulator)
        {
            return _inner.ReAggregate(seedFactory, accumulator);
        }
        /// <summary>
        /// シーケンス要素の集約を行います。
        /// <para>
        /// はじめにアキュームレーター関数がこの<paramref name="seed"/>とシーケンスの<c>0</c>番目の要素に適用されます。
        /// その後、この適用結果と<c>1</c>番目の要素に適用されます。以降、順繰りに適用が繰り返され、
        /// 最後の適用結果がパーサーの読み取り結果型となります。
        /// </para>
        /// </summary>
        /// <typeparam name="TAccumulate"></typeparam>
        /// <param name="seed"></param>
        /// <param name="accumulator"></param>
        /// <returns></returns>
        public Parser<TAccumulate> Aggregate<TAccumulate>(TAccumulate seed, 
            Func<TAccumulate, T, TAccumulate> accumulator) where TAccumulate : struct
        {
            return _inner.ReAggregate(() => seed, accumulator);
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
        /// <typeparam name="TAccumulate"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="seedFactory"></param>
        /// <param name="accumulator"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public Parser<TResult> Aggregate<TAccumulate, TResult>(Func<TAccumulate> seedFactory,
            Func<TAccumulate, T, TAccumulate> accumulator, Func<TAccumulate, TResult> resultSelector)
        {
            return _inner.ReAggregate(seedFactory, accumulator, resultSelector);
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
        /// <typeparam name="TAccumulate"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="seed"></param>
        /// <param name="accumulator"></param>
        /// <param name="resultSelector"></param>
        /// <returns></returns>
        public Parser<TResult> Aggregate<TAccumulate, TResult>(TAccumulate seed,
            Func<TAccumulate, T, TAccumulate> accumulator, Func<TAccumulate, TResult> 
            resultSelector) where TAccumulate : struct
        {
            return _inner.ReAggregate(() => seed, accumulator, resultSelector);
        }
        /// <summary>
        /// シーケンス要素のカウントを行います。
        /// </summary>
        /// <returns></returns>
        public Parser<int> Count()
        {
            return _inner.ReAggregate(() => 0, (a, b) => a + 1);
        }
        /// <summary>
        /// シーケンス要素を文字列として連結します。
        /// </summary>
        /// <returns></returns>
        public Parser<string> Join()
        {
            return _inner.ReAggregate(Join_SeedFactory, Join_Accumulator, Join_ResultSelector);
        }
        /// <summary>
        /// シーケンス要素を文字列として連結します。
        /// </summary>
        /// <param name="sep"></param>
        /// <returns></returns>
        public Parser<string> Join(object sep)
        {
            return _inner.ReAggregate(Join_SeedFactory, Join_Accumulator(sep), Join_ResultSelector);
        }
        StringBuilder Join_SeedFactory() => new StringBuilder();
        StringBuilder Join_Accumulator(StringBuilder a, T b) => a.Append(b);
        Func<StringBuilder, T, StringBuilder> Join_Accumulator(object sep)
        {
            var first = true;
            return (a, b) =>
            {
               if (first)
                {
                    first = false;
                    return a.Append(b);
                }
                return a.Append(sep).Append(b);
            };
        }
        string Join_ResultSelector(StringBuilder a) => a.ToString();
    }
}
