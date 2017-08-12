using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.CharClass
{
    sealed class SingleUnicodeCategoryCharClass : CharClass
    {
        internal UnicodeCategory UnicodeCategory { get; }

        internal SingleUnicodeCategoryCharClass(UnicodeCategory cs)
        {
            UnicodeCategory = cs;
        }

        public override bool Contains(char ch)
        {
            return UnicodeCategory == char.GetUnicodeCategory(ch);
        }
        public override CharClass Union(CharClass other)
        {
            var that0 = other as UnicodeCategoriesCharClass;
            if (that0 != null) new UnicodeCategoriesCharClass(new[] { UnicodeCategory }
                                .Concat(that0.UnicodeCategories).Distinct().ToArray());
            var that1 = other as SingleUnicodeCategoryCharClass;
            if (that1 != null) new UnicodeCategoriesCharClass(UnicodeCategory, that1.UnicodeCategory);
            return base.Union(other);
        }
        public override CharClass Plus(UnicodeCategory cate)
        {
            return new UnicodeCategoriesCharClass(UnicodeCategory, cate);
        }
    }
}
