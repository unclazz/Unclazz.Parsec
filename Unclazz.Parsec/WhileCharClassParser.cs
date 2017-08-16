using System;
using System.Text;

namespace Unclazz.Parsec
{
    sealed class WhileCharClassParser: Parser<string>
    {
        internal WhileCharClassParser(CharClasses.CharClass clazz)
        {
            _clazz = clazz ?? throw new ArgumentNullException(nameof(clazz));
        }

        readonly CharClasses.CharClass _clazz;

        public override ParseResult<string> Parse(ParserInput input)
        {
            var p = input.Position;
            while (!input.EndOfFile)
            {
                if (_clazz.Contains((char)input.Peek())) input.Read();
                else break;
            }
            return ParseResult.OfSuccess<string>(p);
        }
        public override string ToString()
        {
            return string.Format("WhileCharClass({0})", _clazz);
        }
    }
}
