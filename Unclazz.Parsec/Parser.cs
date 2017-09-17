using System;
using System.Text.RegularExpressions;
using Unclazz.Parsec.CoreParsers;

namespace Unclazz.Parsec
{
    /// <summary>
    /// キャプチャを行わず結果型を持たないパーサーを表す抽象クラスです。
    /// <para>
    /// <see cref="Parser{T}"/>とは対照的に、
    /// この抽象クラスから派生するパーサーはパースした値のキャプチャを行わず、パース成否のみを呼び出し元に返します。
    /// キャプチャが必要な場合は<see cref="Capture"/>や<see cref="Map{U}(Func{string, U})"/>メソッドを呼び出します。
    /// </para>
    /// </summary>
    public abstract class Parser
    {
        #region 演算子オーバーロードの宣言
        /// <summary>
        /// <see cref="Parsers.Not(Parser)"/>と同義です。
        /// </summary>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいインスタンス</returns>
        public static Parser operator !(Parser operand)
        {
            return new NotParser(operand);
        }
        /// <summary>
        /// <see cref="Or(Parser)"/>と同義です。
        /// </summary>
        /// <param name="left">元になるパーサー</param>
        /// <param name="right">元になるパーサー</param>
        /// <returns>新しいインスタンス</returns>
        public static Parser operator |(Parser left, Parser right)
        {
            return new OrParser(left, right);
        }
        /// <summary>
        /// <see cref="Or(Parser)"/>と同義です。
        /// 右被演算子は当該キーワードにマッチするパーサーに変換されます。
        /// </summary>
        /// <param name="left">元になるパーサー</param>
        /// <param name="right">元になるパーサー</param>
        /// <returns>新しいインスタンス</returns>
        public static Parser operator |(Parser left, string right)
        {
            return new OrParser(left, new KeywordParser(right));
        }
        /// <summary>
        /// <see cref="Or(Parser)"/>と同義です。
        /// 左被演算子は当該キーワードにマッチするパーサーに変換されます。
        /// </summary>
        /// <param name="left">元になるパーサー</param>
        /// <param name="right">元になるパーサー</param>
        /// <returns>新しいインスタンス</returns>
        public static Parser operator |(string left, Parser right)
        {
            return new OrParser(new KeywordParser(left), right);
        }
        /// <summary>
        /// <see cref="Then(Parser)"/>と同義です。
        /// </summary>
        /// <param name="left">元になるパーサー</param>
        /// <param name="right">元になるパーサー</param>
        /// <returns>新しいインスタンス</returns>
        public static Parser operator &(Parser left, Parser right)
        {
            return left.Then(right);
        }
        /// <summary>
        /// <see cref="Then(Parser)"/>と同義です。
        /// 右被演算子は当該キーワードにマッチするパーサーに変換されます。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Parser operator &(Parser left, string right)
        {
            return left.Then(new KeywordParser(right));
        }
        /// <summary>
        /// <see cref="Then(Parser)"/>と同義です。
        /// 左被演算子は当該キーワードにマッチするパーサーに変換されます。
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Parser operator &(string left, Parser right)
        {
            return new KeywordParser(left).Then(right);
        }
        #endregion

        /// <summary>
        /// デフォルトのコンフィギュレーションを使用するコンストラクタです。
        /// </summary>
        protected Parser()
        {
            _name = Regex.Replace(GetType().Name, "Parser$", string.Empty);
        }
        /// <summary>
        /// 引数で指定されたコンフィギュレーションを使用するコンストラクタです。
        /// </summary>
        /// <param name="config"></param>
        protected Parser(string name)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
        }

        readonly static ParserFactory _factory = new ParserFactory();
        readonly string _name;

        /// <summary>
        /// パースを行います。
        /// <para>
        /// パーサーの具象クラスを実装する場合、このメソッドを実装する必要があります。
        /// パース成否は<see cref="ResultCore"/>のインスタンスで表されます。
        /// このメソッドはいかなる場合も<c>null</c>を返してはなりません。
        /// またこのメソッドは原則として例外スローを行ってはなりません。
        /// 正常・異常を問わずこのメソッド内で起こったことはすべて
        /// <see cref="ResultCore"/>を通じて呼び出し元に通知される必要があります。
        /// </para>
        /// </summary>
        /// <param name="input">入力データ</param>
        /// <returns>パース結果</returns>
        protected abstract ResultCore DoParse(Context input);

        /// <summary>
        /// パース成功を表す<see cref="ResultCore"/>インスタンスを生成します。
        /// </summary>
        /// <returns></returns>
        protected ResultCore Success()
        {
            return ResultCore.OfSuccess(true);
        }
        /// <summary>
        /// パース成功を表す<see cref="ResultCore"/>インスタンスを生成します。
        /// </summary>
        /// <param name="canBacktrack">直近の<c>|</c>や<c>Or(...)</c>を
        /// 起点とするバックトラックを有効にするかどうか（デフォルトは<c>true</c>で、バックトラックは有効）</param>
        /// <returns></returns>
        protected ResultCore Success(bool canBacktrack)
        {
            return ResultCore.OfSuccess(canBacktrack);
        }
        /// <summary>
        /// パース失敗を表す<see cref="ResultCore"/>インスタンスを生成します。
        /// </summary>
        /// <param name="message">パース失敗の理由を示すメッセージ</param>
        /// <returns></returns>
        protected ResultCore Failure(string message)
        {
            return ResultCore.OfFailure(message, true);
        }
        /// <summary>
        /// パース失敗を表す<see cref="ResultCore"/>インスタンスを生成します。
        /// </summary>
        /// <param name="message">パース失敗の理由を示すメッセージ</param>
        /// <param name="canBacktrack">直近の<c>|</c>や<c>Or(...)</c>を
        /// 起点とするバックトラックを有効にするかどうか（デフォルトは<c>true</c>で、バックトラックは有効）</param>
        /// <returns></returns>
        protected ResultCore Failure(string message, bool canBacktrack)
        {
            return ResultCore.OfFailure(message, canBacktrack);
        }

        /// <summary>
        /// パースを行います。
        /// <para>
        /// パース成否は戻り値の<see cref="Result"/>のインスタンスで表されます。
        /// このメソッドはいかなる場合も<c>null</c>を返しません。
        /// またこのメソッドは原則として例外スローも行いません。
        /// 正常・異常を問わずこのメソッド内で起こったことはすべて
        /// <see cref="Result"/>を通じて呼び出し元に通知されます。
        /// </para>
        /// <para>
        /// このメソッドは事前処理の後、具象クラスが実装する<see cref="DoParse(Reader)"/>を呼び出します。
        /// その後事後処理を終えてから、呼び出し元に結果を返します。
        /// </para>
        /// </summary>
        /// <param name="input">入力データ</param>
        /// <returns>パース結果</returns>
        public Result Parse(Reader input)
        {
            return Parse(new Context(input));
        }
        public Result Parse(Context ctx)
        {
            var start = ctx.Source.Position;
            ctx.PreParse(_name);
            var resultCore = DoParse(ctx);
            ctx.PostParse(resultCore);
            return resultCore.AttachPosition(start, ctx.Source.Position);
        }

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
        public Parser<string> Capture()
        {

            return new CaptureParser(this);
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
        public Parser Cut()
        {
            return new CutParser(this);
        }
        /// <summary>
        /// <c>Capture().Map(...)</c>と同義です。
        /// </summary>
        /// <typeparam name="U">読み取り結果を変換した後の型</typeparam>
        /// <param name="func">変換を行う関数</param>
        /// <returns></returns>
        public Parser<U> Map<U>(Func<string, U> func)
        {
            return Capture().Map(func);
        }
        /// <summary>
        /// <c>Capture().Map(...)</c>と同義です。
        /// </summary>
        /// <typeparam name="U">読み取り結果を変換した後の型</typeparam>
        /// <param name="func">変換を行う関数</param>
        /// <param name="canThrow"><c>true</c>の場合<paramref name="func"/>がスローした例外をそのまま再スローする</param>
        /// <returns></returns>
        public Parser<U> Map<U>(Func<string, U> func, bool canThrow)
        {
            return Capture().Map(func, canThrow);
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
        public Parser Or(Parser another)
        {
            return new OrParser(this, another);
        }
        /// <summary>
        /// このパーサーのパースの結果成否にかかわらずパース成功とみなす新しいパーサーを返します。
        /// </summary>
        /// <returns>新しいパーサー</returns>
        public Parser OrNot()
        {
            return new OptionalParser(this);
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
        public Parser Repeat(int min = 0, int max = -1, int exactly = -1, Parser sep = null)
        {
            return new RepeatParser<int>(Typed(0), min, max, exactly,sep).Untyped();
        }
        /// <summary>
        /// パース対象に先行する指定された文字クラスをスキップするパーサーを返します。
        /// <para>新しいパーサーを元に生成される他のパーサーもこの設定を引き継ぎます。</para>
        /// </summary>
        /// <param name="target">スキップ対象の文字クラス</param>
        /// <returns>新しいパーサー</returns>
        public Parser AutoSkip(CharClass target)
        {
            return new SkipParser(this, target);
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
        public Parser Then(Parser another)
        {
            return new ThenParser(this, another);
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
        public Parser<T> Then<T>(Parser<T> another)
        {
            return new ThenTakeRightParser<T>(this, another);
        }
        /// <summary>
        /// パース結果型を持つパーサーに変換します。
        /// <typeparamref name="T"/>のデフォルト値が結果値として採用されます。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Parser<T> Typed<T>() where T : struct
        {
            return new TypedParser<T>(this);
        }
        /// <summary>
        /// パース結果型を持つパーサーに変換します。
        /// 引数で指定さた値が結果値として採用されます。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public Parser<T> Typed<T>(T value)
        {
            return new TypedParser<T>(this, value);
        }

    }
}
