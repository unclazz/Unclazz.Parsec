using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
            _prefixReader = CharArrayReader.Empty;
            _inner = r ?? throw new ArgumentNullException(nameof(r));
        }
        internal PrependableReader(CharPosition p, char[] q, TextReader r) : this(p, q, new TextReaderProxy(r)) { }
        internal PrependableReader(CharPosition p, char[] q, ITextReader r)
        {
            Position = p;
            _prefixReader = CharArrayReader.From(q ?? throw new ArgumentNullException(nameof(q)));
            _inner = r ?? throw new ArgumentNullException(nameof(r));
        }

        CharArrayReader _prefixReader;
        readonly ITextReader _inner;

        protected override IDisposable Disposable => _inner;

        public override int Peek()
        {
            return _prefixReader.EndOfFile ? _inner.Peek() : _prefixReader.Peek();
        }
        public override int ReadSimply()
        {
            return _prefixReader.EndOfFile ? _inner.Read() : _prefixReader.Read();
        }
        public void Reattach(CharPosition p, char[] prefix)
        {
            Position = p;
            if (prefix == null) throw new ArgumentNullException(nameof(prefix));

            _prefixReader = _prefixReader.Prepend(prefix);
        }
    }
}
