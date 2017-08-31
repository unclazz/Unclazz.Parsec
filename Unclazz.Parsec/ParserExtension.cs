using System;
using System.Collections.Generic;
using System.Linq;
using Unclazz.Parsec.CoreParsers;

namespace Unclazz.Parsec
{
    /// <summary>
    /// パーサー・クラス<see cref="Parser"/>および<see cref="Parser{T}"/>のための拡張メソッドを提供する静的クラスです。
    /// <para>このクラスで提供されるメソッドの多くは、双方のクラスに共通する名前を持ちながらも少しずつシグネチャや機能の異なるものです。</para>
    /// </summary>
    public static class ParserExtension
    {
        #region Aggregate系の拡張メソッド
        /// <summary>
        /// <see cref="IList{T}"/>型のパース結果に対して集約を行うパーサーを返します。
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
        public static Parser<TSource> Aggregate<TSource>(this Parser<IList<TSource>> self, Func<TSource, TSource, TSource> func)
        {
            return self.Map(ls => ls.Aggregate(func));
        }
        /// <summary>
        /// <see cref="IList{T}"/>型のパース結果に対して集約を行うパーサーを返します。
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
            (this Parser<IList<TSource>> self, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            return self.Map(ls => ls.Aggregate(seed, func));
        }
        /// <summary>
        /// <see cref="IList{T}"/>型のパース結果に対して集約を行うパーサーを返します。
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
            (this Parser<IList<TSource>> self, TAccumulate seed,
            Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
        {
            return self.Map(ls => ls.Aggregate(seed, func, resultSelector));
        }
        #endregion

        #region Cast系の拡張メソッド
        /// <summary>
        /// このパーサーの読み取り結果をキャプチャするパーサーを生成します。
        /// <para>
        /// パース処理そのものはこのパーサー（レシーバー）に委譲されます。
        /// ただしこのパーサーが本来返す値の型がなんであれ、パース開始から終了（パース成功）までの区間のデータはあくまでも
        /// <see cref="string"/>としてキャプチャされ、それがラッパーとなる新しいパーサーが返す値となります。</para>
        /// <para>
        /// 内部的な動作はおおよそ次のように進みます。
        /// パース処理本体が実行される前に<see cref="Reader.Mark"/>が呼び出されます。
        /// パース処理本体が成功した場合は<see cref="Reader.Capture(bool)"/>が呼び出されます。
        /// パース処理本体が失敗した場合は単に<see cref="Reader.Unmark"/>が呼び出されます。</para>
        /// </summary>
        /// <returns>キャプチャ機能をサポートする新しいパーサー</returns>
        public static Parser<string> Capture(this Parser self)
        {
            return new CaptureParser(self.Configuration, self);
        }
        /// <summary>
        /// <see cref="Parser"/>を<see cref="Parser{T}"/>に変換します。
        /// <para>
        /// 変換後の新しいパーサーは返す値の型の情報こそ持っていますが、値のキャプチャは行いません。
        /// </para>
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> Cast<T>(this Parser self, T defaultValue)
        {
            return new AttacheParser<T>(self.Configuration, self, defaultValue);
        }
        /// <summary>
        /// <see cref="Parser{T}"/>を<see cref="Parser"/>に変換します。
        /// <para>
        /// 元のパーサーが値をキャプチャするものであっても変換後のパーサーはあくまでも値をキャプチャしないパーサーとなります。
        /// 元のパーサーがキャプチャした値は破棄されます。
        /// </para>
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="self">レシーバー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser Cast<T>(this Parser<T> self)
        {
            var p = self as Parser;
            return p == null ? new CastParser<T>(self.Configuration, self) : p;
        }
        #endregion

        #region Cut系の拡張メソッド
        /// <summary>
        /// 直近の<c>|</c>や<c>Or(...)</c>を起点としたバックトラックを無効化します。
        /// <para>
        /// レシーバーのパーサーが成功したあと後続のパーサーが失敗した場合バックトラックは機能せず、
        /// <c>|</c>や<c>Or(...)</c>で連結された他のパーサーの実行が試行されることはありません。
        /// このメソッドを呼び出す以前のパーサーが失敗した場合は引き続きバックトラックが有効です。
        /// </para>
        /// </summary>
        /// <returns>新しいパーサー</returns>
        public static Parser Cut(this Parser self)
        {
            return new CutParser(self.Configuration, self);
        }
        /// <summary>
        /// 直近の<c>|</c>や<c>Or(...)</c>を起点としたバックトラックを無効化します。
        /// <para>
        /// レシーバーのパーサーが成功したあと後続のパーサーが失敗した場合バックトラックは機能せず、
        /// <c>|</c>や<c>Or(...)</c>で連結された他のパーサーの実行が試行されることはありません。
        /// このメソッドを呼び出す以前のパーサーが失敗した場合は引き続きバックトラックが有効です。
        /// </para>
        /// </summary>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> Cut<T>(this Parser<T> self)
        {
            return new CutParser<T>(self.Configuration, self);
        }
        #endregion

        #region Map系の拡張メソッド
        /// <summary>
        /// 読み取り結果の<see cref="Optional{T}"/>が内包する値に関数を提供するパーサーを生成します。
        /// <para>
        /// このメソッドが返すパーサーは関数<paramref name="transform"/>が例外をスローした場合、
        /// そのメッセージを使用してパース失敗を表す<see cref="ParseResult{T}"/>インスタンスを返します。
        /// この挙動を変更し、関数がスローした例外をそのまま再スローさせたい場合は
        /// <paramref name="canThrow"/>に<c>true</c>を指定します。
        /// </para>
        /// </summary>
        /// <typeparam name="TSource">元のパーサーの読み取り結果の型</typeparam>
        /// <typeparam name="TResult">読み取り結果を変換した後の型</typeparam>
        /// <param name="self">レシーバー</param>
        /// <param name="transform">変換を行う関数</param>
        /// <param name="canThrow"><paramref name="transform"/>がスローした例外をそのまま再スローさせる場合<c>true</c></param>
        /// <returns>新しいパーサー</returns>
        public static Parser<TResult> Map<TSource, TResult>(this Parser<TSource> self, Func<TSource, TResult> transform, bool canThrow = false)
        {
            return new MapParser<TSource, TResult>(self.Configuration, self, transform, canThrow);
        }
        /// <summary>
        /// 読み取り結果の<see cref="Optional{T}"/>が内包する値を元に動的にパーサーを構築するパーサーを返します。
        /// </summary>
        /// <typeparam name="TSource">元のパーサーの読み取り結果の型</typeparam>
        /// <typeparam name="TResult">読み取り結果を変換した後の型</typeparam>
        /// <param name="self">レシーバー</param>
        /// <param name="mapper">元のパーサーの読み取り結果から動的にパーサーを生成する関数</param>
        /// <param name="canThrow"><paramref name="mapper"/>がスローした例外をそのまま再スローさせる場合<c>true</c></param>
        /// <returns>新しいパーサー</returns>
        public static Parser<TResult> FlatMap<TSource, TResult>(this Parser<TSource> self, Func<TSource, Parser<TResult>> mapper, bool canThrow = false)
        {
            return new FlatMapParser<TSource, TResult>(self.Configuration, self, mapper, canThrow);
        }
        public static Parser FlatMap<TSource>(this Parser<TSource> self, Func<TSource, Parser> mapper, bool canThrow = false)
        {
            return new FlatMapParser<TSource>(self.Configuration, self, mapper, canThrow);
        }
        #endregion

        #region Or系の拡張メソッド
        /// <summary>
        /// このパーサーの読み取りが失敗したときに実行されるパーサーを指定します。
        /// <para>
        /// このパーサー（レシーバーとなるパーサー）の読み取りが成功した場合は、
        /// その結果がそのまま新しいパーサーの返す結果となります。
        /// 一方、このパーサーの読み取りが失敗した場合は、データソースの読み取り位置はリセットされ（バックトラック）、
        /// 引数で指定されたもう1つのパーサーの読み取りが試行され、その結果が新しいパーサーの返す結果となります。
        /// </para>
        /// <para>演算子<c>|</c>と<c>Or(...)</c>系メソッドはいずれも左結合です。
        /// つまり<c>p0 | p1 | p2</c>や<c>p0.Or(p1).Or(p2)</c>というコードは、概念的には<c>(p0 | p1) | p2</c>と解釈されます。
        /// もし仮に<c>p0</c>構築中のいずれかの地点で<see cref="Cut"/>が実行されており当該地点以降でトラックバックが無効化されている場合、
        /// これ以降の区間でパースが失敗すると当然<c>p1</c>は実行されないとしても、<c>p2</c>は引き続き実行されるということです。
        /// あえてこの挙動を変えるには<c>p0 | (p1 | p2)</c>や<c>p0.Or(p1.Or(p2))</c>というコードに変更する必要があります。
        /// </para>
        /// </summary>
        /// <param name="self">レシーバー</param>
        /// <param name="another">別のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser Or(this Parser self, Parser another)
        {
            return new OrParser(self.Configuration, self, another);
        }
        /// <summary>
        /// このパーサーの読み取りが失敗したときに実行されるパーサーを指定します。
        /// <para>
        /// このパーサー（レシーバーとなるパーサー）の読み取りが成功した場合は、
        /// その結果がそのまま新しいパーサーの返す結果となります。
        /// 一方、このパーサーの読み取りが失敗した場合は、データソースの読み取り位置はリセットされ（バックトラック）、
        /// 引数で指定されたもう1つのパーサーの読み取りが試行され、その結果が新しいパーサーの返す結果となります。
        /// </para>
        /// <para>演算子<c>|</c>と<c>Or(...)</c>系メソッドはいずれも左結合です。
        /// つまり<c>p0 | p1 | p2</c>や<c>p0.Or(p1).Or(p2)</c>というコードは、概念的には<c>(p0 | p1) | p2</c>と解釈されます。
        /// もし仮に<c>p0</c>構築中のいずれかの地点で<see cref="Cut"/>が実行されており当該地点以降でトラックバックが無効化されている場合、
        /// これ以降の区間でパースが失敗すると当然<c>p1</c>は実行されないとしても、<c>p2</c>は引き続き実行されるということです。
        /// あえてこの挙動を変えるには<c>p0 | (p1 | p2)</c>や<c>p0.Or(p1.Or(p2))</c>というコードに変更する必要があります。
        /// </para>
        /// </summary>
        /// <param name="self">レシーバー</param>
        /// <param name="another">別のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> Or<T>(this Parser<T> self, Parser<T> another)
        {
            return OrParser<T>.LeftAssoc(self, another);
        }
        /// <summary>
        /// このパーサーのパースの結果成否にかかわらずパース成功とみなす新しいパーサーを返します。
        /// </summary>
        /// <returns>新しいパーサー</returns>
        public static Parser OrNot(this Parser self)
        {
            return new OrNotParser(self.Configuration, self);
        }
        /// <summary>
        /// このパーサーのパースの結果成否にかかわらずパース成功とみなす新しいパーサーを返します。
        /// </summary>
        /// <returns>新しいパーサー</returns>
        public static Parser<Optional<T>> OrNot<T>(this Parser<T> self)
        {
            return new OrNotParser<T>(self.Configuration, self);
        }
        #endregion

        #region Repeat系の拡張メソッド
        /// <summary>
        /// シーケンスを読み取るパーサーを生成します。
        /// <para>
        /// 4つの引数はいずれもオプションです。
        /// 何も指定しなかった場合、0回以上で上限なしの繰り返しを表します。
        /// <paramref name="exactly"/>を指定した場合はまさにその回数の繰り返しです。
        /// <paramref name="min"/>および/もしくは<paramref name="max"/>を指定した場合は下限および/もしくは上限付きの繰り返しです。
        /// </para>
        /// </summary>
        /// <param name="self">レシーバー</param>
        /// <param name="min">繰り返しの最小回数</param>
        /// <param name="max">繰り返しの最大回数</param>
        /// <param name="exactly">繰り返しの回数</param>
        /// <param name="sep">セパレーターのためのパーサー</param>
        /// <returns>繰り返しをサポートする新しいパーサー</returns>
        public static Parser Repeat(this Parser self, int min = 0, int max = -1, int exactly = -1, Parser sep = null)
        {
            return RepeatParser<string>.Create(new DummyParser<string>(self), min, max, exactly, sep).Cast();
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
        /// <param name="self">レシーバー</param>
        /// <param name="min">繰り返しの最小回数</param>
        /// <param name="max">繰り返しの最大回数</param>
        /// <param name="exactly">繰り返しの回数</param>
        /// <param name="sep">セパレーターのためのパーサー</param>
        /// <returns>繰り返しをサポートする新しいパーサー</returns>
        public static Parser<IList<T>> Repeat<T>(this Parser<T> self, int min = 0, int max = -1, int exactly = -1, Parser sep = null)
        {
            return RepeatParser<T>.Create(self, min, max, exactly, sep);
        }
        #endregion

        #region Skip系の拡張メソッド
        /// <summary>
        /// パース対象に先行する空白文字もしくは指定された文字クラスをスキップするパーサーを返します。
        /// <para>新しいパーサーを元に生成される他のパーサーもこの設定を引き継ぎます。</para>
        /// </summary>
        /// <param name="self">レシーバー</param>
        /// <param name="target">スキップ対象の文字クラス</param>
        /// <returns>新しいパーサー</returns>
        public static Parser Skip(this Parser self, CharClass target = null)
        {
            return new SkipParser(self.Configuration, self, true, target ?? CharClass.SpaceAndControl);
        }
        /// <summary>
        /// パース対象に先行する空白文字もしくは指定された文字クラスをスキップするパーサーを返します。
        /// <para>新しいパーサーを元に生成される他のパーサーもこの設定を引き継ぎます。</para>
        /// </summary>
        /// <param name="self">レシーバー</param>
        /// <param name="target">スキップ対象の文字クラス</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> Skip<T>(this Parser<T> self, CharClass target = null)
        {
            return new SkipSpaceParser<T>(self.Configuration, self, true, target ?? CharClass.SpaceAndControl);
        }
        #endregion

        #region Then系の拡張メソッド
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
        public static Parser<T> Then<T>(this Parser self, Parser<T> another)
        {
            return new ThenTakeRightParser<T>(self.Configuration, self, another);
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
        public static Parser Then(this Parser self, Parser another)
        {
            return new ThenParser(self.Configuration, self, another);
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
        public static Parser<Tuple<T, U>> Then<T, U>(this Parser<T> self, Parser<U> another)
        {
            return new DoubleParser<T, U>(self.Configuration, self, another);
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
        public static Parser<T> Then<T>(this Parser<T> self, Parser another)
        {
            return new ThenTakeLeftParser<T>(self.Configuration, self, another);
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
        public static Parser<Tuple<T1, T2, T3>> Then<T1, T2, T3>(this Parser<T1> self, Parser<Tuple<T2, T3>> another)
        {
            return TripleParser<T1, T2, T3>.Create(self, another);
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
        #endregion
    }
}
