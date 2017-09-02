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
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }
        readonly CharClass _original;
        string _descCache;
        public override string Description => _descCache ??
            (_descCache = string.Format("not({0})", _original.Description));
        public override bool Contains(char ch)
        {
            return !_original.Contains(ch);
        }
    }
}
