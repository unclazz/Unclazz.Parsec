using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.CharClasses
{
    sealed class SingleCharRangeCharClass : CharClass
    {
        internal CharRange CharRange { get; }
        readonly IDictionary<char, bool?> _cache = new Dictionary<char, bool?>();

        internal SingleCharRangeCharClass(CharRange rs)
        {
            CharRange = rs ?? throw new ArgumentNullException(nameof(rs));
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
                var result = CharRange.Contains(ch);
                _cache[ch] = result;
                return result;
            }
        }
        public override CharClass Union(CharClass other)
        {
            var that0 = other as CharRangesCharClass;
            if (that0 != null) return CharRangesCharClass.AnyOf(new[] { CharRange }.Concat(that0.CharRanges).ToArray());
            var that1 = other as SingleCharRangeCharClass;
            if (that1 != null) return CharRangesCharClass.AnyOf(CharRange, that1.CharRange);
            return base.Union(other);
        }
        public override CharClass Plus(CharRange range)
        {
            return CharRangesCharClass.AnyOf(CharRange, range);
        }
    }
}
