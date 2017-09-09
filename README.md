# `Unclazz.Parsec`のREADME

`Unclazz.Parsec`はC#によるパーサーコンビネーター・ライブラリです。
Scala言語で実装されたパーサーコンビネーター [FastParse](https://github.com/lihaoyi/fastparse)を参考にしつつ、
C#言語の機能もしくは制約にあわせたAPIを提供しています。

ライブラリが公開するAPIを使用してパーサーを組み立てる方法については
[Wikiページ](https://github.com/mizukyf/Unclazz.Parsec/wiki)を参照してください。

例えば次のコードは`"hello"`や`"hello hello"`という文字列をパースするパーサーです：

```cs
class Program : Parser<string>
{
    static void Main(string[] args)
    {
        // 何らかの入力データソースを用意
        var text = args.Aggregate((a, b) => a + ' ' + b);
        // 文字列、ファイル、その他ストリームからReaderオブジェクトを生成
        var input = Reader.From(text);
        // パースを行う
        var result = new Program().Parse(input);

        // 成功したら結果をSTDOUT、さもなくばメッセージをSTDERRに出力
        result.IfSuccessful(Console.WriteLine,　Console.Error.WriteLine);
    }

    readonly Parser _hello;
    readonly Parser<Seq<string>> _helloHello;
    readonly Parser _spEof;
    readonly Parser<string> _helloX;

    Program()
    {
        // "hello"というキーワードにマッチするパーサー（キャプチャなし）
        _hello = Keyword("hello");
        // キャプチャありに切替え、1回以上の繰返し、かつセパレータとして空白文字を指定
        _helloHello = _hello.Capture().Repeat(min:1, sep: WhileSpaceAndControls);
        // 空白文字とEOFにマッチするパーサー（キャプチャなし）
        _spEof = WhileSpaceAndControls & EndOfFile;
        // "hello"の繰返しをintに、その後さらにstringに変換、後続にはEOF
        _helloX = _helloHello.Map(a => a.Count).Map(a => string.Format("hello x {0}", a)) & _spEof;
    }

    protected override ResultCore<string> DoParse(Reader input)
    {
        return _helloX.Parse(input);
    }
}
```

これをビルドして実行してみます：

```bat
> example.exe hello
hello x 1

> example.exe hello hello hello 
hello x 3

> example.exe hello hello?
EOF expected but found '?'(63).
```
