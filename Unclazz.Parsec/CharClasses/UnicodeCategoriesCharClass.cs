using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.CharClasses
{
    sealed class UnicodeCategoriesCharClass : CharClass
    {
        readonly IDictionary<char, bool?> _cache = new Dictionary<char, bool?>();
        internal IEnumerable<UnicodeCategory> UnicodeCategories { get; }

        internal UnicodeCategoriesCharClass(params UnicodeCategory[] cs)
        {
            UnicodeCategories = cs ?? throw new ArgumentNullException(nameof(cs));
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
                var result = UnicodeCategories.Any(c => c == char.GetUnicodeCategory(ch));
                _cache[ch] = result;
                return result;
            }
        }
        public override CharClass Union(CharClass other)
        {
            var that = other as UnicodeCategoriesCharClass;
            if (that != null) new UnicodeCategoriesCharClass(UnicodeCategories.Concat(that.UnicodeCategories).Distinct().ToArray());
            var that1 = other as SingleUnicodeCategoryCharClass;
            if (that1 != null) new UnicodeCategoriesCharClass(UnicodeCategories.Concat(new[] { that1.UnicodeCategory }).Distinct().ToArray());
            return base.Union(other);
        }
        public override CharClass Plus(UnicodeCategory cate)
        {
            return new UnicodeCategoriesCharClass(UnicodeCategories.Concat(new[] { cate }).Distinct().ToArray());
        }
    }
}
