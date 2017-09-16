using System;
using System.Collections;
using System.Collections.Generic;
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
            _prefixQueue = PrefixQueue.Empty;
            _inner = r ?? throw new ArgumentNullException(nameof(r));
        }
        internal PrependableReader(CharPosition p, char[] q, TextReader r) : this(p, q, new TextReaderProxy(r)) { }
        internal PrependableReader(CharPosition p, char[] q, ITextReader r)
        {
            Position = p;
            _prefixQueue = PrefixQueue.From(q ?? throw new ArgumentNullException(nameof(q)));
            _inner = r ?? throw new ArgumentNullException(nameof(r));
        }

        PrefixQueue _prefixQueue;
        readonly ITextReader _inner;

        protected override IDisposable Disposable => _inner;

        public override int Peek()
        {
            return _prefixQueue.HasItems ? _prefixQueue.Peek() : _inner.Peek();
        }
        public override int ReadSimply()
        {
            return _prefixQueue.HasItems ? _prefixQueue.Dequeue() : _inner.Read();
        }
        public void Reattach(CharPosition p, char[] prefix)
        {
            Position = p;
            if (prefix == null) throw new ArgumentNullException(nameof(prefix));

            _prefixQueue = _prefixQueue.Prepend(prefix);
        }

        public sealed class PrefixQueue : IEnumerable<char>
        {
            public static PrefixQueue Empty => new PrefixQueue(new char[0]);
            public static PrefixQueue From(char[] items)
            {
                if (items == null) throw new ArgumentNullException(nameof(items));
                var copy = new char[items.Length];
                items.CopyTo(copy, 0);
                return new PrefixQueue(copy);
            }

            PrefixQueue(char[] items)
            {
                _items = items;
            }

            readonly char[] _items;
            int _index;

            public int Count => _items.Length - _index;
            public bool HasItems => _index < _items.Length;
            public char Peek() => HasItems ? _items[_index] : throw new InvalidOperationException("no item.");
            public char Dequeue() => HasItems ? _items[_index++] : throw new InvalidOperationException("no item.");
            public PrefixQueue Prepend(char[] prefix)
            {
                if (prefix == null) throw new ArgumentNullException(nameof(prefix));
                if (prefix.Length == 0) return this;
                if (!HasItems) return From(prefix);

                var restCount = Count;
                var newItems = new char[prefix.Length + restCount];

                Array.Copy(prefix, 0, newItems, 0, prefix.Length);
                Array.Copy(_items, _index, newItems, prefix.Length, restCount);

                return new PrefixQueue(newItems);
            }

            public IEnumerator<char> GetEnumerator()
            {
                while (HasItems) yield return Dequeue();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
