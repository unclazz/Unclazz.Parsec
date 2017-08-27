using System;
using Unclazz.Commons.Json;
using Unclazz.Parsec;

namespace Example.Unclazz.Parcec
{
    sealed class JsonNumberParser : Parser<IJsonObject>
    {
        readonly Parser<IJsonObject> number;
        public JsonNumberParser()
        {
            var sign = CharIn("+-").OrNot();
            var digits = CharsWhileIn("0123456789", min: 0);
            var integral = Char('0') | (CharBetween('1', '9') & digits);
            var fractional = Char('.') & digits;
            var exponent = CharIn("eE") & (sign) & (digits);
            number = ((sign.OrNot() & integral & fractional.OrNot() & exponent.OrNot()).Capture())
                .Map(double.Parse).Map(JsonObject.Of);
        }
        protected override ParseResult<IJsonObject> DoParse(Reader input)
        {
            return number.Parse(input);
        }
    }
}
