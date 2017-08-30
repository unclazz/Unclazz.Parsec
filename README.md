# `Unclazz.Parsec`のREADME

C#によるパーサーコンビネーター・ライブラリーです。
Scala言語で実装されたパーサーコンビネーター [FastParse](https://github.com/lihaoyi/fastparse)を参考にしつつ、
C#言語の機能もしくは制約にあわせてAPIを構築しています。

## 主要コンポーネント

次に示すのはこのライブラリの主要コンポーネントとなるクラスもしくは構造体です。
これらはいずれも`Unclazz.Parsec`名前空間に属しています。

ライブラリのユーザ開発者は抽象クラス`Parser<T>`・`Parser`の派生クラスを実装するか、
`Parsers`ユーティリティが提供する静的ファクトリーメソッドを通じて得られる定義済みパーサーを組み合わせてパーサーを実装します。

クラス|説明
---|---
`Parser<T>`|パーサーを表す抽象クラスです。型パラメータはパース結果の型を表します。
`Parser`|`Parser<T>`の派生型の抽象クラスです。このパーサーのパース結果型は`Nil`です。ここから想像がつくかもしれませんが、このパーサーはパース成否に関わらず決して値のキャプチャを行いません。`Char(char)`や`StringIn(string[])`など多くのパーサーがこのクラスの派生型です。
`Parsers`|定義済みパーサーの静的ファクトリーメソッドを提供するユーティリティです。`using static`ディレクティブの使用をおすすめします。
`Reader`|パーサーの入力データとなるクラスです。このクラスが公開する静的ファクトリーメソッドを使い各種データ型からインスタンスを生成します。
`ParseResult<T>`|パース結果を表す構造体です。`Successful`プロパティでパース成否を、`Capture`でキャプチャ結果を取得できます。
`ParseResult`|`ParseResult<T>`のためのユーティリティです。
`Optional`|Java 8 における同名クラスやScalaにおける`Option`と同じ役割を持つ構造体です。`ParseResult<T>.Capture`はこの構造体のインスタンスを返します。
`Nil`|`Parser`派生型のパーサーの結果型の宣言に使用されているクラスです。このクラスはインスタンス化できません。
`CharClass`|文字クラスを表す抽象クラスです。`CharIn(CharClass)`などのファクトリーメソッドの引数として利用します。`CharClass`が公開する静的ファクトリーメソッドを通じて派生クラスのインスタンスを得られます。

## パーサー実装例

`Example.Unclazz.Parsec`配下に若干のサンプルコードが用意されています。
これらのコードもまた [FastParse](https://github.com/lihaoyi/fastparse) のドキュメンテーションを参考にしてつくられたものです。

### 浮動小数点数

例えば次のコードは浮動小数点数のパーサーの実装例です：

```cs
sealed class NumberParser : Parser<double>
{
    readonly Parser<double> _number;
    public NumberParser()
    {
        Parser sign = CharIn("+-").OrNot();
        Parser digits = CharsWhileIn("0123456789", min: 0);
        Parser integral = Char('0') | (CharBetween('1', '9') & digits);
        Parser fractional = Char('.') & digits;
        Parser exponent = CharIn("eE") & sign & digits;
        // 始まりの符号はあるかもしれない（ないかもしれない）
        // 整数は必須
        // 小数点以下はあるかもしれない（ないかもしれない）
        // 指数はあるかもしれない（ないかもしれない）
        // 以上の文字列をキャプチャし、浮動小数点数に変換
        Parser<string> raw = (sign & integral & fractional.OrNot() & exponent.OrNot()).Capture();
        _number = raw.Map(double.Parse);
    }
    protected override ParseResult<double> DoParse(Reader input)
    {
        return _number.Parse(input);
    }
}
```

こうして作成したパーサーは次のようにして使用します：

```cs
var p = new NumberParser();

var r1 = p.Parse("-123.456");
var s1 = r.Successful; // returns true.
var c1 = r.Capture; // returns Optional(-123.456), and c1.Value returns -123.456.

var r2 = p.Parse("hello");
var s2 = r.Successful; // returns false.
var c2 = r.Capture; // throws InvalidOperationException.
```

### JSON

次に示すのはJSONパーサーの実装例です。パースをしながら順次JSONオブジェクトを構築していきます。
JSONのデータ型を表すC#言語におけるオブジェクトとそのユーティリティとして
[Unclazz.Commons.Json](https://github.com/unclazz/Unclazz.Commons.Json)のインターフェースを利用しています
（実際にはこのライブラリは自前のパーサーを持っているので恐るべき車輪の再発明ということになります）。

まずは`null`・`true`・`false`のリテラルです：

```cs
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
```

続いて、先程も登場した数値リテラルのパーサーです。ただし今回はパース結果の型が`IJsonObject`になっています：

```cs
sealed class JsonNumberParser : Parser<IJsonObject>
{
    readonly Parser<IJsonObject> _number;
    public JsonNumberParser()
    {
        var sign = CharIn("+-").OrNot();
        var digits = CharsWhileIn("0123456789", min: 0);
        var integral = Char('0') | (CharBetween('1', '9') & digits);
        var fractional = Char('.') & digits;
        var exponent = CharIn("eE") & sign & digits;

        // 始まりの符号はあるかもしれない（ないかもしれない）
        // 整数は必須
        // 小数点以下はあるかもしれない（ないかもしれない）
        // 指数はあるかもしれない（ないかもしれない）
        // 以上の文字列をキャプチャし、浮動小数点数に変換
        var raw = (sign & integral & fractional.OrNot() & exponent.OrNot()).Capture();
        _number = raw.Map(double.Parse).Map(JsonObject.Of);
    }
    protected override ParseResult<IJsonObject> DoParse(Reader input)
    {
        return _number.Parse(input);
    }
}
```

段々と複雑になってきますが、次は`String`型のリテラルを読み取るパーサーです
（エスケープされた文字の逆エスケープに`Regex.Unescape`を使用しているのは我ながらちょっとどうなのかと思います）：

```cs
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
```

最後に`Array`型と`Object`型、そしてここまで構築してきた他のデータ型を解析するパーサーです。
`Array`型と`Object`型のリテラルはその要素として自身を含むJSONオブジェクト表現を内包しています。
このため1つのパーサーの中で2つのデータ型のリテラルとそれを含むJSONオブジェクト表現、それぞれのパーサーを組み立てています：

```cs
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
        // パーサーのコンフィギュレーションを変更。
        // トークンに先行する空白文字とCRLFは無意味な文字として自動スキップさせる設定を行う。
        // ※Cofigure(...)による設定はこのパーサーや、このパーサーが派生クラス実装者に公開している
        // 各種ファクトリーメソッドから得られる定義済みパーサーのインスタンスに引き継がれる。
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

    protected override ParseResult<IJsonObject> DoParse(Reader input)
    {
        return jsonExpr.Parse(input);
    }
}
```

## パーサーを組み立てる

独自のパーサーの組み立てのために取りうる方式として2つの選択肢があります。
このうちより自由度の高い方式は前者ですので、以降この方式を中心に説明を行います：

* `Parser<T>`抽象クラスの派生クラスを実装する
* `Parsers`ユーティリティが提供する静的ファクトリーメソッドを使用する

まずは`"hello"`というキーワードを読みとるパーサーを作ってみます：

```cs
class HelloParser : Parser<string>
{
	Parser<string> _hello;
	protected override ParseResult<string> DoParse(Reader input)
	{
		return (_hello ?? (_hello = Keyword("hello").Capture())).Parse(input);
	}
}
```

クラスやメソッドの宣言から、`Parser<T>`を継承・拡張する`HelloParser`は結果型として`string`を宣言するパーサーであることがわかります。
このクラスはベースクラスで抽象的に宣言されたメソッド`DoParse(...)`を実装しています。
その内部ではベースクラスで宣言・実装されたファクトリーメソッド`Keyword(string)`を使用して
`"hello"`というキーワードにマッチするパーサーを生成しています。
`Keyword`以外にも多くの`protected`なファクトリーメソッドが提供されています。

注意すべき点は、各種ファクトリーメソッドが返すパーサーは値のキャプチャを行わないパーサー
`Parser`（型パラメータがない）のインスタンスであるということです。
これらのパーサーは例えば上記の例のように特定のキーワードをパースするものであったり、
特定の文字クラスに属する文字のシーケンスをパースするものであったりしますが、
いずれにせよパース成否に関わらず値のキャプチャを行いません。
パース結果を示す構造体`ParseResult<Nil>`から分かるのはパースが成功したのか失敗したのかだけです。

これらのパーサーが読み取った結果をキャプチャし、読み取られた値にアクセスしたい場合、
`Parser.Capture()`を呼び出して`string`型の値をキャプチャするパーサーに変換します。
変換後のパーサーはパース結果として`ParseResult<string>`を返します。
`ParseResult<string>.Capture`プロパティはキャプチャした値を格納するコンテナ`Optional<string>`を返します。

ではこのパーサーを実行してみましょう：

```cs
class Program
{
    static void Main(string[] args)
    {
        new HelloParser().Parse(args[0])
            .IfSuccessful(
                c => Console.WriteLine("result = {0}", c.Value), 
                m => Console.WriteLine("error = {0}", m));
    }
}
```

このコードをビルドして、生成された*.exeを実行してみます：

```bat
path\to\exe> example.exe hello
result = hello

path\to\exe> example.exe hallo
error = expected 'e'(101) but found 'a'(97) at index 1 in "hello"
```

ところで先程の例ではすべてをレディメイドのパーサーにより行っています。
しかしもちろん完全に独自のロジックでパースを行うパーサーを組み立てることもできます。
そのような場合は次のようにします：

```cs
class CustomParser : Parser<string>
{
	protected override ParseResult<string> DoParse(Reader input)
	{
		// パース開始時の文字位置を記録
		var pos = input.Position;

		// ...ここに独自のパースロジック...
		// ...inputから文字を読み、何かしらのチェックや変換を行う...

		// パースの結果を構造体にくるんで呼び出し元に返す
		return Success(pos, capture: new Capture<string>(value));
	}
}
```

今更の説明ですが`Reader`は`System.IO.TextReader`を大幅に機能拡張したクラスです。
このクラスは`Mark/Reset/Unmark`という一連のメソッドで読み込みのUNDOをサポートし、
`Capture`メソッドで直近`Mark`した文字位置から現在の文字位置までの文字列の取得をサポートします。

独自のパーサーを実装する場合この`Reader`からいくばくかの文字を読み取って、
その内容に応じて何かしらのチェックや変換の処理を行うことになるでしょう。
その上でパース結果を`ParseResult<T>`構造体にラップして呼び出し元に返します。
この構造体のインスタンスを得るための手段として`Parser<T>`は`Success/Failure`という2つのメソッド
およびそのオーバーロードを提供しています。

独自のパーサーを実装する際にとくに注意すべき点として、以下の原則にしたがってください。
これらが守られない場合、パーサーコンビネーターのパーサーコンビネーターたる所以である「コンビネーション」が破綻します：

* `DoParse(...)`メソッドはいかなる場合も`null`を返してはなりません。
* またこのメソッドは原則として例外スローを行ってはなりません。
* 正常・異常を問わずこのメソッド内で起こったことはすべて`ParseResult<T>`を通じて呼び出し元に通知される必要があります。

### シーケンス

例えばあるキーワードのあとに別のあるキーワードが続くとか、
ある文字のあとにある文字クラスに属する文字の並びが続くとかのシーケンスを表現するには、
`&`演算子もしくは`Parser<T>.Then()`メソッドのオーバーロードを使用します。

演算子オーバーロードを使用する例を見てみましょう：

```cs
Parser helloWorld = Keyword("hello") & Keyword("world");
helloWorld.Parse("helloworld"); // => OK. ParseResult<Nil>.Successful is true.
helloWorld.Parse("hello world"); // => NG. ParseResult<Nil>.Successful is false.

Parser hello_ = Keyword("hello") & CharIn('!', '?');
hello_.Parse("hello"); // => NG.
hello_.Parse("hello!"); // => OK.

Parser<Tuple<string, string>> bothCapture = Keyword("hello").Capture() & Keyword("world").Capture();
Parser<string> leftCapture = Keyword("hello").Capture() & Keyword("world");
Parser<string> rightCapture = Keyword("hello") & Keyword("world").Capture();
```

左右の被演算子となるパーサーの結果型の違いが、合成された新しいパーサーの結果型に影響している点が見てとれます。

* 両側の被演算子が`Parser`の場合、合成結果も`Parser`です。
* 片側の被演算子が`Parser`でもう片側が`Parser<T>`の場合、合成結果も`Parser<T>`です。
* 片側の被演算子が`Parser<T>`でもう片側が`Parser<U>`の場合、合成結果は`Parser<Tuple<T, U>>`です。

C#言語仕様の制約により[FastParseの場合](http://www.lihaoyi.com/fastparse/#Sequence)のように
多種多様な状況では`&`が使用できません。`Unclazz.Parsec`において`&`演算子を使用可能なのは、
左右の被演算子となるパーサーが同じ結果型を宣言している場合と、いずれか片方もしくは両方のパーサーが`Parser`である場合だけです。
つまり`Parser<T>.Then<T, U>(Parser<U>)`メソッドに対応する演算子オーバーロードは存在しません。
