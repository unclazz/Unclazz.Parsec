using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unclazz.Parsec.CharClasses;
using Unclazz.Parsec.Intrinsics;

namespace Unclazz.Parsec
{
    /// <summary>
    /// パーサーを表す抽象クラスです。
    /// <para>
    /// <see cref="Parser"/>とは対照的に、
    /// この抽象クラスから派生するパーサーはパースした値のキャプチャを行い、パース成否の情報とともに呼び出し元に返します。
    /// キャプチャ結果が不要な場合は<see cref="Untyped"/>を使用します。
    /// </para>
    /// <para>
    /// <see cref="Parser{T}"/>はまた多くの<c>protected</c>なファクトリーメソッドを派生クラスに対して公開しています。
    /// ライブラリのユーザー開発者はこの抽象クラスの派生クラスを宣言することで独自のパーサーを組み立てることができます。
    /// </para>
    /// </summary>
    /// <typeparam name="T">パース結果の型</typeparam>
    public abstract class Parser<T> : ParserBase
    {
        #region 演算子オーバーロードの宣言
        /// <summary>
        /// デリゲートをもとにパーサーを生成します。
        /// </summary>
        /// <param name="func"></param>
        public static implicit operator Parser<T>(Func<Context, Result<T>> func)
        {
            return For(func);
        }
        /// <summary>
        /// デリゲートを使用してパーサーを生成します。
        /// デリゲートはパースの直前になるまで実行されません。
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static implicit operator Parser<T>(Func<Parser<T>> factory)
        {
            return Lazy(factory);
        }
        /// <summary>
        /// パーサーのパース結果成否を反転させるパーサーを生成します。
        /// </summary>
        /// <param name="operand">元になるパーサー</param>
        /// <returns></returns>
        public static Parser operator !(Parser<T> operand)
        {
            return Not(operand);
        }
        /// <summary>
        /// <see cref="Or(Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns></returns>
        public static Parser<T> operator |(Parser<T> left, Parser<T> right)
        {
            return new OrParser<T>(left, right);
        }
        /// <summary>
        /// <see cref="Or(Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns></returns>
        public static Parser operator |(Parser<T> left, Parser right)
        {
            return new OrParser(left.Untyped(), right);
        }
        /// <summary>
        /// <see cref="Parser.Or(Parser)"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns></returns>
        public static Parser operator |(Parser left, Parser<T> right)
        {
            return new OrParser(left, right.Untyped());
        }
        /// <summary>
        /// <see cref="Then{U}(Parser{U})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns></returns>
        public static Parser<Tuple<T, T>> operator &(Parser<T> left, Parser<T> right)
        {
            return new DoubleParser<T, T>(left, right);
        }
        /// <summary>
        /// <see cref="Parser.Then{T}(Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns></returns>
        public static Parser<T> operator &(Parser left, Parser<T> right)
        {
            return new ThenTakeRightParser<T>(left, right);
        }
        /// <summary>
        /// <see cref="Then(Parser)"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns></returns>
        public static Parser<T> operator &(Parser<T> left, Parser right)
        {
            return new ThenTakeLeftParser<T>(left, right);
        }
        /// <summary>
        /// <see cref="Then(Parser)"/>と同義です。
        /// 右被演算子は<see cref="ParserBase.Keyword(string, int)"/>によりパーサーに変換されます。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Parser<T> operator &(Parser<T> left, string right)
        {
            return left.Then(new KeywordParser(right));
        }
        /// <summary>
        /// <see cref="Parser.Then{T}(Parser{T})"/>と同義です。
        /// 左被演算子は<see cref="ParserBase.Keyword(string, int)"/>によりパーサーに変換されます。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Parser<T> operator &(string left, Parser<T> right)
        {
            return new KeywordParser(left).Then(right);
        }
        #endregion

        /// <summary>
        /// デフォルトのコンストラクタです。
        /// <see cref="ParserBase.Name"/>には型名から導出された値が設定されます。
        /// </summary>
        protected Parser() : base() { }
        /// <summary>
        /// 任意のパーサー名を指定できるコンストラクタです。
        /// </summary>
        /// <param name="name">パーサー名</param>
        protected Parser(string name) : base(name) { }

        /// <summary>
        /// パースを行います。
        /// <para>
        /// パーサーの具象クラスを実装する場合、このメソッドを実装する必要があります。
        /// パース成否は<see cref="ResultCore{T}"/>のインスタンスで表されます。
        /// このメソッドはいかなる場合も<c>null</c>を返してはなりません。
        /// またこのメソッドは原則として例外スローを行ってはなりません。
        /// 正常・異常を問わずこのメソッド内で起こったことはすべて
        /// <see cref="ResultCore{T}"/>を通じて呼び出し元に通知される必要があります。
        /// </para>
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>パース結果</returns>
        protected abstract ResultCore<T> DoParse(Context ctx);

        /// <summary>
        /// パースを行います。
        /// <para>
        /// パース成否は戻り値の<see cref="Result{T}"/>のインスタンスで表されます。
        /// このメソッドはいかなる場合も<c>null</c>を返しません。
        /// またこのメソッドは原則として例外スローも行いません。
        /// 正常・異常を問わずこのメソッド内で起こったことはすべて
        /// <see cref="Result{T}"/>を通じて呼び出し元に通知されます。
        /// </para>
        /// <para>
        /// このメソッドは事前処理の後、具象クラスが実装する<see cref="DoParse(Context)"/>を呼び出します。
        /// その後事後処理を終えてから、呼び出し元に結果を返します。
        /// </para>
        /// </summary>
        /// <param name="src">入力データ</param>
        /// <returns>パース結果</returns>
        public Result<T> Parse(Reader src)
        {
            return Parse(new Context(src));
        }
        /// <summary>
        /// パースを行います。
        /// <para>
        /// パース成否は戻り値の<see cref="Result{T}"/>のインスタンスで表されます。
        /// このメソッドはいかなる場合も<c>null</c>を返しません。
        /// またこのメソッドは原則として例外スローも行いません。
        /// 正常・異常を問わずこのメソッド内で起こったことはすべて
        /// <see cref="Result{T}"/>を通じて呼び出し元に通知されます。
        /// </para>
        /// <para>
        /// このメソッドは事前処理の後、具象クラスが実装する<see cref="DoParse(Context)"/>を呼び出します。
        /// その後事後処理を終えてから、呼び出し元に結果を返します。
        /// </para>
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>パース結果</returns>
        public Result<T> Parse(Context ctx)
        {
            var start = ctx.Source.Position;
            ctx.PreParse(Name);
            var resultCore = DoParse(ctx);
            ctx.PostParse(resultCore);
            return resultCore.AttachPosition(start, ctx.Source.Position);
        }
        /// <summary>
        /// パース成功を表す<see cref="ResultCore{T}"/>インスタンスを生成します。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected ResultCore<T> Success(T value)
        {
            return ResultCore<T>.OfSuccess(value, true);
        }
        /// <summary>
        /// パース成功を表す<see cref="ResultCore{T}"/>インスタンスを生成します。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="canBacktrack">直近の<c>|</c>や<c>Or(...)</c>を
        /// 起点とするバックトラックを有効にするかどうか（デフォルトは<c>true</c>で、バックトラックは有効）</param>
        /// <returns></returns>
        protected ResultCore<T> Success(T value, bool canBacktrack)
        {
            return ResultCore<T>.OfSuccess(value, canBacktrack);
        }
        /// <summary>
        /// パース失敗を表す<see cref="ResultCore{T}"/>インスタンスを生成します。
        /// </summary>
        /// <param name="message">パース失敗の理由を示すメッセージ</param>
        /// <returns></returns>
        protected ResultCore<T> Failure(string message)
        {
            return ResultCore<T>.OfFailure(message, true);
        }
        /// <summary>
        /// パース失敗を表す<see cref="ResultCore{T}"/>インスタンスを生成します。
        /// </summary>
        /// <param name="message">パース失敗の理由を示すメッセージ</param>
        /// <param name="canBacktrack">直近の<c>|</c>や<c>Or(...)</c>を
        /// 起点とするバックトラックを有効にするかどうか（デフォルトは<c>true</c>で、バックトラックは有効）</param>
        /// <returns></returns>
        protected ResultCore<T> Failure(string message, bool canBacktrack)
        {
            return ResultCore<T>.OfFailure(message, canBacktrack);
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
        public Parser<T> Cut()
        {
            return new CutParser<T>(this);
        }
        /// <summary>
        /// パース結果の値を元に動的にパーサーを構築するパーサーを返します。
        /// </summary>
        /// <typeparam name="U">読み取り結果を変換した後の型</typeparam>
        /// <param name="mapper">元のパーサーの読み取り結果から動的にパーサーを生成する関数</param>
        /// <returns>新しいパーサー</returns>
        public Parser<U> FlatMap<U>(Func<T, Parser<U>> mapper)
        {
            return new FlatMapParser<T, U>(this, mapper, false);
        }
        /// <summary>
        /// <see cref="FlatMap{U}(Func{T, Parser{U}})"/>とほぼ同義ですが、
        /// <paramref name="canThrow"/>に<c>true</c>を指定することで
        /// <paramref name="mapper"/>適用時にスローされた例外をそのまま再スローします。
        /// </summary>
        /// <typeparam name="U">読み取り結果を変換した後の型</typeparam>
        /// <param name="mapper">元のパーサーの読み取り結果から動的にパーサーを生成する関数</param>
        /// <param name="canThrow"><c>true</c>の場合<paramref name="mapper"/>がスローした例外をそのまま再スローする</param>
        /// <returns>新しいパーサー</returns>
        public Parser<U> FlatMap<U>(Func<T, Parser<U>> mapper, bool canThrow)
        {
            return new FlatMapParser<T, U>(this, mapper, canThrow);
        }
        /// <summary>
        /// 読み取り結果値に関数を適用するパーサーを生成します。
        /// <para>
        /// このメソッドが返すパーサーは関数<paramref name="func"/>が例外をスローした場合、
        /// そのメッセージを使用してパース失敗を表す<see cref="Result{T}"/>インスタンスを返します。
        /// </para>
        /// </summary>
        /// <typeparam name="U">読み取り結果を変換した後の型</typeparam>
        /// <param name="func">変換を行う関数</param>
        /// <returns>新しいパーサー</returns>
        public Parser<U> Map<U>(Func<T, U> func)
        {
            return new MapParser<T, U>(this, func, false);
        }
        /// <summary>
        /// <see cref="Map{U}(Func{T,U})"/>とほぼ同義ですが、
        /// <paramref name="canThrow"/>に<c>true</c>を指定することで
        /// <paramref name="func"/>適用時にスローされた例外をそのまま再スローします。
        /// </summary>
        /// <typeparam name="U">読み取り結果を変換した後の型</typeparam>
        /// <param name="func">変換を行う関数</param>
        /// <param name="canThrow"><c>true</c>の場合<paramref name="func"/>がスローした例外をそのまま再スローする</param>
        /// <returns>新しいパーサー</returns>
        public Parser<U> Map<U>(Func<T, U> func, bool canThrow)
        {
            return new MapParser<T, U>(this, func, canThrow);
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
        /// <param name="another">別のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public Parser<T> Or(Parser<T> another)
        {
            return new OrParser<T>(this, another);
        }
        /// <summary>
        /// このパーサーのパースが失敗した時、代わりに指定した値を返すパーサーを返します。
        /// </summary>
        /// <param name="value">代替値</param>
        /// <returns></returns>
        public Parser<T> OrElse(T value)
        {
            return Or(Yield(value));
        }
        /// <summary>
        /// このパーサーのパースの結果成否にかかわらずパース成功とみなす新しいパーサーを返します。
        /// </summary>
        /// <returns>新しいパーサー</returns>
        public Parser<Optional<T>> OrNot()
        {
            return new OptionalParser<T>(this);
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
        public RepeatParser<T> Repeat(int min = 0, int max = -1, int exactly = -1, Parser sep = null)
        {
            return new RepeatParser<T>(this, min, max, exactly, sep);
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
        /// <param name="another">別のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public Parser<T> Then(Parser another)
        {
            return new ThenTakeLeftParser<T>(this, another);
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
        /// <param name="another">別のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public Parser<Tuple<T, U>> Then<U>(Parser<U> another)
        {
            return new DoubleParser<T, U>(this, another);
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
        /// <param name="another">別のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public Parser<Tuple<T, T2, T3>> Then<T2, T3>(Parser<Tuple<T2, T3>> another)
        {
            return TripleParser<T, T2, T3>.Create(this, another);
        }
        /// <summary>
        /// パース結果型を持たないパーサーに変換します。
        /// 元のパーサーがキャプチャした値は破棄されます。
        /// </summary>
        /// <returns></returns>
        public Parser Untyped()
        {
            return new UntypedParser<T>(this);
        }
    }
}
