using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec.Reader;

namespace Unclazz.Parsec
{
    sealed class ParserInput : AutoDispose, ITextReader
    {
        ParserInput(IResettableReader r)
        {
            Original = r ?? throw new ArgumentNullException(nameof(r));
        }

        public CharacterPosition Position => Original.Position;
        public bool EndOfFile => Original.EndOfFile;
        protected override IDisposable Disposable => Original;
        IResettableReader Original { get; set; }

        //public void PreparesBacktracking()
        //{
        //    Original = new WrappedResettableReader(Original);
        //}
        //public void Backtracks()
        //{
        //    var nested = Original as WrappedResettableReader;
        //    if (nested == null) throw new InvalidOperationException();
        //    Original = nested.Unwrap();
        //}

        public int Peek() => Original.Peek();
        public int Read() => Original.Read();
        public string ReadLine() => Original.ReadLine();
    }
}
