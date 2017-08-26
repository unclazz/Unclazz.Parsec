using System;
namespace Unclazz.Parsec
{
    /// <summary>
    /// パーサーによるパース結果の値として返すべきものがないことを表す特別なクラスです。
    /// <para>
    /// このクラスはインスタンス化不可能です。
    /// このクラスは抽象クラス<see cref="NilParser"/>の宣言の中で使用されています。
    /// <see cref="NilParser"/>から派生した具象クラスは値のキャプチャを一切行いません。
    /// パースを行いその結果として<see cref="ParseResult{T}"/>を返しますが、
    /// パース結果の成否と関係なく、<see cref="ParseResult{T}.Capture"/>が参照する<see cref="Optional{T}"/>は決して値を持ちません。
    /// </para>
    /// </summary>
    public class Nil
    {
        Nil() { throw new InvalidOperationException(); }
    }
}
