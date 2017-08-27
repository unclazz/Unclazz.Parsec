using System.Text.RegularExpressions;
using Unclazz.Commons.Json;
using Unclazz.Parsec;
using Unclazz.Parsec.CharClasses;

namespace Example.Unclazz.Parcec
{
    sealed class JsonStringParser : Parser<IJsonObject>
    {
        readonly Parser<IJsonObject> _string;
        public JsonStringParser()
        {
            var quote = Char('"');
            var hexDigits = CharIn(CharClass.Between('0', '9') | CharClass.Between('a', 'f') | CharClass.Between('A', 'F'));
            var unicodeEscape = Char('u').Then(hexDigits.Repeat(exactly: 4));
            var escape = Char('\\') & (CharIn("\"/\\bfnrt") | unicodeEscape);
            var stringChars = CharIn(!CharClass.AnyOf("\"\\"));
            _string = (quote.Cut() & (stringChars | escape).Repeat().Capture() & quote)
                .Map(Unescape).Map(JsonObject.Of);
        }
        string Unescape(string escaped)
        {
            return Regex.Unescape(escaped);
        }
        protected override ParseResult<IJsonObject> DoParse(Reader input)
        {
            return _string.Parse(input);
        }
    }
}
