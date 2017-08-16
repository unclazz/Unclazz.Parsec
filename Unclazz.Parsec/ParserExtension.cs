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
        /// <see cref="string"/>を読み取るパーサーをもとに、
        /// 読み取り結果をキャプチャするパーサーを生成します。
        /// </summary>
        /// <param name="self">レシーバー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<string> Capture<T>(this Parser<T> self)
        {
            return new CaptureParser<T>(self);
        }
        /// <summary>
        /// <see cref="string"/>を読み取るパーサーをもとに、
        /// 読み取り結果をキャプチャするパーサーを生成します。
        /// </summary>
        /// <param name="self">レシーバー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<string> Capture(this Parser<char> self)
        {
            return self.Map(a => a);
        }
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
            Func<IEnumerable<string>, string> f = cs =>
            cs.Aggregate(new StringBuilder(), (b, s) => b.Append(s)).ToString();
            return self.Map(f);
        }
        /// <summary>
        /// 任意の型を読み取るパーサーをもとに、
        /// その読み取り結果を任意の関数で変換して返すパーサーを生成します。
        /// <para>
        /// このメソッドが生成して返すパーサーは、その目的ゆえにパース成功時に値を返します。
        /// パース成功時（元になるパーサーがパースに成功した時）はキャプチャした値を引数にして<paramref name="transform"/>を呼び出します。
        /// パース失敗時（元になるパーサーがパースに失敗した時）は<paramref name="transform"/>は呼び出されません。
        /// </para>
        /// </summary>
        /// <typeparam name="T">元のパーサーの読み取り結果の型</typeparam>
        /// <typeparam name="U">読み取り結果を変換した後の型</typeparam>
        /// <param name="self">レシーバー</param>
        /// <param name="transform">変換を行う関数</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<U> Map<T, U>(this Parser<T> self, Func<string, U> transform)
        {
            return new MapParser<T, U>(self, transform);
        }
        /// <summary>
        /// <see cref="IEnumerable{T}"/>を読み取るパーサーをもとに、
        /// その読み取り結果を任意の関数で変換して返すパーサーを生成します。
        /// <para>
        /// このメソッドが生成して返すパーサーは、その目的ゆえにパース成功時に値を返します。
        /// パース成功時（元になるパーサーがパースに成功した時）はキャプチャした値を引数にして<paramref name="transform"/>を呼び出します。
        /// パース失敗時（元になるパーサーがパースに失敗した時）は<paramref name="transform"/>は呼び出されません。
        /// </para>
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <typeparam name="U">別の任意の型</typeparam>
        /// <param name="self">レシーバー</param>
        /// <param name="transform">変換を行う関数</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<U> Map<T,U>(this Parser<IEnumerable<T>> self, Func<IEnumerable<T>, U> transform)
        {
            return new EnumerableMapParser<T, U>(self, transform);
        }
        /// <summary>
        /// 入れ子のシーケンスを読みよるパーサーをもとに、
        /// 平坦化されたシーケンスを読み取るパーサーを生成します。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="self">レシーバー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<IEnumerable<string>> Flatten<T>(this Parser<IEnumerable<IEnumerable<string>>> self)
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
        public static Parser<IEnumerable<T>> FlatMap<T>(this Parser<IEnumerable<IEnumerable<string>>> self, Func<string, T> transform)
        {
            return self.Map(outer => outer.SelectMany(inner => inner).Select(transform));
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
            return self.Then(another.Map(a => a)).Concat();
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
            return self.Map(a => a).Then(another).Concat();
        }
        /// <summary>
        /// <see cref="char"/>を読み取るパーサーと<see cref="string"/>のシーケンスを読み取るパーサーをもとに、
        /// <see cref="string"/>のシーケンスを読みよるパーサーを生成します。
        /// </summary>
        /// <param name="self">レシーバー</param>
        /// <param name="another"><see cref="string"/>のシーケンスを読み取るパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<IEnumerable<string>> Then(this Parser<char> self, Parser<IEnumerable<string>> another)
        {
            Func<IEnumerable<string>, string> f = cs =>
            cs.Aggregate(new StringBuilder(), (b, s) => b.Append(s)).ToString();
            return self.Map(a => a).Then(another.Map(f));
        }
        /// <summary>
        /// <see cref="string"/>のシーケンスを読み取るパーサーと<see cref="char"/>を読み取るパーサーをもとに、
        /// <see cref="string"/>のシーケンスを読みよるパーサーを生成します。
        /// </summary>
        /// <param name="self">レシーバー</param>
        /// <param name="another"><see cref="string"/>のシーケンスを読み取るパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<IEnumerable<string>> Then(this Parser<IEnumerable<string>> self, Parser<char> another)
        {
            return self.Then(another.Map(a => a));
        }
    }
}
