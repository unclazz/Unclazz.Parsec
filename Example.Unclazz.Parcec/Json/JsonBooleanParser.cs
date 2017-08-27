using Unclazz.Commons.Json;
using Unclazz.Parsec;

namespace Example.Unclazz.Parcec
{
    sealed class JsonBooleanParser : Parser<IJsonObject>
    {
        readonly Parser<IJsonObject> _boolean;
        public JsonBooleanParser()
        {
            // キーワード"false"もしくは"true"にマッチ
            // マッチした文字列をキャプチャして、それをマッパーにより真偽値に変換
            _boolean = StringIn("false", "true").Capture().Map(a => JsonObject.Of(a == "true"));
        }
        protected override ParseResult<IJsonObject> DoParse(Reader input)
        {
            return _boolean.Parse(input);
        }
    }
}
