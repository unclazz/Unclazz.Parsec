using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec
{
    /// <summary>
    /// <see cref="Parser{T}"/>クラスのための拡張メソッドを提供するクラスです。
    /// </summary>
    public static class ParserExtension
    {
        /// <summary>
        /// <see cref="char"/>のシーケンスを読み取るパーサーをもとに
        /// <see cref="string"/>を読み取るパーサーを生成します。
        /// </summary>
        /// <param name="self">レシーバー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<string> Concat(this Parser<IEnumerable<char>> self)
        {
            return self.Map(cs => cs.Aggregate(new StringBuilder(), (b, ch) => b.Append(ch)).ToString());
        }
        /// <summary>
        /// <see cref="string"/>のシーケンスを読み取るパーサーをもとに
        /// <see cref="string"/>を読み取るパーサーを生成します。
        /// </summary>
        /// <param name="self">レシーバー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<string> Concat(this Parser<IEnumerable<string>> self)
        {
            return self.Map(cs => cs.Aggregate(new StringBuilder(), (b, s) => b.Append(s)).ToString());
        }
        public static Parser<string> FlatConcat(this Parser<IEnumerable<IEnumerable<string>>> self)
        {
            return self.Map(sss => sss.SelectMany(ss => ss).Aggregate(new StringBuilder(), (b, s) => b.Append(s)).ToString());
        }
        /// <summary>
        /// 任意の型の値を読み取るパーサーと同じ型のシーケンスを読み取るパーサーをもとに、
        /// 同じ型のシーケンスを読みよるパーサーを生成します。
        /// </summary>
        /// <typeparam name="T">読み取り対象の型</typeparam>
        /// <param name="self">レシーバー</param>
        /// <param name="another">任意の型のシーケンスを読み取るパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<IEnumerable<T>> Then<T>(this Parser<T> self, Parser<IEnumerable<T>> another)
        {
            return new ThenManyParser<T>(self, another);
        }
        /// <summary>
        /// 任意の型のシーケンスを読み取るパーサーと同じ型の単一値を読み取るパーサーをもとに、
        /// 同じ型のシーケンスを読みよるパーサーを生成します。
        /// </summary>
        /// <typeparam name="T">読み取り対象の型</typeparam>
        /// <param name="self">レシーバー</param>
        /// <param name="another">任意の型の単一値を読み取るパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<IEnumerable<T>> Then<T>(this Parser<IEnumerable<T>> self, Parser<T> another)
        {
            return new ManyThenParser<T>(self, another);
        }
        /// <summary>
        /// <see cref="string"/>を読み取るパーサーと<see cref="char"/>を読み取るパーサーをもとに、
        /// <see cref="string"/>を読みよるパーサーを生成します。
        /// </summary>
        /// <param name="self">レシーバー</param>
        /// <param name="another"><see cref="char"/>を読み取るパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<string> Then(this Parser<string> self, Parser<char> another)
        {
            return self.Then(another.Map(char.ToString)).Concat();
        }
        /// <summary>
        /// <see cref="char"/>を読み取るパーサーと<see cref="string"/>を読み取るパーサーをもとに、
        /// <see cref="string"/>を読みよるパーサーを生成します。
        /// </summary>
        /// <param name="self">レシーバー</param>
        /// <param name="another"><see cref="string"/>を読み取るパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<string> Then(this Parser<char> self, Parser<string> another)
        {
            return self.Map(char.ToString).Then(another).Concat();
        }
        public static Parser<IEnumerable<string>> Then(this Parser<IEnumerable<string>> self, Parser<char> another)
        {
            return self.Then(another.Map(char.ToString));
        }
        /// <summary>
        /// 入れ子のシーケンスを読みよるパーサーをもとに、
        /// 平坦化されたシーケンスを読み取るパーサーを生成します。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="self">レシーバー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<IEnumerable<T>> Flatten<T>(this Parser<IEnumerable<IEnumerable<T>>> self)
        {
            return self.Map(outer => outer.SelectMany(inner => inner));
        }
        /// <summary>
        /// 入れ子のシーケンスを読みよるパーサーと任意の型から別の任意の型への変換を行う関数をもとに、
        /// 平坦化されたシーケンスを読み取るパーサーを生成します。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <typeparam name="U">別の任意の型</typeparam>
        /// <param name="self">レシーバー</param>
        /// <param name="transform">変換のための関数</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<IEnumerable<U>> FlatMap<T, U>(this Parser<IEnumerable<IEnumerable<T>>> self, Func<T, U> transform)
        {
            return self.Map(outer => outer.SelectMany(inner => inner).Select(transform));
        }
        /// <summary>
        /// <see cref="string"/>を読み取るパーサーをもとに、
        /// 読み取り結果をキャプチャするパーサーを生成します。
        /// </summary>
        /// <param name="self">レシーバー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<string> Capture(this Parser<string> self)
        {
            return new CaptureParser(self);
        }
    }
}
