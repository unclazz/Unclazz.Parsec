using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.CharClasses
{
    sealed class CharRangeCharClass : CharClass
    {
        readonly CharRange[] _charRanges;
        readonly IDictionary<char, bool?> _cache = new Dictionary<char, bool?>();
        string _descCache;

        public override string Description => _descCache ??
            (_descCache = _charRanges.Aggregate(new StringBuilder(),
                (a, b) => a.Append(a.Length == 0 ? string.Empty : " | ").Append(b.ToString()),
                (a) => a.ToString()));

        internal CharRangeCharClass(params CharRange[] rs)
        {
            _charRanges = rs ?? throw new ArgumentNullException(nameof(rs));
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
                var result = _charRanges.Any(r => r.Contains(ch));
                _cache[ch] = result;
                return result;
            }
        }
        public override CharClass Union(CharClass other)
        {
            var that = other as CharRangeCharClass;
            if (that == null)
            {
                return base.Union(other);
            }
            return new CharRangeCharClass(CharRangeUtility.TryMerge(_charRanges.Concat(that._charRanges)));
        }
        public override CharClass Plus(char ch)
        {
            return new CharRangeCharClass(CharRangeUtility.TryMerge(_charRanges.Concat(new[] { CharRange.Exactly(ch) })));
        }

    }
}
