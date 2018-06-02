using System;

namespace Unclazz.Parsec.Intrinsics
{
    sealed class CaptureParser : Parser<string>
    {
        public CaptureParser(Parser parse) : base("Capture")
        {
            _parse = parse ?? throw new ArgumentNullException(nameof(parse));
        }

        readonly Parser _parse;

        /// <summary>
        /// パースを行います。
        /// </summary>
        /// <param name="src"></param>
        /// <returns>パース結果</returns>
        protected override ResultCore<string> DoParse(Reader src)
        {
            src.Mark();
            var r = _parse.Parse(src);
            if (r.Successful)
            {
                return r.Typed(src.Capture(true));
            }
            src.Unmark();
            return r.Typed<string>();
        }
    }
}
