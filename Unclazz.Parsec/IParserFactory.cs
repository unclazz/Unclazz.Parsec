using System;
using System.Collections.Generic;
using Unclazz.Parsec.CharClasses;
using Unclazz.Parsec.CoreParsers;

namespace Unclazz.Parsec
{
    /// <summary>
    /// 定義済みパーサーのファクトリーオブジェクトを表すインターフェースです。
    /// </summary>
    public interface IParserFactory
    {
        /// <summary>
        /// このファクトリーで使用されるコンフィギュレーションです。
        /// </summary>
        IParserConfiguration Configuration { get; }

        #region 定義済みパーサーを提供するプロパティの宣言
        /// <summary>
        /// データソースの先頭（BOF）にだけマッチするパーサーです。
        /// </summary>
        Parser BeginningOfFile { get; }
        /// <summary>
        /// データソースの終端（EOF）にだけマッチするパーサーです。
        /// </summary>
        Parser EndOfFile { get; }
        /// <summary>
        /// 0文字以上の空白文字(コードポイント<c>32</c>）と
        /// 制御文字（同<c>0</c>から<c>31</c>と<c>127</c>）にマッチするパーサーです。
        /// </summary>
        Parser WhileSpaceAndControls { get; }
        #endregion

        #region 定義済みパーサーを提供するメソッドの宣言
        /// <summary>
        /// <c>"\\u"</c>もしくは任意の接頭辞から始まるUnicodeエスケープシーケンスを読み取ります。
        /// Unicode拡張領域の文字は上位サロゲートと下位サロゲートのそれぞれ単体でパースされます。
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        Parser<char> Utf16UnicodeEscape(string prefix = "\\u");
        /// <summary>
        /// 制御文字のエスケープシーケンスを読み取ります。
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        Parser<char> ControlEscape(char prefix = '\\');
        /// <summary>
        /// 任意の文字のエスケープシーケンスを読み取ります。
        /// 読み取り結果はその文字そのもの、つまりエスケープシーケンスから接頭辞を除去したものです。
        /// </summary>
        /// <param name="chars"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        Parser<char> CharEscape(IEnumerable<char> chars, char prefix = '\\');
        /// <summary>
        /// 引用符で囲われた文字列を読み取ります。
        /// デフォルトでは引用符自体を含めていかなるエスケープシーケンスも認識しません。
        /// パース対象文字列にエスケープシーケンスが含まれる場合は、
        /// 当該シーケンスを適切にハンドルするパーサーを引数で指定してください。
        /// </summary>
        /// <param name="quote"></param>
        /// <param name="escape"></param>
        /// <returns></returns>
        Parser<string> QuotedString(char quote = '\"', Parser<char> escape = null);

        /// <summary>
        /// 指定された文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="ch">文字</param>
        /// <returns>新しいパーサー</returns>
        CharParser Char(char ch);
        /// <summary>
        /// 指定された範囲に該当する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="start">範囲の開始</param>
        /// <param name="end">範囲の終了</param>
        /// <returns>新しいパーサー</returns>
        CharParser CharBetween(char start, char end);
        /// <summary>
        /// 指定された文字クラスに属する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="clazz">文字クラス</param>
        /// <returns>新しいパーサー</returns>
        CharParser CharIn(CharClass clazz);
        /// <summary>
        /// 指定された文字の集合に属する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="chars">文字集合</param>
        /// <returns>新しいパーサー</returns>
        CharParser CharIn(IEnumerable<char> chars);
        /// <summary>
        /// 文字範囲に該当する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="start">範囲の開始</param>
        /// <param name="end">範囲の終了</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        Parser CharsWhileBetween(char start, char end, int min = 1);
        /// <summary>
        /// 文字クラスに属する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="clazz">文字クラス</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        Parser CharsWhileIn(CharClass clazz, int min = 1);
        /// <summary>
        /// 文字集合に属する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="chars">文字集合</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        Parser CharsWhileIn(IEnumerable<char> chars, int min = 1);
        /// <summary>
        /// デリゲートをもとにパーサーを生成します。
        /// </summary>
        /// <param name="func">パースの実処理を行うデリゲート</param>
        /// <returns>新しいパーサー</returns>
        Parser For(Func<Reader, Result> func);
        /// <summary>
        /// デリゲートをもとにパーサーを生成します。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="func">パースの実処理を行うデリゲート</param>
        /// <returns>新しいパーサー</returns>
        Parser<T> For<T>(Func<Reader, Result<T>> func);
        /// <summary>
        /// 指定したキーワードにのみマッチするパーサーを生成します。
        /// <para>
        /// <paramref name="cutIndex"/>によりカット（トラックバックの無効化）を行う文字位置を指定できます。
        /// パース処理がこの文字位置の以降に進んだ時、直前の<c>|</c>や<c>Or(...)</c>を起点とするトラックバックは無効になります。
        /// </para>
        /// </summary>
        /// <param name="keyword">キーワード</param>
        /// <param name="cutIndex">カットを行う文字位置</param>
        /// <returns>新しいパーサー</returns>
        Parser Keyword(string keyword, int cutIndex = -1);
        /// <summary>
        /// デリゲートを使用してパーサーを生成します。
        /// デリゲートはパースの直前になるまで実行されません。
        /// </summary>
        /// <param name="factory">パーサーを生成するデリゲート</param>
        /// <returns>新しいパーサー</returns>
        Parser Lazy(Func<Parser> factory);
        /// <summary>
        /// デリゲートを使用してパーサーを生成します。
        /// デリゲートはパースの直前になるまで実行されません。
        /// </summary>
        /// <typeparam name="T">パーサーが返す値の型</typeparam>
        /// <param name="factory">パーサーを生成するデリゲート</param>
        /// <returns>新しいパーサー</returns>
        Parser<T> Lazy<T>(Func<Parser<T>> factory);
        /// <summary>
        /// 先読み（look-ahead）を行うパーサーを生成します。
        /// <para>このパーサーはその成否に関わらず文字位置を前進させません。</para>
        /// </summary>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        Parser Lookahead(Parser operand);
        /// <summary>
        /// パーサーのパース結果成否を反転させるパーサーを生成します。
        /// </summary>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        Parser Not(Parser operand);
        /// <summary>
        /// パーサーのパース結果成否を反転させるパーサーを生成します。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        Parser Not<T>(Parser<T> operand);
        /// <summary>
        /// 指定したキーワードのいずれかにのみマッチするパーサーを生成します。
        /// </summary>
        /// <param name="keywords">キーワード</param>
        /// <returns>新しいパーサー</returns>
        Parser KeywordIn(params string[] keywords);
        /// <summary>
        /// 指定した値をキャプチャ結果とするパーサーを生成します。
        /// <para>このパーサーは実際には読み取りは行わず、パース結果は必ず成功となります。
        /// キャプチャを行わないパーサーに<c>&amp;</c>や<c>Then(...)</c>で結合することで、
        /// そのパーサーの代わりにキャプチャ結果を作り出すように働きます。</para>
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="value">キャプチャ結果となる値</param>
        /// <returns>新しいパーサー</returns>
        Parser<T> Yield<T>(T value);
        #endregion
    }
}