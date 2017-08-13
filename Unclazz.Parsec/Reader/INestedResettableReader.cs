using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.Reader
{
    public interface INestedResettableReader : IResettableReader
    {
        void Nest();
        void Unnest();
    }
}
