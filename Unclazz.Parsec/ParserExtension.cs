using System;
using System.Collections.Generic;
using System.Linq;
using Unclazz.Parsec.CoreParsers;

namespace Unclazz.Parsec
{
    /// <summary>
    /// パーサー・クラス<see cref="Parser"/>および<see cref="Parser{T}"/>のための拡張メソッドを提供する静的クラスです。
    /// <para>このクラスで提供されるメソッドは、双方のクラスに共通する名前を持ちながらも少しずつシグネチャや機能の異なるものです。</para>
    /// </summary>
    public static class ParserExtension
    {
        public static Parser<TSource> Aggregate<TSource>(this Parser<IList<TSource>> self, Func<TSource, TSource, TSource> func)
        {
            return self.Map(ls => ls.Aggregate(func));
        }
        public static Parser<TAccumulate> Aggregate<TSource, TAccumulate>
            (this Parser<IList<TSource>> self, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            return self.Map(ls => ls.Aggregate(seed, func));
        }
        public static Parser<TResult> Aggregate<TSource, TAccumulate, TResult>
            (this Parser<IList<TSource>> self, TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
        {
            return self.Map(ls => ls.Aggregate(seed, func, resultSelector));
        }
        /// <summary>
        /// <see cref="Parser{T}.Cast{U}"/>と同義です。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> Cast<T>(this Parser self)
        {
            return new CastParser<Nil, T>(self);
        }
        /// <summary>
        /// <see cref="Parser{T}.Cast{U}"/>と同義です。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> Cast<T>(this Parser self, T defaultValue)
        {
            return new CastParser<Nil, T>(self, defaultValue);
        }
        /// <summary>
        /// <see cref="Cast{U}"/>と同義です。
        /// </summary>
        /// <returns>新しいパーサー</returns>
        public static Parser Cast<T>(this Parser<T> self)
        {
            return new CastParser<T>(self);
        }
        /// <summary>
        /// <see cref="Parser{T}.Cut"/>と同義です。
        /// </summary>
        /// <returns>新しいパーサー</returns>
        public static Parser Cut(this Parser self)
        {
            return new CutParser<Nil>(self).Cast();
        }
        /// <summary>
        /// 直近の<see cref="Parser{T}.Or(Parser{T})"/>を起点としたバックトラックを無効化します。
        /// <para>
        /// このパーサーが成功したあと後続のパーサーが失敗した場合バックトラックは機能せず、
        /// <see cref="Parser{T}.Or(Parser{T})"/>で連結された他のパーサーの実行が試行されることはありません。
        /// もちろんこのメソッドを呼び出す以前のパーサーが失敗した場合は引き続きバックトラックが有効です。
        /// </para>
        /// </summary>
        /// <returns>バックトラック機能が無効化された新しいパーサー</returns>
        public static Parser<T> Cut<T>(this Parser<T> self)
        {
            return new CutParser<T>(self);
        }
        /// <summary>
        /// デバッグたのめパース処理前後の情報をログ出力するパーサーを返します。
        /// </summary>
        /// <param name="logger">ログ出力そのものを行うアクション</param>
        /// <returns>新しいパーサー</returns>
        public static Parser Log(this Parser self, Action<string> logger)
        {
            return new LogParser<Nil>(self, logger).Cast();
        }
        /// <summary>
        /// デバッグたのめパース処理前後の情報をログ出力するパーサーを返します。
        /// </summary>
        /// <param name="logger">ログ出力そのものを行うアクション</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> Log<T>(this Parser<T> self, Action<string> logger)
        {
            return new LogParser<T>(self, logger);
        }
        /// <summary>
        /// <see cref="Parser{T}.Or(Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="another">別のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser Or(this Parser self, Parser another)
        {
            return OrParser<Nil>.LeftAssoc(self, another).Cast();
        }
        /// <summary>
        /// <see cref="Parser{T}.Or(Parser{T}, Parser{T}[])"/>と同義です。
        /// </summary>
        /// <param name="another">別のパーサー</param>
        /// <param name="andOthers">その他のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser Or(this Parser self, Parser another, params Parser[] andOthers)
        {
            return OrParser<Nil>.LeftAssoc(self, another, andOthers).Cast();
        }
        /// <summary>
        /// <see cref="Parser{T}.OrNot"/>と同義です。
        /// </summary>
        /// <returns>新しいパーサー</returns>
        public static Parser OrNot(this Parser self)
        {
            return new OrNotParser<Nil>(self).Cast();
        }
        /// <summary>
        /// このパーサーの読み取りが失敗したときに実行されるパーサーを指定します。
        /// <para>
        /// このパーサー（レシーバーとなるパーサー）の読み取りが成功した場合は、
        /// その結果がそのまま新しいパーサーの返す結果となります。
        /// 一方、このパーサーの読み取りが失敗した場合は、データソースの読み取り位置はリセットされ（バックトラック）、
        /// 引数で指定されたもう1つのパーサーの読み取りが試行され、その結果が新しいパーサーの返す結果となります。
        /// </para>
        /// <para>演算子<c>|</c>とインスタンス・メソッド<see cref="Parser{T}.Or(Parser{T})"/>のグループと
        /// 静的メソッド<see cref="Parser.Or{T}(Parser{T}, Parser{T})"/>はいずれも右結合です。
        /// つまり<c>p0 | p1 | p2</c>や<c>p0.Or(p1).Or(p2)</c>というコードは、概念的には<c>(p0 | p1) | p2</c>と解釈されます。
        /// もし仮に<c>p0</c>構築中のいずれかの地点で<see cref="Cut"/>が実行されており当該地点以降でトラックバックが無効化されている場合、
        /// これ以降の区間でパースが失敗すると当然<c>p1</c>は実行されないとしても、<c>p2</c>は引き続き実行されるということです。
        /// あえてこの挙動を変えるには<c>p0 | (p1 | p2)</c>や<c>p0.Or(p1.Or(p2))</c>というコードに変更する必要があります。
        /// </para>
        /// </summary>
        /// <param name="another">別のパーサー</param>
        /// <returns>バックトラック機能をサポートする新しいパーサー</returns>
        public static Parser<T> Or<T>(this Parser<T> self, Parser<T> another)
        {
            return OrParser<T>.LeftAssoc(self, another);
        }
        /// <summary>
        /// <see cref="Parser{T}.Or(Parser{T})"/>と同義ですが、
        /// 複数のパーサーを一括指定することができます。
        /// </summary>
        /// <param name="another">別のパーサー</param>
        /// <param name="andOthers">その他のパーサー</param>
        /// <returns>バックトラック機能をサポートする新しいパーサー</returns>
        public static Parser<T> Or<T>(this Parser<T> self, Parser<T> another, params Parser<T>[] andOthers)
        {
            return OrParser<T>.LeftAssoc(self, another, andOthers);
        }
        /// <summary>
        /// このパーサーのパースの結果成否にかかわらずパース成功とみなす新しいパーサーを返します。
        /// </summary>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> OrNot<T>(this Parser<T> self)
        {
            return new OrNotParser<T>(self);
        }
        /// <summary>
        /// <see cref="Parser{T}.Repeat(int, int, int, Parser)"/>と同義です。
        /// </summary>
        /// <param name="min">繰り返しの最小回数</param>
        /// <param name="max">繰り返しの最大回数</param>
        /// <param name="exactly">繰り返しの回数</param>
        /// <param name="sep">セパレーターのためのパーサー</param>
        /// <returns>繰り返しをサポートする新しいパーサー</returns>
        public static Parser Repeat(this Parser self, int min = 0, int max = -1, int exactly = -1, Parser sep = null)
        {
            return RepeatParser<Nil>.Create(self, min, max, exactly, sep).Cast();
        }
        /// <summary>
        /// シーケンスを読み取るパーサーを生成します。
        /// <para>
        /// 4つの引数はいずれもオプションです。
        /// 何も指定しなかった場合、0回以上で上限なしの繰り返しを表します。
        /// <paramref name="exactly"/>を指定した場合はまさにその回数の繰り返しです。
        /// <paramref name="min"/>および/もしくは<paramref name="max"/>を指定した場合は下限および/もしくは上限付きの繰り返しです。
        /// </para>
        /// </summary>
        /// <param name="min">繰り返しの最小回数</param>
        /// <param name="max">繰り返しの最大回数</param>
        /// <param name="exactly">繰り返しの回数</param>
        /// <param name="sep">セパレーターのためのパーサー</param>
        /// <returns>繰り返しをサポートする新しいパーサー</returns>
        public static Parser<IList<T>> Repeat<T>(this Parser<T> self, int min = 0, int max = -1, int exactly = -1, Parser sep = null)
        {
            return RepeatParser<T>.Create(self, min, max, exactly, sep);
        }
        /// <summary>
        /// <see cref="Parser{T}.Then{U}(Parser{U})"/>と同義です。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="another">別のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> Then<T>(this Parser self, Parser<T> another)
        {
            return new ThenTakeRightParser<Nil, T>(self, another);
        }
        /// <summary>
        /// <see cref="Parser{T}.Then{U}(Parser{U})"/>と同義です。
        /// </summary>
        /// <param name="another">別のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser Then(this Parser self, Parser another)
        {
            return new ThenTakeRightParser<Nil, Nil>(self, another).Cast();
        }
        /// <summary>
        /// このパーサーのパースが成功したあと引数で指定した別のパーサーのパースを行う新しいパーサーを返します。
        /// <para>
        /// 例えば<c>var p2 = p0.Then(p1); p2.Parse(...);</c>というコードがあったとき、
        /// p0のパースが成功した場合は、引き続きp1のパースが実行されます。
        /// p1が成功した場合はp2の結果も成功となります。p1が失敗した場合はp2の結果も失敗です。
        /// p0が失敗した場合はp1は実行されず、p2の結果は失敗となります。
        /// </para>
        /// <para>
        /// 元になるパーサー（p0やp1）が値をキャプチャしないパーサーであれば
        /// 新しいパーサー（p2）もまた値をキャプチャしないパーサーとなります。
        /// 元になるパーサーが値をキャプチャするパーサーであれば
        /// 新しいパーサーもまた値をキャプチャするパーサーとなります。
        /// </para>
        /// </summary>
        /// <param name="another">別のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<Tuple<T, U>> Then<T, U>(this Parser<T> self, Parser<U> another)
        {
            return new DoubleParser<T, U>(self, another);
        }
        /// <summary>
        /// <see cref="Parser{T}.Then{U}(Parser{U})"/>と同義です。
        /// </summary>
        /// <param name="another">別のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> Then<T>(this Parser<T> self, Parser another)
        {
            return new ThenTakeLeftParser<T, Nil>(self, another);
        }
        public static Parser<Tuple<T1, T2, T3>> Then<T1, T2, T3>(this Parser<T1> self, Parser<Tuple<T2, T3>> another)
        {
            return new AndDoubleParser<T1, T2, T3>(self, another);
        }
        public static Parser<Tuple<T1, T2, T3>> Then<T1, T2, T3>(this Parser<Tuple<T1, T2>> self, Parser<T3> another)
        {
            return new DoubleAndParser<T1, T2, T3>(self, another);
        }
    }
}
