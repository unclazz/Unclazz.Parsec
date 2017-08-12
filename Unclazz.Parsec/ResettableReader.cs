using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec
{
    sealed class ResettableReader : AutoDispose, IResettableReader
    {
        readonly PrependableReader _inner;
        bool _marked;
        CharacterPosition _markedPosition;
        //int _markedLinePosition;
        //int _markedColumnPosition;
        readonly Queue<char> _backup = new Queue<char>();

        public CharacterPosition Position => _inner.Position;
        //public int LinePosition => _inner.LinePosition;
        //public int ColumnPosition => _inner.ColumnPosition;
        public bool EndOfFile => _inner.EndOfFile;
        protected override IDisposable Disposable => _inner;

        internal ResettableReader(TextReader r)
        {
            _inner = new PrependableReader(r) ?? throw new ArgumentNullException(nameof(r));
        }

        public void Mark()
        {
            _marked = true;
            _markedPosition = Position;
            //_markedLinePosition = LinePosition;
            //_markedColumnPosition = ColumnPosition;
            _backup.Clear();
        }

        public void Unmark()
        {
            _marked = false;
            //_markedPosition = 0;
            //_markedLinePosition = 0;
            //_markedColumnPosition = 0;
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
