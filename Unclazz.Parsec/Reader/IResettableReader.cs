using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.Reader
{
    /// <summary>
    /// リセット機能を持つ<see cref="ITextReader"/>です。
    /// <para>
    /// あらかじめ<see cref="Mark"/>により文字位置に印をつけておくことで、
    /// <see cref="ITextReader.Read"/>により文字位置を先に進めたあとで
    /// <see cref="Reset"/>により文字位置を元に戻すことができます。
    /// </para>
    /// </summary>
    public interface IResettableReader : ITextReader
    {
        /// <summary>
        /// 現在の文字位置に印をつけます。
        /// </summary>
        void Mark();
        /// <summary>
        /// <see cref="Mark"/>により設定された印をはずします。
        /// まだ<see cref="Mark"/>が呼び出された実績がない場合、このメソッドは何も行わず、例外も発生しません。
        /// </summary>
        void Unmark();
        /// <summary>
        /// あらかじめ印をつけておいた文字位置に戻ります。
        /// このメソッドを呼び出しても印は外されないので、何度でも<see cref="Reset"/>を行うことができます。
        /// </summary>
        void Reset();
    }
}
