using System;
using System.Collections.Generic;
using Unclazz.Parsec.CharClasses;

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

        Parser Char(char ch);
        Parser CharBetween(char start, char end);
        Parser CharIn(CharClass clazz);
        Parser CharIn(IEnumerable<char> chars);
        Parser CharsWhileBetween(char start, char end, int min = 1);
        Parser CharsWhileIn(CharClass clazz, int min = 1);
        Parser CharsWhileIn(IEnumerable<char> chars, int min = 1);
        Parser For(Func<Reader, ParseResult<Nil>> func);
        Parser<T> For<T>(Func<Reader, ParseResult<T>> func);
        Parser Keyword(string keyword, int cutIndex = -1);
        Parser Lazy(Func<Parser> factory);
        Parser<T> Lazy<T>(Func<Parser<T>> factory);
        Parser Not(Parser operand);
        Parser Not<T>(Parser<T> operand);
        Parser StringIn(params string[] keywords);
        Parser<T> Yield<T>(T value);
    }
}