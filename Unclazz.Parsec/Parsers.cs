using System;
using System.Collections.Generic;
using Unclazz.Parsec.CharClasses;
using Unclazz.Parsec.CoreParsers;

namespace Unclazz.Parsec
{
    public static class Parsers
    {
        #region 定義済みパーサーを提供するプロパティの宣言
        /// <summary>
        /// データソースの先頭（BOF）にだけマッチするパーサーです。
        /// </summary>
        public static Parser BeginningOfFile { get; } = new BeginningOfFileParser();
        /// <summary>
        /// データソースの終端（EOF）にだけマッチするパーサーです。
        /// </summary>
        public static Parser EndOfFile { get; } = new EndOfFileParser();
        /// <summary>
        /// 0文字以上の空白文字(コードポイント<c>32</c>）と
        /// 制御文字（同<c>0</c>から<c>31</c>と<c>127</c>）にマッチするパーサーです。
        /// </summary>
        public static Parser WhileSpaceAndControls { get; } =
            new CharsWhileInParser(CharClass.Between((char)0, (char)32) + (char)127, 0);
        /// <summary>
        /// 0文字以上の制御文字（同<c>0</c>から<c>31</c>と<c>127</c>）にマッチするパーサーです。
        /// </summary>
        public static Parser WhileControls { get; } =
            new CharsWhileInParser(CharClass.Between((char)0, (char)31) + (char)127, 0);
        #endregion

        #region 静的ファクトリーメソッドの宣言
        /// <summary>
        /// パーサーのパース結果成否を反転させるパーサーを生成します。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser Not<T>(Parser<T> operand)
        {
            return new NotParser<T>(operand);
        }
        /// <summary>
        /// パーサーのパース結果成否を反転させるパーサーを生成します。
        /// </summary>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser Not(Parser operand)
        {
            return new NotParser<Nil>(operand);
        }
        /// <summary>
        /// デリゲートをもとにパーサーを生成します。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="func">パースの実処理を行うデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> For<T>(Func<ParserInput, ParseResult<T>> func)
        {
            return new DelegateParser<T>(func);
        }
        /// <summary>
        /// デリゲートをもとにパーサーを生成します。
        /// </summary>
        /// <param name="func">パースの実処理を行うデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public static Parser For(Func<ParserInput, ParseResult<Nil>> func)
        {
            return new DelegateParser<Nil>(func).Cast();
        }
        /// <summary>
        /// デリゲートを使用してパーサーを生成します。
        /// デリゲートはパースの直前になるまで実行されません。
        /// </summary>
        /// <typeparam name="T">パーサーが返す値の型</typeparam>
        /// <param name="factory">パーサーを生成するデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> Lazy<T>(Func<Parser<T>> factory)
        {
            return new LazyParser<T>(factory);
        }
        /// <summary>
        /// デリゲートを使用してパーサーを生成します。
        /// デリゲートはパースの直前になるまで実行されません。
        /// </summary>
        /// <param name="factory">パーサーを生成するデリゲート</param>
        /// <returns>新しいパーサー</returns>
        public static Parser Lazy(Func<Parser> factory)
        {
            return new LazyParser<Nil>(factory).Cast();
        }
        /// <summary>
        /// パーサーのパース失敗時に結果を反転させるパーサーを生成します。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="parser">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> OrNot<T>(Parser<T> parser)
        {
            return new OrNotParser<T>(parser);
        }
        /// <summary>
        /// <see cref="OrNot{T}(Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="parser">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser OrNot(Parser parser)
        {
            return new OrNotParser<Nil>(parser).Cast();
        }
        /// <summary>
        /// <see cref="Parser{T}.Or(Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="left">元のパーサー</param>
        /// <param name="right">元のパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> Or<T>(Parser<T> left, Parser<T> right)
        {
            return OrParser<T>.LeftAssoc(left, right);
        }
        /// <summary>
        /// <see cref="Parser{T}.Or(Parser{T})"/>と同義です。
        /// </summary>
        /// <param name="left">元になるパーサー</param>
        /// <param name="right">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser Or(Parser left, Parser right)
        {
            return OrParser<Nil>.LeftAssoc(left, right).Cast();
        }
        /// <summary>
        /// 指定された文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="ch">文字</param>
        /// <returns>新しいパーサー</returns>
        public static Parser Char(char ch)
        {
            return new CharParser(ch);
        }
        /// <summary>
        /// 指定された範囲に該当する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="start">範囲の開始</param>
        /// <param name="end">範囲の終了</param>
        /// <returns>新しいパーサー</returns>
        public static Parser CharBetween(char start, char end)
        {
            return new CharClassParser(CharClass.Between(start, end));
        }
        /// <summary>
        /// 指定された文字クラスに属する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="clazz">文字クラス</param>
        /// <returns>新しいパーサー</returns>
        public static Parser CharIn(CharClass clazz)
        {
            return new CharClassParser(clazz);
        }
        /// <summary>
        /// 指定された文字の集合に属する文字にマッチするパーサーを返します。
        /// </summary>
        /// <param name="chars">文字集合</param>
        /// <returns>新しいパーサー</returns>
        public static Parser CharIn(IEnumerable<char> chars)
        {
            return new CharClassParser(CharClass.AnyOf(chars));
        }
        /// <summary>
        /// 文字範囲に該当する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="start">範囲の開始</param>
        /// <param name="end">範囲の終了</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        public static Parser CharsWhileBetween(char start, char end, int min = 1)
        {
            return new CharsWhileBetweenParser(start, end, min);
        }
        /// <summary>
        /// 文字集合に属する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="chars">文字集合</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        public static Parser CharsWhileIn(IEnumerable<char> chars, int min = 1)
        {
            return new CharsWhileInParser(CharClass.AnyOf(chars), min);
        }
        /// <summary>
        /// 文字クラスに属する文字からなる文字列にマッチするパーサーを返します。
        /// </summary>
        /// <param name="clazz">文字クラス</param>
        /// <param name="min">最小の文字数</param>
        /// <returns>新しいパーサー</returns>
        public static Parser CharsWhileIn(CharClass clazz, int min = 1)
        {
            return new CharsWhileInParser(clazz, min);
        }
        /// <summary>
        /// 指定したキーワードにのみマッチするパーサーを生成します。
        /// オプションのパラメータによりカット（トラックバックの無効化）を行う文字位置を指定できます。
        /// パース処理がこの文字位置の以降に進んだ時、
        /// 直前の<see cref="Parser{T}.Or(Parser{T})"/>を起点とするトラックバックは無効になります。
        /// </summary>
        /// <param name="keyword">キーワード</param>
        /// <param name="cutIndex">カットを行う文字位置</param>
        /// <returns>新しいパーサー</returns>
        public static Parser Keyword(string keyword, int cutIndex = -1)
        {
            return new KeywordParser(keyword, cutIndex);
        }
        /// <summary>
        /// 指定したキーワードのいずれかにのみマッチするパーサーを生成します。
        /// </summary>
        /// <param name="keywords">キーワード</param>
        /// <returns>新しいパーサー</returns>
        public static Parser StringIn(params string[] keywords)
        {
            return new StringInParser(keywords);
        }
        #endregion
    }
}
