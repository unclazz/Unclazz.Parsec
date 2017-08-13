using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.Reader
{
    sealed class PrependableReader : AbstractTextReader
    {
        Queue<char> _prefix;
        ITextReader _internal;

        ITextReader Internal
        {
            get
            {
                if (_internal == null) throw new InvalidOperationException();
                return _internal;
            }
            set
            {
                _internal = value;
            }
        }
        protected override IDisposable Disposable => Internal;

        internal PrependableReader(CharacterPosition p, Queue<char> q, TextReader r) : this(p, q, new TextReaderProxy(r)) { }
        internal PrependableReader(CharacterPosition p, Queue<char> q, ITextReader r)
        {
            Position = p;
            _prefix = q ?? throw new ArgumentNullException(nameof(q));
            Internal = r ?? throw new ArgumentNullException(nameof(r));
        }

        internal PrependableReader(TextReader r)
        {
            Position = CharacterPosition.StartOfFile;
            _prefix = new Queue<char>();
            Internal = new TextReaderProxy(r);
        }
        internal PrependableReader(ITextReader r)
        {
            Position = r.Position;
            _prefix = new Queue<char>();
            Internal = r ?? throw new ArgumentNullException(nameof(r));
        }

        public override int Peek() => _prefix.Count == 0 ? Internal.Peek() : _prefix.Peek();

        public override int ReadSimply()
        {
            return _prefix.Count == 0 ? Internal.Read() : _prefix.Dequeue();
        }

        public void Reattach(CharacterPosition p, Queue<char> prefix)
        {
            Position = p;
            _prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
        }
        public IResettableReader Unwrap()
        {
            var tmp = Internal as ResettableReader;
            if (tmp == null) throw new InvalidOperationException();
            Internal = null;
            return tmp;
        }
    }
}
