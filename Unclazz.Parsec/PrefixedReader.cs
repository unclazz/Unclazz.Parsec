using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec
{
    sealed class PrefixedReader : AutoDispose, IDisposable
    {
        static readonly Queue<char> EmptyQueue = new Queue<char>();

        int _last;
        Queue<char> _prefix;
        readonly TextReader _inner;

        public int Position { get; private set; }
        public int LinePosition { get; private set; }
        public int ColumnPosition { get; private set; }
        public bool EndOfFile => Peek() == -1;
        protected override IDisposable Disposable => _inner;

        internal PrefixedReader(int p, int l, int c, Queue<char>  q, TextReader r)
        {
            if (p < 1) throw new ArgumentOutOfRangeException(nameof(p));
            Position = p;
            if (l < 1) throw new ArgumentOutOfRangeException(nameof(l));
            LinePosition = l;
            if (c < 1) throw new ArgumentOutOfRangeException(nameof(c));
            ColumnPosition = c;
            _prefix = q ?? throw new ArgumentNullException(nameof(q));
            _inner = r ?? throw new ArgumentNullException(nameof(r));
        }

        internal PrefixedReader(TextReader r)
        {
            Position = 1;
            LinePosition = 1;
            ColumnPosition = 1;
            _prefix = EmptyQueue;
            _inner = r ?? throw new ArgumentNullException(nameof(r));
        }

        public int Peek()
        {
            return _prefix.Count == 0 ? _inner.Peek() : _prefix.Peek();
        }

        public int Read()
        {
            var curr = _prefix.Count == 0 ? _inner.Read() : _prefix.Dequeue();
            if (curr == '\r' || curr == '\n')
            {
                var next = _prefix.Count == 0 ? _inner.Peek() : _prefix.Peek();
                if (next != '\r' && next != '\n')
                {
                    LinePosition++;
                    ColumnPosition = 0;
                }
            }
            if (curr > -1)
            {
                Position++;
                ColumnPosition++;
            }
            return _last = curr;
        }

        public void Reattach(int p, int l, int c, Queue<char> prefix)
        {
            if (p < 1) throw new ArgumentOutOfRangeException(nameof(p));
            Position = p;
            if (l < 1) throw new ArgumentOutOfRangeException(nameof(l));
            LinePosition = l;
            if (c < 1) throw new ArgumentOutOfRangeException(nameof(c));
            ColumnPosition = c;
            _prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
        }
    }
}
