using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.Readers
{
    sealed class ResettableReader : AutoDispose, IResettableReader
    {
        internal ResettableReader(TextReader r) : this(new TextReaderProxy(r)) { }
        internal ResettableReader(ITextReader r)
        {
            _inner = new PrependableReader(r ?? throw new ArgumentNullException(nameof(r)));
        }

        bool _marked;
        readonly PrependableReader _inner;
        readonly Queue<char> _backup = new Queue<char>();
        readonly Stack<CharPosition> _marks = new Stack<CharPosition>();

        public CharPosition Position => _inner.Position;
        public bool EndOfFile => _inner.EndOfFile;
        protected override IDisposable Disposable => _inner;

        public void Mark()
        {
            _marked = true;
            _marks.Push(Position);
        }
        public void Unmark()
        {
            if (_marked)
            {
                _marks.Pop();
                _marked = _marks.Count > 0;
                if (!_marked)
                {
                    _backup.Clear();
                }
            }
        }
        public string Capture(bool unmark)
        {
            if (_marked)
            {
                var delta = Position.Index - _marks.Peek().Index;
                var skip = _backup.Count - delta;
                var buff = new StringBuilder();
                foreach (var ch in _backup.Skip(skip)) buff.Append(ch);
                if (unmark) Unmark();
                return buff.ToString();
            }
            return null;
        }
        public void Reset(bool unmark)
        {
            if (_marked)
            {
                var lastMark = _marks.Peek();
                var delta = Position.Index - lastMark.Index;

                // 現在の文字位置とマークした文字位置が同値ならリセットは不要
                if (delta == 0) return; 

                // それ以外の場合は完全もしくは部分リセットが必要
                // バックアップの現状の情報を一時変数に移動
                var bkCount = _backup.Count;
                var bkList = _backup.ToArray();
                _backup.Clear();
                // マークした文字位置との間の添字差分とバックアップされていた要素数が一致するかどうかチェック
                if (delta == bkCount)
                {
                    // 一致する場合は完全リセット
                    // バックアップされていた要素すべてを使用してリセットを行う
                    _inner.Reattach(lastMark, new Queue<char>(bkList));
                }
                else if (delta < bkCount)
                {
                    // 一致しない場合は部分リセット
                    // バックアップされていた要素のうち必要な分だけを使用してリセットを行う
                    // バックアップの残りは再度バックアップ・キューに投入
                    var newPrefix = new Queue<char>();
                    var newBkCount = bkCount - delta;
                    for (var i = 0; i < bkCount; i++)
                    {
                        (i < newBkCount ? _backup : newPrefix).Enqueue(bkList[i]);
                    }
                    _inner.Reattach(lastMark, newPrefix);
                }
                else
                {
                    // あってはならないパス
                    // なにか良からぬことが起きている
                    throw new InvalidOperationException();
                }
                if (unmark) Unmark();
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
