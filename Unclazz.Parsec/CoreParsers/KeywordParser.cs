using System;
using System.Collections.Generic;
using System.Text;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class KeywordParser : Parser
    {
        internal KeywordParser(string keyword) : this(ParserFactory.Default, keyword) { }
        internal KeywordParser(IParserConfiguration conf, string keyword) : this(conf, keyword, -1) { }
        internal KeywordParser(IParserConfiguration conf, string keyword, int cutIndex) : base(conf)
        {
            if (cutIndex < -1) throw new ArgumentOutOfRangeException(nameof(cutIndex));
            _cut = cutIndex;
            _keyword = keyword ?? throw new ArgumentNullException(nameof(keyword));
            if (keyword.Length == 0) throw new ArgumentException
                    ("length of keyword is must be greater than 0.");
            if (cutIndex == 0 || keyword.Length < cutIndex) throw new ArgumentOutOfRangeException(nameof(cutIndex));
            if (keyword.Length == cutIndex) _cut = -1;
        }

        readonly int _cut;
        readonly string _keyword;

        protected override ResultCore DoParse(Reader input)
        {
            for (var i = 0; i < _keyword.Length; i++)
            {
                var expected = _keyword[i];
                var actual = input.Read();
                if (expected != actual)
                {
                    return Failure(string.Format("expected {0} but found {1} at index {2} in \"{3}\"",
                        ParsecUtility.CharToString(expected), ParsecUtility.CharToString(actual), i, _keyword),
                        -1 == _cut || i < _cut);
                }
            }
            return Success();
        }
        public override string ToString()
        {
            if (_cut == -1) return string.Format("Keyword({0})", _keyword);
            return string.Format("Keyword({0}, cutIndex = {1})", _keyword, _cut);
        }
    }
}
