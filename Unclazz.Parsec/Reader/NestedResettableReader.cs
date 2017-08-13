using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.Reader
{
    sealed class NestedResettableReader : IResettableReader
    {
        IResettableReader _nested;
        bool _disposed;
        internal NestedResettableReader(IResettableReader r)
        {
            _nested = r ?? throw new ArgumentNullException(nameof(r));
        }
        ~NestedResettableReader()
        {
            Dispose(false);
        }

        public CharacterPosition Position => _nested.Position;

        public bool EndOfFile => _nested.EndOfFile;

        void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing && _nested != null) _nested.Dispose();
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Mark()
        {
            _nested.Mark();
        }

        public int Peek()
        {
            return _nested.Peek();
        }

        public int Read()
        {
            return _nested.Read();
        }

        public string ReadLine()
        {
            return _nested.ReadLine();
        }

        public void Reset()
        {
            _nested.Reset();
        }

        public void Unmark()
        {
            _nested.Unmark();
        }

        public IResettableReader Unnest()
        {
            var tmp = _nested;
            _nested = null;
            return tmp;
        }
    }
}
