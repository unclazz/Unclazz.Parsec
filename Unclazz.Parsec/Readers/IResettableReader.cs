using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.Readers
{
    /// <summary>
    /// リセット機能を持つ<see cref="ITextReader"/>です。
    /// <para>
    /// あらかじめ<see cref="Mark"/>により文字位置を記録しておくことで、
    /// <see cref="ITextReader.Read"/>により文字位置を先に進めたあとで
    /// <see cref="Reset"/>により文字位置を元に戻すことができます。
    /// </para>
    /// <para>
    /// <see cref="Mark"/>を呼び出すたびにその時点での文字位置が記録されます。
    /// <see cref="Mark"/>を呼び出した後、例え文字位置が1文字も前進していない場合でも<see cref="Mark"/>を再度呼び出すと、
    /// その時の文字位置が独立した記録として残ります。
    /// </para>
    /// <para>
    /// <see cref="Unmark"/>を呼び出すと直近の<see cref="Mark"/>呼び出しで行われた文字位置の記録が削除されます。
    /// <see cref="Mark"/>により残された文字位置の記録すべてを削除するには、
    /// <see cref="Mark"/>と同じ回数だけ<see cref="Unmark"/>を呼び出します。
    /// </para>
    /// </summary>
    interface IResettableReader : ITextReader
    {
        /// <summary>
        /// 現在の文字位置に印をつけます。
        /// </summary>
        void Mark();
        /// <summary>
        /// <see cref="Mark"/>により設定された印をはずします。
        /// まだ<see cref="Mark"/>が呼び出された実績がない場合、このメソッドは何も行わず、例外もスローしません。
        /// </summary>
        void Unmark();
        /// <summary>
        /// あらかじめ印をつけておいた文字位置に戻ります。
        /// まだ<see cref="Mark"/>が呼び出された実績がない場合、このメソッドは何も行わず、例外もスローしません。
        /// </summary>
        /// <param name="unmark"><c>true</c>の場合リセット後に<see cref="Unmark"/>を行う</param>
        void Reset(bool unmark);
        /// <summary>
        /// 直近<see cref="Mark"/>した文字位置から現在の文字位置までのコンテンツを返します。
        /// </summary>
        /// <param name="unmark"><c>true</c>の場合コンテンツを返す前に<see cref="Unmark"/>を行う</param>
        /// <returns>キャプチャしたコンテンツ</returns>
        string Capture(bool unmark);
    }
}
