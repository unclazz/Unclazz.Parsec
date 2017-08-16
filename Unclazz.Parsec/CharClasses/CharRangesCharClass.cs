using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.CharClasses
{
    sealed class CharRangesCharClass : CharClass
    {
        internal static CharRangesCharClass AnyOf(params CharRange[] rs)
        {
            return new CharRangesCharClass(rs);
        }

        internal IEnumerable<CharRange> CharRanges { get; }
        readonly IDictionary<char, bool?> _cache = new Dictionary<char, bool?>();
        CharRangesCharClass(CharRange[] rs)
        {
            CharRanges = rs ?? throw new ArgumentNullException(nameof(rs));
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
                var result = CharRanges.Any(r => r.Contains(ch));
                _cache[ch] = result;
                return result;
            }
        }
        public override CharClass Union(CharClass other)
        {
            var that0 = other as CharRangesCharClass;
            if (that0 != null) return AnyOf(CharRanges.Concat(that0.CharRanges).ToArray());
            var that1 = other as SingleCharRangeCharClass;
            if (that1 != null) return AnyOf(CharRanges.Concat(new[] { that1.CharRange }).ToArray());
            return base.Union(other);
        }
        public override CharClass Plus(CharRange range)
        {
            return AnyOf(CharRanges.Concat(new[] { range }).ToArray());
        }
    }
}
