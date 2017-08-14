using System;
using System.Text;

namespace Unclazz.Parsec
{
    sealed class WordParser : Parser<string>
    {
        internal WordParser(string word)
        {
            _word = word ?? throw new ArgumentNullException(nameof(word));
            if (word.Length == 0) throw new ArgumentException("word.Length == 0");
        }

        readonly string _word;

        public override ParseResult<string> Parse(ParserInput input)
        {
            var p = input.Position;
            var buff = new StringBuilder();
            for (var i = 0; i < _word.Length && !input.EndOfFile; i++)
            {
                buff.Append((char)input.Read());
            }
            return _word == buff.ToString() ? ParseResult.OfSuccess(p, _word)
                : ParseResult.OfFailure<string>(p, 
                string.Format("expected word \"{0}\" but found \"{1}\"", _word, buff));
        }
        public override string ToString()
        {
            return string.Format("Word({0})", _word);
        }
    }
}
