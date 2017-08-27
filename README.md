# `Unclazz.Parsec`のREADME

C#によるパーサーコンビネーター・ライブラリーです。
Scala言語で実装されたパーサーコンビネーター [FastParse](https://github.com/lihaoyi/fastparse)を参考にしつつ、
C#言語の機能もしくは制約にあわせて構築されたAPIを公開しています。

`Example.Unclazz.Parsec`配下に若干のサンプルコードが用意されています。例えば次のコードは浮動小数点数のパーサーの実装例です：

```cs
sealed class NumberParser : Parser<double>
{
    readonly Parser sign;
    readonly Parser digits;
    readonly Parser integral;
    readonly Parser fractional;
    readonly Parser exponent;
    readonly Parser<double> number;

    public NumberParser()
    {
        sign = CharIn("+-").OrNot();
        digits = CharsWhileIn("0123456789", min: 0);
        integral = Char('0') | (CharBetween('1', '9') & digits);
        fractional = Char('.') & digits;
        exponent = CharIn("eE") & (sign) & (digits);
        number = ((sign.OrNot() & integral & fractional.OrNot() & exponent.OrNot()).Capture()).Map(double.Parse);
    }

    protected override ParseResult<double> DoParse(Reader input)
    {
        return number.Parse(input);
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

