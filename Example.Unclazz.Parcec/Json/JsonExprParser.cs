using System;
using System.Collections.Generic;
using System.Linq;
using Unclazz.Commons.Json;
using Unclazz.Parsec;

namespace Example.Unclazz.Parcec
{
    sealed class JsonExprParser : Parser<IJsonObject>
    {
        static readonly Parser<IJsonObject> _null = new JsonNullParser();
        static readonly Parser<IJsonObject> _boolean = new JsonBooleanParser();
        static readonly Parser<IJsonObject> _number = new JsonNumberParser();
        static readonly Parser<IJsonObject> _string = new JsonStringParser();

        readonly Parser<IJsonObject> jsonExpr;
        readonly Parser<IJsonObject> _object;
        readonly Parser<IJsonObject> _array;

        public JsonExprParser()
        {
            Configure(c => c.SetNonSignificant(CharsWhileIn(" \r\n")));

            jsonExpr = Lazy<IJsonObject>(JsonExpr);

            var propPair = _string.Cut() & Char(':') & jsonExpr;
            var comma = Char(',');

            _array = (Char('[').Cut() & jsonExpr.Repeat(sep: comma) & Char(']')).Map(JsonObject.Of);
            _object = (Char('{').Cut() & propPair.Repeat(sep: comma) & Char('}')).Map(PairsToObject);
        }
        Parser<IJsonObject> JsonExpr()
        {
            return _object | (_array | (_string | (_boolean | (_null | _number))));
        }
        IJsonObject PairsToObject(IList<Tuple<IJsonObject,IJsonObject>> pairs)
        {
            return pairs.Aggregate(JsonObject.Builder(),
                (a0,a1) => a0.Append(a1.Item1.AsString(), a1.Item2),
                a0 => a0.Build());
        }

        protected override ParseResult<IJsonObject> DoParse(Reader input)
        {
            return jsonExpr.Parse(input);
        }
    }
}
