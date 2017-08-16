using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec.CharClass;

namespace Unclazz.Parsec
{
    /// <summary>
    /// <see cref="Parser{T}"/>のコンパニオン・オブジェクトです。
    /// <see cref="Parser{T}"/>のインスタンスを生成するためのユーティリティとして機能します。
    /// </summary>
    public static class Parser
    {
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
        /// パーサーのパース結果成否を反転させるパーサーを生成します。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="operand">元になるパーサー</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<T> Not<T>(Parser<T> operand)
        {
            return new NotParser<T>(operand);
        }
        /// <summary>
        /// パーサーのパース失敗時に結果を反転させるパーサーを生成します。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="parser">元になるパーサー</param>
        /// <returns></returns>
        public static Parser<T> Optional<T>(Parser<T> parser)
        {
            return new OptionalParser<T>(parser);
        }
        /// <summary>
        /// いずれか片方のパースが成功すれば全体の結果も成功とするパーサーを生成します。
        /// <paramref name="left"/>のパース失敗時のみ<paramref name="right"/>のパースが試みられます。
        /// </summary>
        /// <typeparam name="T">任意の型</typeparam>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Parser<T> Or<T>(Parser<T> left, Parser<T> right)
        {
            return new OrParser<T>(left, right);
        }
        public static Parser<string> Char(CharClass.CharClass clazz)
        {
            return new CharClassParser(clazz);
        }
        public static Parser<string> Char(params char[] chars)
        {
            return new CharClassParser(new CharactersCharClass(chars));
        }
        public static Parser<string> Char(IEnumerable<char> chars)
        {
            return new CharClassParser(new CharactersCharClass(chars));
        }
        /// <summary>
        /// 指定した文字列にのみマッチするパーサーを生成します。
        /// </summary>
        /// <param name="word">文字列</param>
        /// <returns>新しいパーサー</returns>
        public static Parser<string> Word(string word)
        {
            return new WordParser(word);
        }

        /// <summary>
        /// データソースの先頭（BOF）にだけマッチするパーサーです。
        /// </summary>
        public static Parser<string> BeginningOfFile { get; } = new BeginningOfFileParser();
        /// <summary>
        /// データソースの終端（EOF）にだけマッチするパーサーです。
        /// </summary>
        public static Parser<string> EndOfFile { get; } = new EndOfFileParser();
        /// <summary>
        /// 0文字以上の空白文字(コードポイント<c>32</c>）と制御文字（同<c>0</c>から<c>31</c>と<c>127</c>）にマッチするパーサーです。
        /// </summary>
        public static Parser<string> WhileSpaceAndControls { get; } =
            new WhileCharClassParser(CharClass.CharClass.Between((char)0, (char)32) + (char)127);
        /// <summary>
        /// 0文字以上の制御文字（同<c>0</c>から<c>31</c>と<c>127</c>）にマッチするパーサーです。
        /// </summary>
        public static Parser<string> WhileControls { get; } =
            new WhileCharClassParser(CharClass.CharClass.Between((char)0, (char)31) + (char)127);

    }

    public abstract class Parser<T>
    {
        public static implicit operator Parser<T>(Func<ParserInput, ParseResult<T>> func)
        {
            return Parser.For(func);
        }
        public static Parser<T> operator!(Parser<T> operand)
        {
            return Parser.Not<T>(operand);
        }
        public static Parser<T> operator|(Parser<T> left, Parser<T> right)
        {
            return Parser.Or<T>(left, right);
        }
        public static Parser<T> operator|(Parser<T> left, T right)
        {
            return left.Or(new SuccessParser<T>(right));
        }
        public static Parser<IEnumerable<T>> operator +(Parser<T> left, Parser<T> right)
        {
            return left.Then(right);
        }
        public static Parser<IEnumerable<T>> operator +(Parser<IEnumerable<T>> left, Parser<T> right)
        {
            return new ManyThenParser<T>(left, right);
        }
        public static Parser<IEnumerable<T>> operator +(Parser<T> left, Parser<IEnumerable<T>> right)
        {
            return new ThenManyParser<T>(left, right);
        }

        public abstract ParseResult<T> Parse(ParserInput input);

        protected ParseResult<T> Success(CharacterPosition p)
        {
            return ParseResult.OfSuccess<T>(p);
        }
        protected ParseResult<T> Failure(CharacterPosition p, string m)
        {
            return ParseResult.OfFailure<T>(p, m);
        }

        public Parser<U> Map<U>(Func<T,U> transform)
        {
            return new MapParser<T, U>(this, transform);
        }
        public Parser<IEnumerable<T>> Repeat(int min, int max)
        {
            return new RepeatMinMaxParser<T>(this, min, max);
        }
        public Parser<IEnumerable<T>> RepeatMin(int min)
        {
            return new RepeatMinMaxParser<T>(this, min, -1);
        }
        public Parser<IEnumerable<T>> RepeatMax(int max)
        {
            return new RepeatMinMaxParser<T>(this, 0, max);
        }
        public Parser<IEnumerable<T>> RepeatExactly(int exactly)
        {
            return new RepeatExactlyParser<T>(this, exactly);
        }
        public Parser<T> Or(Parser<T> another)
        {
            return new OrParser<T>(this, another);
        }
        public Parser<IEnumerable<T>> Then(Parser<T> another)
        {
            return new ThenParser<T>(this, another);
        }
    }
}
