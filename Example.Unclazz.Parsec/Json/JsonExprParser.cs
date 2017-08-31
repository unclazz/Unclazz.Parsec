using System;
using System.Collections.Generic;
using System.Linq;
using Unclazz.Commons.Json;
using Unclazz.Parsec;

namespace Example.Unclazz.Parsec
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
            Configure(c => c.SetAutoSkip(true));

            // JSON表現はObject・Arrayの要素としても登場。
            // 結果、jsonExpr・_array・_objectは再帰的関係を持つ。
            // C#言語仕様により、同じスコープ内でも後続行（下方の行）で宣言される変数は
            // 先行行（上方の行）で宣言される式（ラムダ式含む）の中で参照できないから、
            // jsonExprの構築は別メソッド化し、Lazy(...)による遅延評価型のパーサーとして
            // インスタンス・フィールドにアサインしておく。
            jsonExpr = Lazy<IJsonObject>(JsonExpr);

            var propPair = _string.Cut() & Char(':') & jsonExpr;
            var comma = Char(',');

            // jsonExprを要素とするObject・Arrayのパーサーを組み立てる
            _array = (Char('[').Cut() & jsonExpr.Repeat(sep: comma) & Char(']')).Map(JsonObject.Of);
            _object = (Char('{').Cut() & propPair.Repeat(sep: comma) & Char('}')).Map(PairsToObject);
        }
        Parser<IJsonObject> JsonExpr()
        {
            // JSONオブジェクト表現はObject、Array、String、Boolean、Null、Numberのいずれかである
            return _object | (_array | (_string | (_boolean | (_null | _number))));
        }
        IJsonObject PairsToObject(IList<Tuple<IJsonObject,IJsonObject>> pairs)
        {
            return pairs.Aggregate(JsonObject.Builder(),
                (a0,a1) => a0.Append(a1.Item1.AsString(), a1.Item2),
                a0 => a0.Build());
        }

        protected override ResultCore<IJsonObject> DoParse(Reader input)
        {
            return jsonExpr.Parse(input);
        }
    }
}
