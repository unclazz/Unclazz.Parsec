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
