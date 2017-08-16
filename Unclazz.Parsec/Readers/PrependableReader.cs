using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.Readers
{
    sealed class PrependableReader : AbstractTextReader
    {
        internal PrependableReader(TextReader r) : this(new TextReaderProxy(r)) { }
        internal PrependableReader(ITextReader r)
        {
            Position = r.Position;
            _prefix = new Queue<char>();
            _inner = r ?? throw new ArgumentNullException(nameof(r));
        }
        internal PrependableReader(CharacterPosition p, Queue<char> q, TextReader r) : this(p, q, new TextReaderProxy(r)) { }
        internal PrependableReader(CharacterPosition p, Queue<char> q, ITextReader r)
        {
            Position = p;
            _prefix = q ?? throw new ArgumentNullException(nameof(q));
            _inner = r ?? throw new ArgumentNullException(nameof(r));
        }

        Queue<char> _prefix;
        readonly ITextReader _inner;

        protected override IDisposable Disposable => _inner;

        public override int Peek() => _prefix.Count == 0 ? _inner.Peek() : _prefix.Peek();
        public override int ReadSimply()
        {
            return _prefix.Count == 0 ? _inner.Read() : _prefix.Dequeue();
        }
        public void Reattach(CharacterPosition p, Queue<char> prefix)
        {
            Position = p;
            _prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
        }
    }
}
