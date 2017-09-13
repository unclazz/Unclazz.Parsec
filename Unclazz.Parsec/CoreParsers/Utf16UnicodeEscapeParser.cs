namespace Unclazz.Parsec.CoreParsers
{
    sealed class Utf16UnicodeEscapeParser : Parser<char>
    {
        readonly static Parser<char> _hex4 = new HexDigitsParser(exactly: 4).Map(a => (char)a);
        internal Utf16UnicodeEscapeParser(string prefix = "\\u", int cutIndex = -1)
        {
            _unicode = Keyword(prefix, cutIndex) & _hex4;
        }
        readonly Parser<char> _unicode;
        protected override ResultCore<char> DoParse(Reader input)
        {
            return _unicode.Parse(input);
        }
    }
}
