using System;
namespace Unclazz.Parsec
{
    /// <summary>
    /// パーサーによるパース結果の値として返すべきものがないことを表す特別なクラスです。
    /// <para>
    /// このクラスはインスタンス化不可能です。
    /// このクラスは抽象クラス<see cref="Parser"/>の宣言の中で使用されています。
    /// <see cref="Parser"/>から派生した具象クラスは値のキャプチャを一切行いません。
    /// メソッド<see cref="Parser.Parse(ParserInput)"/>はパースを行いその結果として<see cref="ParseResult{T}"/>を返しますが、
    /// パース結果の成否と関係なく、<see cref="ParseResult{T}.Capture"/>は必ず空のシーケンスになります。
    /// </para>
    /// </summary>
    public class Nil
    {
        Nil() { throw new InvalidOperationException(); }
    }
}
