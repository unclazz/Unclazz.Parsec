using System;
using Unclazz.Commons.Json;
using Unclazz.Parsec;

namespace Example.Unclazz.Parsec
{
    sealed class JsonNumberParser : Parser<IJsonObject>
    {
        readonly Parser<IJsonObject> _number;
        public JsonNumberParser()
        {
            var signOpt = CharIn("+-").OrNot();
            var digits = CharsWhileIn("0123456789", min: 0);
            var integral = Char('0') | (CharBetween('1', '9') & digits);
            var fractionalOpt = (Char('.') & digits).OrNot();
            var exponentOpt = (CharIn("eE") & signOpt & digits).OrNot();

            // 始まりの符号はあるかもしれない（ないかもしれない）
            // 整数は必須
            // 小数点以下はあるかもしれない（ないかもしれない）
            // 指数はあるかもしれない（ないかもしれない）
            // 以上の文字列をキャプチャし、浮動小数点数に変換
            var raw = (signOpt & integral & fractionalOpt & exponentOpt).Capture();
            _number = raw.Map(double.Parse).Map(JsonObject.Of);
        }
        protected override ResultCore<IJsonObject> DoParse(Reader src)
        {
            return _number.Parse(src);
        }
    }
}
