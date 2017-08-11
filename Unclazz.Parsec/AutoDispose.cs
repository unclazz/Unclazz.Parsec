using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec
{
    abstract class AutoDispose : IDisposable
    {
        bool _disposed;

        ~AutoDispose()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                var d = Disposable;
                if (d != null) d.Dispose();
            }
            _disposed = true;
        }

        protected abstract IDisposable Disposable { get; }
    }
}
