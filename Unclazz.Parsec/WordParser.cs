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
            for (var i = 0; i < _word.Length; i++)
            {
                var expected = _word[i];
                var actual = input.Read();
                if (actual == -1)
                {
                    return Failure(input.Position,
                        string.Format("expected '{0}'(at index {1} in \"{2}\") but found EOF",
                        expected, i, _word, (char)actual));
                }
                else if (expected != actual)
                {
                    return Failure(input.Position, 
                        string.Format("expected '{0}'(at index {1} in \"{2}\") but found '{3}'",
                        expected, i, _word, (char)actual));
                }
            }
            return Success(p);
        }
        public override string ToString()
        {
            return string.Format("Word({0})", _word);
        }
    }
}
