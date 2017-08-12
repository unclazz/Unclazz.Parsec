using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec
{
    public abstract class ParseResult<T>
    {
        internal ParseResult()
        {
        }

        public T Value { get; }
        public int Position { get; }
        public int LinePosition { get; }
        public int ColumnPosition { get; }
        public string Message { get; }
        bool Success => Position > 0;
    }
}
