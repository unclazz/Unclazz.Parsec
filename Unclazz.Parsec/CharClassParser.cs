using System;

namespace Unclazz.Parsec
{
    sealed class CharClassParser : Parser<string>
    {
        internal CharClassParser(CharClass.CharClass clazz)
        {
            _clazz = clazz ?? throw new ArgumentNullException(nameof(clazz));
        }

        readonly CharClass.CharClass _clazz;

        public override ParseResult<string> Parse(ParserInput input)
        {
            var p = input.Position;
            var ch = (char)input.Read();
            return (_clazz.Contains(ch))
                ? ParseResult.OfSuccess<string>(p)
                : ParseResult.OfFailure<string>(p, 
                string.Format("expected a member of {0} but found '{1}' (code = {2})", _clazz, ch, (int) ch));
        }
        public override string ToString()
        {
            return string.Format("CharClass({0})", _clazz);
        }
    }
}
