using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.CharClasses
{
    sealed class ComplementCharClass : CharClass
    {
        internal ComplementCharClass(CharClass original)
        {
            Original = original ?? throw new ArgumentNullException(nameof(original));
        }

        CharClass Original { get; }
        public override bool Contains(char ch)
        {
            return !Original.Contains(ch);
        }
    }
}
