using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.CharClasses
{
    sealed class PredicateCharClass : CharClass
    {
        internal PredicateCharClass(Func<char, bool> pred)
        {
            _pred = pred ?? throw new ArgumentNullException(nameof(pred));
            _desc = ParsecUtility.ObjectTypeToString(pred);
        }
        internal PredicateCharClass(Func<char, bool> pred, string desc)
        {
            _pred = pred ?? throw new ArgumentNullException(nameof(pred));
            _desc = desc ?? throw new ArgumentNullException(nameof(desc));
        }
        readonly string _desc;
        readonly Func<char, bool> _pred;

        public override string Description => _desc;

        public override bool Contains(char ch)
        {
            return _pred(ch);
        }
    }
}
