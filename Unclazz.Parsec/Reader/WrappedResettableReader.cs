using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.Reader
{
    ///// <summary>
    ///// <see cref="IResettableReader"/>をラップする<see cref="IResettableReader"/>です。
    ///// このクラスは<see cref="INestedResettableReader"/>の実装のために利用されます。
    ///// </summary>
    //sealed class WrappedResettableReader : AutoDispose, IResettableReader
    //{
    //    IResettableReader _internal;
    //    bool _marked;
    //    CharacterPosition _markedPosition;
    //    readonly Queue<char> _backup = new Queue<char>();

    //    IResettableReader Internal
    //    {
    //        get
    //        {
    //            if (_internal == null) throw new InvalidOperationException();
    //            return _internal;
    //        }
    //        set
    //        {
    //            _internal = value;
    //        }
    //    }
    //    internal WrappedResettableReader(IResettableReader r)
    //    {
    //        Internal = r ?? throw new ArgumentNullException(nameof(r));
    //    }

    //    public CharacterPosition Position => Internal.Position;
    //    public bool EndOfFile => Internal.EndOfFile;
    //    protected override IDisposable Disposable => Internal;

    //    public void Mark()
    //    {
    //        _marked = true;
    //        _markedPosition = Position;
    //        _backup.Clear();
    //    }

    //    public void Unmark()
    //    {
    //        _marked = false;
    //        _backup.Clear();
    //    }

    //    public void Reset()
    //    {
    //        if (_marked)
    //        {
    //            Internal.Reset();
    //        }
    //    }

    //    public int Peek() => Internal.Peek();

    //    public int Read()
    //    {
    //        var ch = Internal.Read();
    //        if (_marked && ch != -1) _backup.Enqueue((char)ch);
    //        return ch;
    //    }

    //    public string ReadLine() => Internal.ReadLine();

    //    public IResettableReader Unwrap()
    //    {
    //        var tmp = Internal;
    //        Internal = null;
    //        return tmp;
    //    }
    //}
}
