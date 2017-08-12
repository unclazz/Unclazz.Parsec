using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.CharClass
{
    sealed class DelegateCharClass : CharClass
    {
        internal DelegateCharClass(Func<char, bool> func)
        {
            Delegate = func ?? throw new ArgumentNullException(nameof(func));
        }
        Func<char, bool> Delegate { get; }
        public override bool Contains(char ch)
        {
            return Delegate(ch);
        }
    }
}
