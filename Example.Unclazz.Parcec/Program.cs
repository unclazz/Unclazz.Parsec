using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Commons.Json;
using Unclazz.Parsec;
using Unclazz.Parsec.CharClasses;
using static Unclazz.Parsec.Parser;

namespace Example.Unclazz.Parcec
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = new JsonExprParser();
            Console.WriteLine("parsed = {0}", (p).Parse(args[0]));
        }
    }

    sealed class JsonExprParser : Parser<IJsonObject>
    {
        readonly static Parser<IJsonObject> _null = new JsonNullParser();
        readonly static Parser<IJsonObject> _boolean = new JsonBooleanParser();
        readonly static Parser<IJsonObject> _number = new JsonNumberParser();
        readonly static Parser<IJsonObject> _string = new JsonStringParser();
        readonly static Parser<string> space = CharsWhileIn(" \r\n", min: 0);
        readonly static Parser<string> _comma = space & Char(',');

        readonly Parser<IJsonObject> _exprNoRecur = _null | _boolean | _string | _number;

        Parser<IJsonObject> Array()
        {
            return (space.Then(Char('['))
                .CastThen((Lazy(Object) | Lazy(Array) | _string | _number | _null | _boolean)
                .RepeatMin(0, sep: _comma))
                .Relay(space & Char(']')).Map(a => JsonObject.Of(a)));
        }
        Parser<IJsonObject> Object()
        {
            return Keyword("{}").Map(a => JsonObject.OfNull());
        }

        public override ParseResult<IJsonObject> Parse(ParserInput input)
        {
            return Array().Parse(input);
        }
    }

    sealed class JsonNullParser : Parser<IJsonObject>
    {
        readonly static Parser<string> space = CharsWhileIn(" \r\n", min: 0);
        readonly static Parser<IJsonObject> _null = 
            (space & Keyword("null", cutIndex: 1))
            .Map(a => JsonObject.OfNull());

        void Log(string message)
        {
            Console.WriteLine(">>> null: {0}", message);
        }

        public override ParseResult<IJsonObject> Parse(ParserInput input)
        {
            var r = _null.Log(Log).Parse(input);
            return r;
        }
    }

    sealed class JsonBooleanParser : Parser<IJsonObject>
    {
        readonly static Parser<string> space = CharsWhileIn(" \r\n", min: 0);
        readonly static Parser<IJsonObject> _boolean = 
            (space & StringIn("true", "false").Capture())
            .Map(a => JsonObject.Of(a == "true"));

        void Log(string message)
        {
            Console.WriteLine(">>> boolean: {0}", message);
        }

        public override ParseResult<IJsonObject> Parse(ParserInput input)
        {
            return _boolean.Log(Log).Parse(input);
        }
    }

    sealed class JsonNumberParser : Parser<IJsonObject>
    {
        readonly static Parser<string> space = CharsWhileIn(" \r\n", min: 0);
        readonly static Parser<string> sign = CharIn("+-").OrNot();
        readonly static Parser<string> digits = CharsWhileIn("0123456789");
        readonly static Parser<string> integral = Char('0') | (CharBetween('1', '9') & digits);
        readonly static Parser<string> fractional = Char('.') & digits;
        readonly static Parser<string> exponent = CharIn("eE") & (sign) & (digits);
        readonly static Parser<IJsonObject> number = (space & (sign & integral & fractional.OrNot() &
            exponent.OrNot()).Capture()).Map(double.Parse).Map(JsonObject.Of);

        void Log(string message)
        {
            Console.WriteLine(">>> number: {0}", message);
        }

        public override ParseResult<IJsonObject> Parse(ParserInput input)
        {
            return number.Log(Log).Parse(input);
        }
    }

    sealed class JsonStringParser : Parser<IJsonObject>
    {
        readonly static Parser<string> space = CharsWhileIn(" \r\n", min: 0);
        readonly static Parser<string> strChars = CharsWhileIn(!CharClass.AnyOf("\"\\"));
        readonly static Parser<string> hexDigit = CharBetween('0', '9') | CharBetween('a', 'f') | CharBetween('A', 'Z');
        readonly static Parser<string> unicodeEscape = Char('u') & hexDigit & hexDigit & hexDigit & hexDigit;
        readonly static Parser<string> escape = Char('\\') & (CharIn("\"/\\bfnrt") | unicodeEscape);
        readonly static Parser<IJsonObject> _string = (space & Char('"').Cut() & 
            (strChars | escape).RepeatMin(0).Capture() & Char('"')).Map(JsonObject.Of);

        void Log(string message)
        {
            Console.WriteLine(">>> string: {0}", message);
        }

        public override ParseResult<IJsonObject> Parse(ParserInput input)
        {
            return _string.Log(Log).Parse(input);
        }
    }
}
