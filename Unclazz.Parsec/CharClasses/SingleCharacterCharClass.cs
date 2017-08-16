using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec.CharClasses
{
    sealed class SingleCharacterCharClass : CharClass
    {
        internal char Character { get; }

        internal SingleCharacterCharClass(char ch)
        {
            Character = ch;
        }
        public override bool Contains(char ch)
        {
            return Character == ch;
        }
        public override CharClass Union(CharClass other)
        {
            var that0 = other as CharactersCharClass;
            if (that0 != null) return new CharactersCharClass(that0.Characters.Concat(new[] { Character }).Distinct().ToArray());
            var that1 = other as SingleCharacterCharClass;
            if (that1 != null) return new CharactersCharClass(that1.Character, Character);
            return base.Union(other);
        }
        public override CharClass Plus(char ch)
        {
            return new CharactersCharClass(Character, ch);
        }
    }
}
