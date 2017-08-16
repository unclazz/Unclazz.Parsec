using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.Readers
{
    /// <summary>
    /// <see cref="TextReader"/>を<see cref="ITextReader"/>として参照するためのラッパーです。
    /// </summary>
    sealed class TextReaderProxy : AbstractTextReader
    {
        internal TextReaderProxy(TextReader r)
        {
            Principal = r ?? throw new ArgumentNullException(nameof(r));
        }

        /// <summary>
        /// ラップされている<see cref="TextReader"/>への参照です。
        /// </summary>
        TextReader Principal { get; }
        protected override IDisposable Disposable => Principal;

        public override int Peek() => Principal.Peek();
        public override int ReadSimply() => Principal.Read();
    }
}
