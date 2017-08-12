using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.CharClass
{
    sealed class UnionCharClass : CharClass
    {
        readonly CharClass[] _classes;
        readonly IDictionary<char, bool?> _cache = new Dictionary<char, bool?>();
        internal UnionCharClass(params CharClass[] cs)
        {
            _classes = cs ?? throw new ArgumentNullException(nameof(cs));
        }
        public override bool Contains(char ch)
        {
            bool? cachedResult = null;
            if (_cache.TryGetValue(ch, out cachedResult))
            {
                return cachedResult.Value;
            }
            else
            {
                var result = _classes.Any(c => c.Contains(ch));
                _cache[ch] = result;
                return result;
            }
        }
    }
}
