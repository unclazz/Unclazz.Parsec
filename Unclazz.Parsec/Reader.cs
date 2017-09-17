using System;
using System.IO;
using System.Text;
using Unclazz.Parsec.Readers;

namespace Unclazz.Parsec
{
    /// <summary>
    /// 文字のシーケンスを読み取るためのクラスです。
    /// <para>
    /// あらかじめ<see cref="Mark"/>により文字位置を記録しておくことで、
    /// <see cref="Read"/>により文字位置を先に進めたあとで
    /// <see cref="Reset()"/>により文字位置を元に戻すことができます。
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
    /// <para>
    /// <see cref="Mark"/>を呼び出した後<see cref="Capture()"/>を呼び出すと、
    /// 2つのメソッド呼び出しの間に前進した文字位置の区間の文字列が取得できます。
    /// </para>
    /// </summary>
    public sealed class Reader : IDisposable
    {
        /// <summary>
        /// <see cref="TextReader"/>から<see cref="Reader"/>に暗黙のキャストを行います。
        /// </summary>
        /// <param name="reader">新しいリーダー</param>
        public static implicit operator Reader(TextReader reader)
        {
            return From(reader);
        }
        /// <summary>
        /// <see cref="string"/>から<see cref="Reader"/>に暗黙のキャストを行います。
        /// </summary>
        /// <param name="text">新しいリーダー</param>
        public static implicit operator Reader(string text)
        {
            return From(new StringReader(text));
        }
        /// <summary>
        /// <paramref name="text"/>をデータソースとする<see cref="Reader"/>を返します。
        /// </summary>
        /// <param name="text">データソース</param>
        /// <returns>新しいリーダー</returns>
        public static Reader From(string text)
        {
            return From(new StringReader(text ?? throw new ArgumentNullException(nameof(text))));
        }
        /// <summary>
        /// <paramref name="filepath"/>が指すファイルをデータソースとする<see cref="Reader"/>を返します。
        /// </summary>
        /// <param name="filepath">ファイルパス</param>
        /// <param name="enc">エンコーディング</param>
        /// <returns>新しいリーダー</returns>
        public static Reader From(string filepath, Encoding enc)
        {
            return From(filepath ?? throw new ArgumentNullException(nameof(filepath)), enc);
        }
        /// <summary>
        /// <paramref name="stream"/>をデータソースとする<see cref="Reader"/>を返します。
        /// </summary>
        /// <param name="stream">ストリーム</param>
        /// <param name="enc">エンコーディング</param>
        /// <returns>新しいリーダー</returns>
        public static Reader From(Stream stream, Encoding enc)
        {
            return From(new StreamReader(
                stream ?? throw new ArgumentNullException(nameof(stream)),
                enc ?? throw new ArgumentNullException(nameof(enc))));
        }
        /// <summary>
        /// <paramref name="reader"/>をデータソースとする<see cref="Reader"/>を返します。
        /// </summary>
        /// <param name="reader">リーダー</param>
        /// <returns>新しいリーダー</returns>
        public static Reader From(TextReader reader)
        {
            return new Reader(reader ?? throw new ArgumentNullException(nameof(reader)));
        }

        Reader(TextReader reader)
        {
            _inner = new ResettableReader(reader);
        }
        /// <summary>
        /// デストラクタです。
        /// </summary>
        ~Reader()
        {
            Dispose(false);
        }

        bool _disposed;
        readonly IResettableReader _inner;

        /// <summary>
        /// 現在の文字位置です。
        /// <see cref="Read"/>のたびにインクリメントされます。
        /// </summary>
        public CharPosition Position => _inner.Position;
        /// <summary>
        /// データソースの終端（EOF）まで到達している場合<c>true</c>。
        /// </summary>
        public bool EndOfFile => _inner.EndOfFile;
        /// <summary>
        /// 直近<see cref="Mark"/>した文字位置から現在の文字位置までのコンテンツを返します。
        /// </summary>
        /// <returns>キャプチャしたコンテンツ</returns>
        public string Capture() => _inner.Capture(false);
        /// <summary>
        /// 直近<see cref="Mark"/>した文字位置から現在の文字位置までのコンテンツを返します。
        /// </summary>
        /// <param name="unmark"><c>true</c>の場合コンテンツを返す前に<see cref="Unmark"/>を行う</param>
        /// <returns>キャプチャしたコンテンツ</returns>
        public string Capture(bool unmark) => _inner.Capture(unmark);

        /// <summary>
        /// 現在の文字位置に印をつけます。
        /// </summary>
        public void Mark() => _inner.Mark();
        /// <summary>
        /// データソースから現在の文字位置の文字を読み取って返します。
        /// <para>
        /// <see cref="Read"/>と異なり文字位置を進めることはありません。
        /// </para>
        /// </summary>
        /// <returns></returns>
        public int Peek() => _inner.Peek();
        /// <summary>
        /// データソースから現在の文字位置の文字を読み取って返します。
        /// <para>
        /// データソースから読み取った文字を呼び出し元に返す前に、文字位置を次に進めます。
        /// </para>
        /// </summary>
        /// <returns></returns>
        public int Read() => _inner.Read();
        /// <summary>
        /// 現在の文字位置から行末までの文字を読み取り文字列として返します。
        /// <para><see cref="Read"/>により文字位置が行頭から移動している場合、
        /// このメソッドが返す文字列は当該行の文字すべてを含む文字列ではなくなる点に注意してください。
        /// 文字位置がデータソースの末尾（EOF）に到達している場合、このメソッドは<c>null</c>を返します。</para>
        /// </summary>
        /// <returns></returns>
        public string ReadLine() => _inner.ReadLine();
        /// <summary>
        /// 現在の文字位置からデータソースの末尾（EOF）までの文字を読み取り文字列として返します。
        /// <para>文字位置がデータソースの末尾（EOF）に到達している場合、このメソッドは<c>null</c>を返します。</para>
        /// </summary>
        /// <returns></returns>
        public string ReadToEnd() => _inner.ReadToEnd();
        /// <summary>
        /// あらかじめ印をつけておいた文字位置に戻ります。
        /// まだ<see cref="Mark"/>が呼び出された実績がない場合、このメソッドは何も行わず、例外もスローしません。
        /// </summary>
        public void Reset() => _inner.Reset(false);
        /// <summary>
        /// あらかじめ印をつけておいた文字位置に戻ります。
        /// まだ<see cref="Mark"/>が呼び出された実績がない場合、このメソッドは何も行わず、例外もスローしません。
        /// </summary>
        /// <param name="unmark"><c>true</c>の場合リセット後に<see cref="Unmark"/>を行う</param>
        public void Reset(bool unmark) => _inner.Reset(unmark);
        /// <summary>
        /// <see cref="Mark"/>により設定された印をはずします。
        /// まだ<see cref="Mark"/>が呼び出された実績がない場合、このメソッドは何も行わず、例外もスローしません。
        /// </summary>
        public void Unmark() => _inner.Unmark();
        /// <summary>
        /// アンマネージ・リソースを解放します。
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _inner.Dispose();
            }
            _disposed = true;
        }
        /// <summary>
        /// リーダーからコンテキストを作成します。
        /// </summary>
        /// <returns></returns>
        public Context ToContext()
        {
            return new Context(this);
        }
        /// <summary>
        /// リーダーからコンテキストを作成し、コンフィギュレーションを変更します。
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        public Context ToContext(Action<ContextConfigurer> act)
        {
            return ToContext().Configure(act ?? throw new ArgumentNullException(nameof(act)));
        }
    }
}
