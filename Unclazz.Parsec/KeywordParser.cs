using System;
using System.Text;

namespace Unclazz.Parsec
{
    sealed class KeywordParser : Parser<string>
    {
        internal KeywordParser(string keyword)
        {
            _keyword = keyword ?? throw new ArgumentNullException(nameof(keyword));
            if (keyword.Length == 0) throw new ArgumentException
                    ("length of keyword is must be greater than 0.");
        }

        readonly string _keyword;

        public override ParseResult<string> Parse(ParserInput input)
        {
            var p = input.Position;
            for (var i = 0; i < _keyword.Length; i++)
            {
                var expected = _keyword[i];
                var actual = input.Read();
                if (expected != actual)
                {
                    return Failure(input.Position,
                        string.Format("expected {0} but found {1} at index {1} in \"{2}\"",
                        ParsecUtility.CharToString(expected), ParsecUtility.CharToString(actual), i, _keyword));
                }
            }
            return Success(p);
        }
        public override string ToString()
        {
            return string.Format("Keyword({0})", _keyword);
        }
    }
}
