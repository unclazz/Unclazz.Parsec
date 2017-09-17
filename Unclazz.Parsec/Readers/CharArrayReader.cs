using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Unclazz.Parsec.Readers
{
    sealed partial class PrependableReader
    {
        public sealed class CharArrayReader
        {
            public static CharArrayReader Empty => new CharArrayReader(new char[0]);
            public static CharArrayReader From(char[] items)
            {
                if (items == null) throw new ArgumentNullException(nameof(items));
                var copy = new char[items.Length];
                items.CopyTo(copy, 0);
                return new CharArrayReader(copy);
            }

            CharArrayReader(char[] items)
            {
                _items = items;
            }

            readonly char[] _items;
            int _index;

            public int Count => _items.Length - _index;
            public bool EndOfFile => _items.Length <= _index;
            public int Peek() => EndOfFile ? -1 : _items[_index];
            public int Read() => EndOfFile ? -1 : _items[_index++];
            public CharArrayReader Prepend(char[] prefix)
            {
                if (prefix == null) throw new ArgumentNullException(nameof(prefix));
                if (prefix.Length == 0) return this;
                if (EndOfFile) return From(prefix);

                var restCount = Count;
                var newItems = new char[prefix.Length + restCount];

                Array.Copy(prefix, 0, newItems, 0, prefix.Length);
                Array.Copy(_items, _index, newItems, prefix.Length, restCount);

                return new CharArrayReader(newItems);
            }
            public string ReadToEnd()
            {
                var buff = new StringBuilder();
                while (!EndOfFile) buff.Append((char)Read());
                return buff.ToString();
            }
            public char[] ToArray()
            {
                var array = new char[_items.Length];
                _items.CopyTo(array, 0);
                return array;
            }
        }
    }
}
