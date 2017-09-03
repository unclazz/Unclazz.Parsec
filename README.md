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
`Parser`|パーサーを表す抽象クラスです。このパーサーはパース成否に関わらず決して値のキャプチャを行いません。`Char(char)`や`KeywordIn(params string[])`など多くのパーサーがこのクラスの派生型です。
`Parsers`|定義済みパーサーの静的ファクトリーメソッドを提供するユーティリティです。`using static`ディレクティブの使用をおすすめします。
`Reader`|パーサーの入力データとなるクラスです。このクラスが公開する静的ファクトリーメソッドを使い各種データ型からインスタンスを生成します。
`Result<T>`|`Parser<T>`のパース結果を表す構造体です。`Successful`プロパティでパース成否を、`Capture`でキャプチャ結果を、`Message`でパース失敗の理由を示すメッセージを取得できます。
`Result`|`Parser`のパース結果を表す構造体です。`Successful`プロパティでパース成否を、`Message`でパース失敗の理由を示すメッセージを取得できます。
`Optional`|Java 8 における同名クラスやScalaにおける`Option`と同じ役割を持つ構造体です。`ParseResult<T>.Capture`はこの構造体のインスタンスを返します。
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
    protected override ResultCore<double> DoParse(Reader input)
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
var c1 = r.Capture; // returns Optional(-123.456), and c1.Capture returns -123.456.

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
    protected override ResultCore<IJsonObject> DoParse(Reader input)
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
        _boolean = KeywordIn("false", "true").Capture().Map(a => JsonObject.Of(a == "true"));
    }
    protected override ResultCore<IJsonObject> DoParse(Reader input)
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
    protected override ResultCore<IJsonObject> DoParse(Reader input)
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
    protected override ResultCore<IJsonObject> DoParse(Reader input)
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

    protected override ResultCore<IJsonObject> DoParse(Reader input)
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

`Parser<T>`で宣言されている主なメンバー：

シグネチャ|戻り値|説明
---|---|---
`Parse(Reader)`|`Result<T>`|パース処理を実行する具象メソッド。
`DoParse(Reader)`|`ResultCore<T>`|`Parse(...)`から呼び出される抽象メソッド。パーサー実装者はこのメソッドを実装しなくてはならない。
`Success(T)`|`ResultCore<T>`|`DoParse(...)`実装コードのためのヘルパーメソッド。
`Failure(string)`|`ResultCore<T>`|同上。
`Char(char)`|`Parser`|ファクトリーメソッド。
`Keyword(string)`|`Parser`|ファクトリーメソッド。

まずは`"hello"`というキーワードを読みとるパーサーを作ってみます：

```cs
class HelloParser : Parser<string>
{
	Parser<string> _hello;
	protected override ResultCore<string> DoParse(Reader input)
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

戻り値型の宣言には `ResultCore<string>` 構造体が使用されています。
`ResultCore<T>`は`Result<T>`からパース開始・終了の文字位置を示すプロパティを除去したものです。
`Parser<T>.Parse(...)`メソッドは、派生クラスで実装された`DoParse(...)`が返した`ResultCore<T>`に、
パース前後の文字位置情報を付与した`Result<T>`を呼び出し元に返します。

注意すべき点は、各種ファクトリーメソッドが返すパーサーはそのほとんどが
値のキャプチャを行わないパーサー `Parser`（型パラメータがない）のインスタンスであるということです。
これらのパーサーは例えば上記の例のように特定のキーワードをパースするものであったり、
特定の文字クラスに属する文字のシーケンスをパースするものであったりしますが、
いずれにせよパース成否に関わらず値のキャプチャを行いません。
パース結果を示す構造体`Result`（これにも型パラメータがない）はパースが成功したのか失敗したのかを示すだけです。

これらのパーサーが読み取った結果をキャプチャし、読み取られた値にアクセスしたい場合、
`Capture()`を呼び出して`string`型の値をキャプチャするパーサーに変換するか、
`Map<T>(Func<string, T>)`を呼び出して任意の型をキャプチャするパーサーに変換するかします。
変換後のパーサーはパース結果として`Result<T>`を返します。
`ParseResult<T>.Capture`プロパティはキャプチャした値を返します。

ではこのパーサーを実行してみましょう：

```cs
class Program
{
    static void Main(string[] args)
    {
        new HelloParser().Parse(args[0])
            .IfSuccessful(
                v => Console.WriteLine("result = {0}", v), 
                m => Console.WriteLine("error = {0}", m));
    }
}
```

`Result<T>`で宣言されている主なメンバー：

シグネチャ|戻り値|説明
---|---|---
`Successful { get; }`|`bool`|パースの成否を示すプロパティ。
`Capture { get; }`|`T`|キャプチャした値。パース失敗時は例外をスローする。
`Message { get; }`|`string`|パース失敗の理由を示すメッセージ。パース成功時は例外をスローする。
`Map<U>(Func<T,U>)`|`Reult<U>`|キャプチャした値に関数を適用する。パース失敗時は何もしない。
`IfSuccessful(Action<T>)`|`void`|キャプチャした値を引数にアクションを実行する。パース失敗時は何もしない。


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
	protected override ResultCore<string> DoParse(Reader input)
	{
		// ...ここに独自のパースロジック...
		// ...inputから文字を読み、何かしらのチェックや変換を行う...

		// パースの結果を呼び出し元に返す
		return Success(value);
	}
}
```

今更の説明ですが`Reader`は`System.IO.TextReader`を大幅に機能拡張したクラスです。
このクラスは`Mark/Reset/Unmark`という一連のメソッドで読み込みのUNDOをサポートし、
`Capture`メソッドで直近`Mark`した文字位置から現在の文字位置までの文字列の取得をサポートします。

独自のパーサーを実装する場合この`Reader`からいくばくかの文字を読み取って、
その内容に応じて何かしらのチェックや変換の処理を行うことになるでしょう。
その上でパース結果を`ResultCore<T>`構造体にラップして呼び出し元に返します。
この構造体のインスタンスを得るための手段として`Parser<T>`は`Success/Failure`という2つのメソッド
およびそのオーバーロードを提供しています。

独自のパーサーを実装する際にとくに注意すべき点として、以下の原則にしたがってください。
これらが守られない場合、パーサーコンビネーターのパーサーコンビネーターたる所以である「コンビネーション」が破綻します：

* `DoParse(...)`メソッドはいかなる場合も`null`を返してはなりません。
* またこのメソッドは原則として例外スローを行ってはなりません。
* 正常・異常を問わずこのメソッド内で起こったことはすべて`ResultCore<T>`を通じて呼び出し元に通知される必要があります。

### 順序

例えばあるキーワードのあとに別のあるキーワードが続くとか、
ある文字のあとにある文字クラスに属する文字の並びが続くとかのシーケンスを表現する（それらにマッチするパーサーを組み立てる）には、
`&`演算子もしくは`Parser<T>.Then(...)`メソッドのオーバーロードを使用します。
`&`は`Then(...)`のエイリアスとして機能します。

演算子オーバーロードを使用する例を見てみましょう：

```cs
Parser hello = Keyword("hello");
Parser helloWorld = hello & Keyword("world");
helloWorld.Parse("helloworld"); // => OK. Result.Successful is true.
helloWorld.Parse("hello world"); // => NG. Parse.Successful is false.

Parser hello_ = hello & CharIn("!?");
hello_.Parse("hello"); // => NG.
hello_.Parse("hello!"); // => OK.

Parser<string> helloCapture = hello.Capture();
Parser<Tuple<string,string>> bothCapture = helloCapture & helloCapture;
Parser<string> leftCapture = helloCapture & hello;
Parser<string> rightCapture = hello & helloCapture;
```

左右の被演算子となるパーサーの結果型の違いが、合成された新しいパーサーの結果型に影響している点が見てとれます。

* 両側の被演算子が`Parser`の場合、合成結果も`Parser`です。
* 片側の被演算子が`Parser`でもう片側が`Parser<T>`の場合、合成結果は`Parser<T>`です。
* 両側の被演算子が`Parser<T>`の場合、合成結果は`Parser<Tuple<T, T>>`です。

C#言語仕様の制約により[FastParseの場合](http://www.lihaoyi.com/fastparse/#Sequence)のように
多種多様な状況では`&`が使用できません。`Unclazz.Parsec`において`&`演算子を使用可能なのは、
左右の被演算子となるパーサーが同じ結果型を宣言している場合と、いずれか片方もしくは両方のパーサーが`Parser`である場合だけです。
つまり`Parser<T>.Then<T, U>(Parser<U>)`メソッドに対応する演算子オーバーロードは存在しません。

### 繰り返し

ある文字やキーワードの繰り返しを表現する（それらにマッチするパーサーを組み立てる）には、`Repeat(int, int, int, Parser)`を使用します。
このメソッドの引数はいずれもオプション引数となっています。使用例を見てみましょう：

```cs
var a = Char('a');
var comma = Char(',');

var min2Max4 = a.Repeat(min: 2, max: 3);
min2Max4.Parse("a___"); // => NG.
min2Max4.Parse("aa__"); // => OK.
min2Max4.Parse("aaaa"); // => OK. 3文字目まで読み、EOFに到達しない

var exactly2 = a.Repeat(exactly: 2);
exactly2.Parse("a___"); // => NG.
exactly2.Parse("aa__"); // => OK.
exactly2.Parse("aaaa"); // => OK. ぴったり2文字読み、EOFに到達しない

var zeroOrMore = a.Repeat(); // デフォルトで0回以上かつ上限なしの繰り返し
zeroOrMore.Parse("____"); // => OK. しかし1文字も前進しない
zeroOrMore.Parse("aaaa"); // => OK. EOFに到達する

var sepComma = a.Repeat(sep: comma); // セパレータ指定も可能
sepComma.Parse("a,a,a,b"); // => OK. 3つ目の'a'まで読む
```

上記の例ではいずれのパーサーも`Parser`の派生クラスをベースとするため結果型がなく、したがって`Repeat(...)`の戻り値型も`Parser`です。
一方で例えば`a.Capture().Repeat(...)`とした場合は`Parser<T>`の派生クラスをベースとするため結果型あり、`Repeat(...)`の戻り値型は`Parser<IList<string>>`となります。

### オプション

あるトークンが存在するケースとそうでないケースがあるという場合、`OrNot()`を使用します。
例によって`Parser.OrNot()`は`Parser`の派生クラスを返すため結果型なしですが、
`Parser<T>.OrNot()`は`Parser<T>`の派生クラスを返すため結果型ありです。
ただし後者の場合でも対象のトークンの存否は実際にパースしてみないと確定しないため`Parser<Optional<T>>`となります。
`Parse(...)`は`Result<Optional<T>>`を返します：

```cs
var ab = Char('a') & Char('b').Capture().OrNot();
var result0 = ab.Parse("abc"); // OK. result0.Capture.Value は "b".
var result1 = ab.Parse("acd"); // OK. result1.Capture.Value は例外スロー
```

`Optional<T>`はJava SE 8の同名クラスやScalaにおける`Option`に対応するものです。`Optional<T>`で宣言されている主なメンバー：

シグネチャ|戻り値|説明
---|---|---
`Present { get; }`|`bool`|値の有無を示すプロパティ。
`Value { get; }`|`T`|値を返すプロパティ。値が存在しない場合は例外をスローする。
`OrElse(T)`|`T`|値を返すメソッド。値が存在しない場合は引数で指定した値を返す。
`Map<U>(Func<T,U>)`|`Optional<U>`|値に関数を適用する。値が存在しない時は何もしない。
`IfPresent(Action<T>)`|`void`|値を引数にアクションを呼び出す。値が存在しない時は何もしない。

### いずれか1つ

いくつかの選択肢のうちいずれか1つに該当するという場合、
`|`演算子もしくは`Parser<T>.Or(...)`メソッドのオーバーロードを使用します。
`|`は`Or(...)`のエイリアスとして機能します：

```cs
var abcd = Char('a').Repeat() & (Char('b') | Char('c') | Char('d'));
var result0 = abcd.Parse("aaaaab"); // OK.
var result1 = abcd.Parse("aaaaae"); // NG.
```

### BOFとEOF

`Unclazz.Parsec`のコンビネーターAPIで構築したパーサーは、
入力となるテキストのうち`Reader`の現在の文字位置から条件にマッチする部分まで読み進めて、
そこでそれ以上は読み進めずに処理を終えます。

この通常の動作を変更し必ずデータソースの開始位置（BOF）にマッチするパーサーを構築するには`BeginningOfFile`を使用します。
また必ずデータソースの終了位置（EOF）にマッチするパーサーを構築するには`EndOfFile`を使用します。

```cs
var ab = ((Char('a') | BeginningOfFile) & Char('b')).Repeat() & EndOfFile;
var result0 = ab.Parse("abab"); // OK.
var result1 = ab.Parse("babab"); // OK.
var result2 = ab.Parse("abb"); // NG.
```

### 肯定先読みと否定先読み

`Lookahead(Parser)`を使用すると肯定先読みと否定先読みを実現できます。
引数で指定した元のパーサーがパース成功する状況でのみ`Lookahead(...)`が返すパーサーもパース成功します。
しかし元のパーサーと異なってこのパーサーは文字位置を前進させません。
もちろんキャプチャされる文字列にもこの先読み部分は反映されません。

したがって、次に示すように`&`演算子もしくは`Then(...)`メソッドや`!`演算子もしくは`Not(...)`と組わせて、
肯定先読みと否定先読みを行うパーサーを構築できます：

```cs
var helloSp = (Keyword("hello") & Lookahead(Char(' '))).Capture();
var result0 = helloSp.Parse("hello "); // OK. result0.Capture == "hello" (!= "hello ")
var result1 = helloSp.Parse("helloX"); // NG.

var helloNoSp = (Keyword("hello") & !Lookahead(Char(' '))).Capture();
var result2 = helloNoSp.Parse("hello "); // NG.
var result3 = helloNoSp.Parse("helloX"); // OK. result3.Capture == "hello" (!= "helloX")
```


