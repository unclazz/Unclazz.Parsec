﻿namespace Unclazz.Parsec.CoreParsers
{
    sealed class QuotedStringParser : Parser<string>
    {
        internal QuotedStringParser(char quote = '"', Parser<char> escape = null) : base("QuotedString")
        {
            var q = Char(quote);
            var nq = CharIn(!CharClass.Exact(quote)).Capture();
            if (escape == null) _string = q & (nq).Repeat().Join() & q;
            else _string = q & (escape | nq).Repeat().Join() & q;
        }
        readonly Parser<string> _string;
        protected override ResultCore<string> DoParse(Context ctx)
        {
            return _string.Parse(ctx);
        }
    }
}
