using Unclazz.Commons.Json;
using Unclazz.Parsec;

namespace Example.Unclazz.Parcec
{
    sealed class JsonNullParser : Parser<IJsonObject>
    {
        readonly Parser<IJsonObject> _null;
        public JsonNullParser()
        {
            // キーワード"null"にマッチ
            // 1文字目（'n'）にマッチしたら直近の |(Or) によるバックトラックを無効化
            // "null"マッチ成功後、読み取り結果のキャプチャはせず、Yieldパーサーで直接値を産生
            _null = Keyword("null", cutIndex: 1) & Yield(JsonObject.OfNull());
        }
        protected override ParseResult<IJsonObject> DoParse(Reader input)
        {
            return _null.Parse(input);
        }
    }
}
