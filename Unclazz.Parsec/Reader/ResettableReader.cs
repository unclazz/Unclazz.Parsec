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
        readonly ResettableReader _innerResettable;
        bool _marked;
        CharacterPosition _markedPosition;
        readonly Queue<char> _backup = new Queue<char>();

        public CharacterPosition Position => _inner.Position;
        public bool EndOfFile => _inner.EndOfFile;
        protected override IDisposable Disposable => _inner;

        internal ResettableReader(TextReader r)
        {
            _inner = new PrependableReader(r ?? throw new ArgumentNullException(nameof(r)));
        }
        internal ResettableReader(ITextReader r)
        {
            _inner = new PrependableReader(r ?? throw new ArgumentNullException(nameof(r)));
            _innerResettable = r as ResettableReader;
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
                //if (_innerResettable != null)
                //{
                //    _innerResettable.Reset();
                //    var innerResettableIndex = _innerResettable._markedPosition.Index;
                //    var thisResettableIndex = _markedPosition.Index;
                //    var delta = thisResettableIndex - innerResettableIndex;
                //    for (var i = 0; i < delta; i++)
                //    {
                //        _innerResettable.Read();
                //    }
                //}
                //_inner.Reattach(_markedPosition, new Queue<char>(_backup));
                //_backup.Clear();
                ResetTo(_markedPosition);
            }
        }
        void ResetTo(CharacterPosition p)
        {
            // 内部に抱えるリーダーもResettableReaderかどうかチェック
            if (_innerResettable != null)
            {
                // YESの場合は再帰的なリセットを行う
                _innerResettable.ResetTo(p);
                return;
            }
            var delta = p.Index - _markedPosition.Index;
            var bkCount = _backup.Count;
            var bkList = _backup.ToList();
            _backup.Clear();

            if (delta == 0)
            {
                // 指定された文字位置がマークした文字位置と等しい場合
                // バックアップしたすべての要素をPrependableReaderの先頭に再装着する
                _inner.Reattach(_markedPosition, new Queue<char>(bkList));
            }
            else if (delta > 1)
            {
                // 指定された文字位置 ＞ マークした文字位置 の場合
                // delta分の要素をバックアップ・キューに再設定
                // 残りの要素をPrependableReaderの先頭に再装着する
                foreach(var e in bkList.Take(delta))
                {
                    _backup.Enqueue(e);
                }
                _inner.Reattach(p, new Queue<char>(bkList.Skip(delta)));
            }
            else
            {
                // 指定された文字位置 ＜ マークした文字位置 の場合
                // 指定は無効なので例外をスローする
                throw new InvalidOperationException();
            }
            //// 内部に抱えるリーダーもResettableReaderかどうかチェック
            //if (_innerResettable != null)
            //{
            //    // YESの場合は再帰的なリセットを行う
            //    _innerResettable.ResetTo(p);
            //}
        }

        public int Peek() => _inner.Peek();
        public int Read()
        {
            var ch = _inner.Read();
            if (_marked && ch != -1) _backup.Enqueue((char)ch);
            return ch;
        }
        public string ReadLine() => _inner.ReadLine();
        public IResettableReader Unwrap()
        {
            return _inner.Unwrap();
        }
    }
}
