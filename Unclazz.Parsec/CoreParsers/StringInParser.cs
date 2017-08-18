using System;
using System.Linq;

namespace Unclazz.Parsec
{
    sealed class StringInParser : Parser<string>
    {
        static int CommonPrefixLength(string keyword0, string keyword1)
        {
            return keyword0.Zip(keyword1,
                    (prevChar, currChar) => prevChar == currChar)
                    .TakeWhile(a => a)
                    .Count();
        }
        static Parser<string> KeywordsParser(string[] keywords)
        {
            Parser<string> parser = null;
            var zip = keywords.Zip(keywords.Skip(1), (k0, k1) =>
            new { Keyword = k0, CommonPrefixLength = CommonPrefixLength(k0, k1) });
            foreach (var zipElem in zip)
            {
                var delta = zipElem.Keyword.Length - zipElem.CommonPrefixLength;
                var cutIndex = Math.Min(zipElem.Keyword.Length, zipElem.CommonPrefixLength + 1);
                var nextParser = new KeywordParser(zipElem.Keyword, cutIndex);
                if (parser == null) parser = nextParser;
                else parser = parser.Or(nextParser);
            }
            if (parser == null) parser = new KeywordParser(keywords[0]);
            else parser = parser.Or(new KeywordParser(keywords[keywords.Length - 1]));
            return parser;
        }

        internal StringInParser(string[] keywords)
        {
            var tmp = keywords ?? throw new ArgumentNullException(nameof(keywords));
            if (tmp.Length == 0) throw new ArgumentException(nameof(keywords) + " must not be empty.");
            if (tmp.Any(k => k == null)) throw new ArgumentException(nameof(keywords) + " must not contain null.");
            if (tmp.Any(k => k.Length == 0)) throw new ArgumentException(nameof(keywords) + " must not contain empty string.");
            tmp = tmp.OrderBy(k => k).Distinct().ToArray();
            _keywords = tmp;
            _parser = KeywordsParser(tmp);

        }
        readonly string[] _keywords;
        readonly Parser<string> _parser;
        public override ParseResult<string> Parse(ParserInput input)
        {
            return _parser.Parse(input);
        }
        public override string ToString()
        {
            return string.Format("StringIn({0})", 
                ParsecUtility.ValueToString(_keywords));
        }
    }
}
