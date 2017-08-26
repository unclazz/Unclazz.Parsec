using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class CharClassParser : Parser
    {
        internal CharClassParser(CharClasses.CharClass clazz)
        {
            _clazz = clazz ?? throw new ArgumentNullException(nameof(clazz));
        }

        readonly CharClasses.CharClass _clazz;

        public override ParseResult<Nil> Parse(Reader input)
        {
            var p = input.Position;
            var ch = input.Read();
            if (0 <= ch && _clazz.Contains((char)ch))
            {
                return Success(p);
            }
            else
            {
                return Failure(p, string.Format("expected a member of {0} but found {1}.", 
                    _clazz, ParsecUtility.CharToString(ch)));
            }
        }
        public override string ToString()
        {
            return string.Format("CharClass({0})", _clazz);
        }
    }
}
