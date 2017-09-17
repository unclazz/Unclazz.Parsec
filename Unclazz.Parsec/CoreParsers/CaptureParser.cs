using System;

namespace Unclazz.Parsec.CoreParsers
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
        /// <param name="ctx"></param>
        /// <returns>パース結果</returns>
        protected override ResultCore<string> DoParse(Context ctx)
        {
            ctx.Source.Mark();
            var r = _parse.Parse(ctx);
            if (r.Successful)
            {
                return r.Typed(ctx.Source.Capture(true));
            }
            ctx.Source.Unmark();
            return r.Typed<string>();
        }
    }
}
