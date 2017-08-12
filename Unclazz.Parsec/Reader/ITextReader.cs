using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.Reader
{
    /// <summary>
    /// 一連の文字を読み取ることができるリーダーを表します。
    /// このインターフェースは<see cref="System.IO.TextReader"/>のインターフェースとしての再定義を行うものです。
    /// </summary>
    public interface ITextReader : IDisposable
    {
        /// <summary>
        /// データソースから現在の文字位置の文字を読み取って返します。
        /// <see cref="Read"/>と異なり文字位置を進めることはありません。
        /// </summary>
        /// <returns>文字</returns>
        int Peek();
        /// <summary>
        /// データソースから現在の文字位置の文字を読み取って返します。
        /// データソースから読み取った文字を呼び出し元に返す前に、文字位置を次に進めます。
        /// </summary>
        /// <returns>文字</returns>
        int Read();
        /// <summary>
        /// 現在の文字位置です。
        /// <see cref="Read"/>のたびにインクリメントされます。
        /// </summary>
        CharacterPosition Position { get; }
        /// <summary>
        /// データソースの終端（EOF）まで到達している場合<c>true</c>。
        /// </summary>
        bool EndOfFile { get; }
        /// <summary>
        /// 現在の文字位置から1行文の文字列を読み取って返します。
        /// </summary>
        /// <returns>文字列</returns>
        string ReadLine();
    }
}
