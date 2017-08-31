using System;
using System.Collections.Generic;
using Unclazz.Parsec.CharClasses;
using Unclazz.Parsec.CoreParsers;

namespace Unclazz.Parsec
{
    /// <summary>
    /// パーサーの静的ファクトリー・メソッドを提供するユーティリティです。
    /// </summary>
    public static class Parsers
    {
        static readonly ParserFactory _factory = ParserFactory.Default;

        #region 定義済みパーサーを提供するプロパティの宣言
        /// <summary>
        /// データソースの先頭（BOF）にだけマッチするパーサーです。
        /// </summary>
        public static Parser BeginningOfFile { get; } = _factory.BeginningOfFile;
        /// <summary>
        /// データソースの終端（EOF）にだけマッチするパーサーです。
        /// </summary>
        public static Parser EndOfFile { get; } = _factory.EndOfFile;
        /// <summary>
        /// 0文字以上の空白文字(コードポイント<c>32</c>）と
        /// 制御文字（同<c>0</c>から<c>31</c>と<c>127</c>）にマッチするパーサーです。
        /// </summary>
        public static Parser WhileSpaceAndControls { get; } = _factory.WhileSpaceAndControls;
        #endregion

        #region 静的ファクトリーメソッドの宣言
        /// <summary>
        /// パーサーのパース結果成否を反転させるパーサーを生成します。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser Not<T>(Parser<T> operand) => _factory.Not(operand);
        /// <summary>
        /// パーサーのパース結果成否を反転させるパーサーを生成します。
        /// </summary>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser Not(Parser operand) => _factory.Not(operand);
        /// <summary>
        /// デリゲートをもとにパーサーを生成します。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="func">パースの実処理を行うデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> For<T>(Func<Reader, Result<T>> func) => _factory.For(func);
        /// <summary>
        /// デリゲートをもとにパーサーを生成します。
        /// </summary>
        /// <param name="func">パースの実処理を行うデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public static Parser For(Func<Reader, Result> func) => _factory.For(func);
        /// <summary>
        /// デリゲートを使用してパーサーを生成します。
        /// デリゲートはパースの直前になるまで実行されません。
        /// </summary>
        /// <typeparam name="T">パーサーが返す値の型</typeparam>
        /// <param name="factory">パーサーを生成するデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> Lazy<T>(Func<Parser<T>> factory) => _factory.Lazy(factory);
        /// <summary>
        /// デリゲートを使用してパーサーを生成します。
        /// デリゲートはパースの直前になるまで実行されません。
        /// </summary>
        /// <param name="factory">パーサーを生成するデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public static Parser Lazy(Func<Parser> factory) => _factory.Lazy(factory);
        /// <summary>
        /// 指定された文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="ch">文字</param>
        /// <returns>新しいパーサー</returns>
        public static Parser Char(char ch) => _factory.Char(ch);
        /// <summary>
        /// 指定された範囲に該当する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="start">範囲の開始</param>
        /// <param name="end">範囲の終了</param>
        /// <returns>新しいパーサー</returns>
        public static Parser CharBetween(char start, char end) => _factory.CharBetween(start, end);
        /// <summary>
        /// 指定された文字クラスに属する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="clazz">文字クラス</param>
        /// <returns>新しいパーサー</returns>
        public static Parser CharIn(CharClass clazz) => _factory.CharIn(clazz);
        /// <summary>
        /// 指定された文字の集合に属する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="chars">文字集合</param>
        /// <returns>新しいパーサー</returns>
        public static Parser CharIn(IEnumerable<char> chars) => _factory.CharIn(chars);
        /// <summary>
        /// 文字範囲に該当する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="start">範囲の開始</param>
        /// <param name="end">範囲の終了</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        public static Parser CharsWhileBetween(char start, char end, int min = 1) => _factory.CharsWhileBetween(start, end, min);
        /// <summary>
        /// 文字集合に属する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="chars">文字集合</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        public static Parser CharsWhileIn(IEnumerable<char> chars, int min = 1) => _factory.CharsWhileIn(chars, min);
        /// <summary>
        /// 文字クラスに属する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="clazz">文字クラス</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        public static Parser CharsWhileIn(CharClass clazz, int min = 1) => _factory.CharsWhileIn(clazz, min);
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
        public static Parser Keyword(string keyword, int cutIndex = -1) => _factory.Keyword(keyword, cutIndex);
        /// <summary>
        /// 指定したキーワードのいずれかにのみマッチするパーサーを生成します。
        /// </summary>
        /// <param name="keywords">キーワード</param>
        /// <returns>新しいパーサー</returns>
        public static Parser StringIn(params string[] keywords) => _factory.StringIn(keywords);
        /// <summary>
        /// 指定した値をキャプチャ結果とするパーサーを生成します。
        /// <para>このパーサーは実際には読み取りは行わず、パース結果は必ず成功となります。
        /// キャプチャを行わないパーサーに<c>&amp;</c>や<c>Then(...)</c>で結合することで、
        /// そのパーサーの代わりにキャプチャ結果を作り出すように働きます。</para>
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="value">キャプチャ結果となる値</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> Yield<T>(T value) => _factory.Yield(value);
        #endregion
    }
}
