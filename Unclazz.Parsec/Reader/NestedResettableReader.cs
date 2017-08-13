using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.Reader
{
    sealed class NestedResettableReader : AutoDispose, INestedResettableReader
    {
        internal NestedResettableReader(TextReader r) : this(new TextReaderProxy(r)) { }
        internal NestedResettableReader(ITextReader r)
        {
            if (r == null) throw new ArgumentNullException(nameof(r));
            var resettable = r as IResettableReader;
            Internal = resettable == null ? new ResettableReader(r) : resettable;
        }

        public CharacterPosition Position => Internal.Position;
        public bool EndOfFile => Internal.EndOfFile;
        protected override IDisposable Disposable => Internal;
        IResettableReader Internal { get; set; }

        public void Nest()
        {
            Internal = new ResettableReader(Internal);
        }
        public void Unnest()
        {
            var nested = Internal as ResettableReader;
            if (nested == null) throw new InvalidOperationException();
            Internal = nested.Unwrap();
        }

        public int Peek() => Internal.Peek();
        public int Read() => Internal.Read();
        public string ReadLine() => Internal.ReadLine();
        public void Mark() => Internal.Mark();
        public void Unmark() => Internal.Unmark();
        public void Reset() => Internal.Reset();
    }
}
