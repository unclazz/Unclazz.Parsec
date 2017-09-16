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
        internal PrependableReader(CharPosition p, Queue<char> q, TextReader r) : this(p, q, new TextReaderProxy(r)) { }
        internal PrependableReader(CharPosition p, Queue<char> q, ITextReader r)
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
//#if DEBUG
//            Console.WriteLine("[DEBUG]\t{0}.{1}\t(start):\t_prefix={2}\t_inner.Peek={3}\tPosition={4}",
//                nameof(PrependableReader), nameof(ReadSimply),
//                _prefix.Aggregate(new StringBuilder(), (a, b) => a.Append(b), a => a.ToString()),
//                _inner.Peek(), Position);
//#endif

            var ch = _prefix.Count == 0 ? _inner.Read() : _prefix.Dequeue();

//#if DEBUG
//            Console.WriteLine("[DEBUG]\t{0}.{1}\t(end):\t_prefix={2}\t_inner.Peek={3}\tPosition={4}",
//                nameof(PrependableReader), nameof(ReadSimply),
//                _prefix.Aggregate(new StringBuilder(), (a, b) => a.Append(b), a => a.ToString()),
//                _inner.Peek(), Position);
//#endif

            return ch;
        }
        public void Reattach(CharPosition p, Queue<char> prefix)
        {
//#if DEBUG
//            Console.WriteLine("[DEBUG]\t{0}.{1}\t(start):\t_prefix={2}\t_inner.Peek={3}\tPosition={4}",
//                nameof(PrependableReader), nameof(Reattach),
//                _prefix.Aggregate(new StringBuilder(), (a, b) => a.Append(b), a => a.ToString()),
//                _inner.Peek(), Position);
//#endif

            Position = p;
            var origPrefix = _prefix;
            _prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
            foreach (var e in origPrefix) _prefix.Enqueue(e);

//#if DEBUG
//            Console.WriteLine("[DEBUG]\t{0}.{1}\t(end):\t_prefix={2}\t_inner.Peek={3}\tPosition={4}",
//                nameof(PrependableReader), nameof(Reattach),
//                _prefix.Aggregate(new StringBuilder(), (a, b) => a.Append(b), a => a.ToString()),
//                _inner.Peek(), Position);
//#endif
        }
    }
}
