using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.Readers
{
    /// <summary>
    /// 実装済みの<see cref="IDisposable.Dispose()"/>を提供する抽象クラスです。
    /// <para>
    /// 具象クラスは<see cref="Disposable"/>を通じて解放対象のアンマネージ・リソースへの参照を返します。
    /// </para>
    /// </summary>
    public abstract class AutoDispose : IDisposable
    {
        /// <summary>
        /// デストラクタです。
        /// </summary>
        ~AutoDispose()
        {
            Dispose(false);
        }

        bool _disposed;

        /// <summary>
        /// アンマネージ・リソースへの参照です。
        /// </summary>
        protected abstract IDisposable Disposable { get; }

        /// <summary>
        /// アンマネージ・リソースを解放します。
        /// </summary>
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
    }
}
