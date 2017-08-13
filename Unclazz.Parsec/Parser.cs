using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec.CharClass;

namespace Unclazz.Parsec
{
    public static class Parser
    {
        public static Parser<T> For<T>(Func<ParserInput, ParseResult<T>> func)
        {
            return new DelegateParser<T>(func);
        }
        public static Parser<T> Not<T>(Parser<T> operand)
        {
            return new NotParser<T>(operand);
        }
        public static Parser<T> Optional<T>(Parser<T> parser)
        {
            return new OptionalParser<T>(parser);
        }
        public static Parser<T> Optional<T>(Parser<T> parser, T defaultValue)
        {
            return new OptionalParser<T>(parser, defaultValue);
        }
        public static Parser<T> Or<T>(Parser<T> left, Parser<T> right)
        {
            return new OrParser<T>(left, right);
        }
        public static Parser<char> Char(CharClass.CharClass clazz)
        {
            return new CharClassParser(clazz);
        }
        public static Parser<char> Char(params char[] chars)
        {
            return new CharClassParser(new CharactersCharClass(chars));
        }
        public static Parser<char> Char(IEnumerable<char> chars)
        {
            return new CharClassParser(new CharactersCharClass(chars));
        }
        public static Parser<string> Word(string word)
        {
            return new WordParser(word);
        }
        public static Parser<string> WhiteSpaceAndControls { get; } =
            new WhileCharClassParser(new DelegateCharClass(ch => ch <= 32 || ch == 127));
        public static Parser<string> Controls { get; } =
            new WhileCharClassParser(new DelegateCharClass(ch => ch < 32 || ch == 127));

        public static Parser<string> Concat(this Parser<IEnumerable<char>> self)
        {
            return self.Map(cs => cs.Aggregate(new StringBuilder(), (b, ch) => b.Append(ch)).ToString());
        }
        public static Parser<string> Concat(this Parser<IEnumerable<string>> self)
        {
            return self.Map(cs => cs.Aggregate(new StringBuilder(), (b, s) => b.Append(s)).ToString());
        }
        public static Parser<IEnumerable<T>> Then<T>(this Parser<IEnumerable<T>> self, Parser<T> another)
        {
            return new ManyThenParser<T>(self, another);
        }
        public static Parser<IEnumerable<T>> Or<T>(this Parser<IEnumerable<T>> self, params T[] defaultValue)
        {
            return new OptionalParser<IEnumerable<T>>(self, defaultValue);
        }
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
            return left.Or(right);
        }
        public static Parser<IEnumerable<T>> operator &(Parser<T> left, Parser<T> right)
        {
            return left.Then(right);
        }
        public static Parser<IEnumerable<T>> operator &(Parser<IEnumerable<T>> left, Parser<T> right)
        {
            return new ManyThenParser<T>(left, right);
        }

        public abstract ParseResult<T> Parse(ParserInput input);

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
        public Parser<T> Or(T defaultValue)
        {
            return new OptionalParser<T>(this, defaultValue);
        }
    }
}
