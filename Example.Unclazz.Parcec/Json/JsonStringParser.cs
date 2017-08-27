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

            // まず'"'にマッチを試みる。
            // マッチに成功したら直近の |(Or)によるバックトラックは無効化。
            // その後JSON文字列を構成する文字として有効な文字の0回以上の繰り返しを読み取ってキャプチャ。
            // 再度'"'にマッチを試みる。
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
