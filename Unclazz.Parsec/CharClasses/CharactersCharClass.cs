using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.CharClasses
{
    sealed class CharactersCharClass : CharClass
    {
        internal IEnumerable<char> Characters { get; }
        readonly IDictionary<char, bool?> _cache = new Dictionary<char, bool?>();

        internal CharactersCharClass(params char[] cs)
        {
            Characters = cs ?? throw new ArgumentNullException(nameof(cs));
        }
        internal CharactersCharClass(IEnumerable<char> cs)
        {
            if (cs == null) throw new ArgumentNullException(nameof(cs));
            Characters = cs.Distinct().ToArray();
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
                var result = Characters.Any(_ch => _ch == ch);
                _cache[ch] = result;
                return result;
            }
        }
        public override CharClass Union(CharClass other)
        {
            var that = other as CharactersCharClass;
            if (that != null) return new CharactersCharClass(Characters.Concat(that.Characters).Distinct().ToArray());
            var that1 = other as SingleCharacterCharClass;
            if (that1 != null) return new CharactersCharClass(Characters.Concat(new[] { that1.Character }).Distinct().ToArray());
            return base.Union(other);
        }
        public override CharClass Plus(char ch)
        {
            return new CharactersCharClass(Characters.Concat(new[] { ch }).Distinct().ToArray());
        }
    }
}
