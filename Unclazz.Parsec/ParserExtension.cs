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
        public static Parser<string> Join(this Parser<Seq<char>> self)
        {
            var seqParser = self as SeqParser<char>;
            if (seqParser == null) return self.Map(a => a.Aggregate(new StringBuilder(), (b,c)=>b.Append(c), b => b.ToString()));
            return seqParser.ReAggregate(() => new StringBuilder(), (a, b) => a.Append(b), a => a.ToString());
        }
        public static Parser<TAccumulate> Aggregate<TAccumulate>(
            this Parser<Seq<TAccumulate>> self, Func<TAccumulate, TAccumulate, TAccumulate> accumulator)
        {
            var seqParser = self as SeqParser<TAccumulate>;
            if (seqParser == null) return self.Map(a => a.Aggregate(accumulator));
            return seqParser.ReAggregate(accumulator);
        }
        /// <summary>
        /// <see cref="Seq{T}"/>型のパース結果に対して集約を行うパーサーを返します。
        /// <para>
        /// <paramref name="accumulator"/>呼び出しの初回は
        /// <paramref name="seedFactory"/>が返す値とリストの1番目の要素が引数にアサインされます。
        /// 2回目は初回の呼び出し結果とリストの2番目の要素が引数にアサインされます。
        /// 3回目以降は前回の呼び出し結果とリストの次の要素が引数にアサインされます。
        /// 最終回の呼び出し結果が新しいパーサーの返す結果の値となります。
        /// </para>
        /// </summary>
        /// <typeparam name="TSource">要素の型</typeparam>
        /// <typeparam name="TAccumulate">集約中/集約後の型</typeparam>
        /// <param name="self">レシーバー</param>
        /// <param name="seedFactory">集約の起点となる値を生成するファクトリー</param>
        /// <param name="accumulator">集約の実処理を担う関数</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<TAccumulate> Aggregate<TSource, TAccumulate>(this Parser<Seq<TSource>> self,
            Func<TAccumulate> seedFactory, Func<TAccumulate, TSource, TAccumulate> accumulator)
        {
            var seqParser = self as SeqParser<TSource>;
            if (seqParser == null) return self.Map(a => a.Aggregate(seedFactory(), accumulator));
            return seqParser.ReAggregate(seedFactory, accumulator);
        }
        /// <summary>
        /// <see cref="Seq{T}"/>型のパース結果に対して集約を行うパーサーを返します。
        /// <para>
        /// <paramref name="accumulator"/>呼び出しの初回は
        /// <paramref name="seedFactory"/>が返す値とリストの1番目の要素が引数にアサインされます。
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
        /// <param name="seedFactory">集約の起点となる値</param>
        /// <param name="accumulator">集約の実処理を担う関数</param>
        /// <param name="resultSelector">集約結果を返す関数</param>
        /// <returns></returns>
        public static Parser<TResult> Aggregate<TSource, TAccumulate, TResult>(this Parser<Seq<TSource>> self,
            Func<TAccumulate> seedFactory, Func<TAccumulate, TSource, TAccumulate> accumulator, Func<TAccumulate, TResult> resultSelector)
        {
            var seqParser = self as SeqParser<TSource>;
            if (seqParser == null) return self.Map(a => a.Aggregate(seedFactory(), accumulator, resultSelector));

            return seqParser.ReAggregate(seedFactory, accumulator, resultSelector);
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
