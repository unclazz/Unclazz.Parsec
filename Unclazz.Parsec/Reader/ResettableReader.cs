using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.Reader
{
    sealed class ResettableReader : AutoDispose, IResettableReader
    {
        readonly PrependableReader _inner;
        bool _marked;
        CharacterPosition _markedPosition;
        readonly Queue<char> _backup = new Queue<char>();

        public CharacterPosition Position => _inner.Position;
        public bool EndOfFile => _inner.EndOfFile;
        protected override IDisposable Disposable => _inner;

        internal ResettableReader(TextReader r) : this(new TextReaderProxy(r)) { }
        internal ResettableReader(ITextReader r)
        {
            _inner = new PrependableReader(r) ?? throw new ArgumentNullException(nameof(r));
        }

        public void Mark()
        {
            _marked = true;
            _markedPosition = Position;
            _backup.Clear();
        }

        public void Unmark()
        {
            _marked = false;
            _backup.Clear();
        }

        public void Reset()
        {
            if (_marked)
            {
                _inner.Reattach(_markedPosition, new Queue<char>(_backup));
            }
        }

        public int Peek() => _inner.Peek();

        public int Read()
        {
            var ch = _inner.Read();
            if (_marked && ch != -1) _backup.Enqueue((char)ch);
            return ch;
        }

        public string ReadLine() => _inner.ReadLine();
    }
}
