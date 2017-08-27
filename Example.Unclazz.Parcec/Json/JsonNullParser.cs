using Unclazz.Commons.Json;
using Unclazz.Parsec;

namespace Example.Unclazz.Parcec
{
    sealed class JsonNullParser : Parser<IJsonObject>
    {
        readonly Parser<IJsonObject> _null;
        public JsonNullParser()
        {
            _null = Keyword("null", cutIndex: 1) & Yield(JsonObject.OfNull());
        }
        protected override ParseResult<IJsonObject> DoParse(Reader input)
        {
            return _null.Parse(input);
        }
    }
}
