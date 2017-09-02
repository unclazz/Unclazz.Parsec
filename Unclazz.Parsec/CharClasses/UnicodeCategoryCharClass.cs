using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.CharClasses
{
    sealed class UnicodeCategoryCharClass : CharClass
    {
        readonly IDictionary<char, bool?> _cache = new Dictionary<char, bool?>();
        readonly UnicodeCategory[] _categories;
        string _descCache;

        internal UnicodeCategoryCharClass(params UnicodeCategory[] cs)
        {
            _categories = cs ?? throw new ArgumentNullException(nameof(cs));
        }

        public override string Description => _descCache ??
            (_descCache = _categories.Aggregate(new StringBuilder(),
                (a, b) => a.Append(a.Length == 0 ? string.Empty : " | ").Append(b.ToString()),
                (a) => a.ToString()));

        public override bool Contains(char ch)
        {
            bool? cachedResult = null;
            if (_cache.TryGetValue(ch, out cachedResult))
            {
                return cachedResult.Value;
            }
            else
            {
                var result = _categories.Any(c => c == char.GetUnicodeCategory(ch));
                _cache[ch] = result;
                return result;
            }
        }
        public override CharClass Union(CharClass other)
        {
            var that = other as UnicodeCategoryCharClass;
            if (that != null) new UnicodeCategoryCharClass(_categories.Concat(that._categories).Distinct().ToArray());
            return base.Union(other);
        }
        public override CharClass Plus(UnicodeCategory cate)
        {
            return new UnicodeCategoryCharClass(_categories.Concat(new[] { cate }).Distinct().ToArray());
        }
    }
}
