using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec.Reader;

namespace Unclazz.Parsec
{
    public sealed class ParserInput : AutoDispose, IResettableReader
    {
        public static ParserInput FromStream(Stream stream, Encoding enc)
        {
            return FromReader(new StreamReader(stream, enc));
        }
        public static ParserInput FromFile(string filepath, Encoding enc)
        {
            return FromStream(new FileStream(filepath, FileMode.Open), enc);
        }
        public static ParserInput FromString(string text)
        {
            return FromReader(new StringReader(text));
        }
        public static ParserInput FromReader(TextReader reader)
        {
            return new ParserInput(new ResettableReader(reader));
        }

        ParserInput(IResettableReader r)
        {
            _inner = r ?? throw new ArgumentNullException(nameof(r));
        }

        readonly IResettableReader _inner;

        public CharacterPosition Position => _inner.Position;
        public bool EndOfFile => _inner.EndOfFile;
        protected override IDisposable Disposable => _inner;

        public int Peek() => _inner.Peek();
        public int Read() => _inner.Read();
        public string ReadLine() => _inner.ReadLine();
        public void Mark() => _inner.Mark();
        public void Unmark() => _inner.Unmark();
        public void Reset() => _inner.Reset();
        public string Capture(bool unmark) => _inner.Capture(unmark);
    }
}
