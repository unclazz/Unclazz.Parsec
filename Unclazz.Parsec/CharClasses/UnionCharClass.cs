using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.CharClasses
{
    sealed class UnionCharClass : CharClass
    {
        readonly CharClass[] _classes;
        readonly IDictionary<char, bool?> _cache = new Dictionary<char, bool?>();
        string _descCache;

        internal UnionCharClass(params CharClass[] cs)
        {
            _classes = cs ?? throw new ArgumentNullException(nameof(cs));
        }

        public override string Description => _descCache ??
            (_descCache = _classes.Aggregate(new StringBuilder(),
                (a, b) => a.Append(a.Length == 0 ? "(" : ") | (").Append(b.Description),
                (a) => a.Append(")").ToString()));

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
