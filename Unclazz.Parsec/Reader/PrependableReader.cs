using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.Reader
{
    sealed class PrependableReader : AutoDispose, ITextReader
    {
        static readonly Queue<char> EmptyQueue = new Queue<char>();

        int _last;
        Queue<char> _prefix;
        readonly TextReader _inner;

        public CharacterPosition Position { get; private set; }
        //public int LinePosition { get; private set; }
        //public int ColumnPosition { get; private set; }
        public bool EndOfFile => Peek() == -1;
        protected override IDisposable Disposable => _inner;

        internal PrependableReader(CharacterPosition p, Queue<char>  q, TextReader r)
        {
            //if (p < 1) throw new ArgumentOutOfRangeException(nameof(p));
            Position = p;
            //if (l < 1) throw new ArgumentOutOfRangeException(nameof(l));
            //LinePosition = l;
            //if (c < 1) throw new ArgumentOutOfRangeException(nameof(c));
            //ColumnPosition = c;
            _prefix = q ?? throw new ArgumentNullException(nameof(q));
            _inner = r ?? throw new ArgumentNullException(nameof(r));
        }

        internal PrependableReader(TextReader r)
        {
            Position = CharacterPosition.StartOfFile;
            //LinePosition = 1;
            //ColumnPosition = 1;
            _prefix = EmptyQueue;
            _inner = r ?? throw new ArgumentNullException(nameof(r));
        }

        public int Peek() => _prefix.Count == 0 ? _inner.Peek() : _prefix.Peek();

        public int Read()
        {
            var curr = _prefix.Count == 0 ? _inner.Read() : _prefix.Dequeue();
            if (curr == '\n' || (curr == '\r' && Peek() != '\n'))
            {
                Position = Position.NextLine;
                //LinePosition++;
                //ColumnPosition = 0;
            }
            else if (curr > -1)
            {
                Position = Position.NextColumn;
                //Position++;
                //ColumnPosition++;
            }
            return _last = curr;
        }

        public string ReadLine()
        {
            //var l = LinePosition;
            var startedOn = Position.Line;
            var buff = new StringBuilder();
            while (startedOn == Position.Line && EndOfFile)
            {
                var ch = Read();
                if (ch != '\r' && ch != '\n') buff.Append(ch);
            }
            return buff.ToString();
        }

        public void Reattach(CharacterPosition p, Queue<char> prefix)
        {
            //if (p < 1) throw new ArgumentOutOfRangeException(nameof(p));
            Position = p;
            //if (l < 1) throw new ArgumentOutOfRangeException(nameof(l));
            //LinePosition = l;
            //if (c < 1) throw new ArgumentOutOfRangeException(nameof(c));
            //ColumnPosition = c;
            _prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
        }
    }
}
