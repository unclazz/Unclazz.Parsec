using System;
using System.Text;

namespace Unclazz.Parsec
{
    sealed class WhileCharClassParser: Parser<string>
    {
        internal WhileCharClassParser(CharClass.CharClass clazz)
        {
            _clazz = clazz ?? throw new ArgumentNullException(nameof(clazz));
        }

        readonly CharClass.CharClass _clazz;

        public override ParseResult<string> Parse(ParserInput input)
        {
            var p = input.Position;
            var buff = new StringBuilder();
            while (!input.EndOfFile)
            {
                if (_clazz.Contains((char)input.Peek())) buff.Append((char)input.Read());
                else break;
            }
            return ParseResult.OfSuccess(p, buff.ToString());
        }
    }
}
