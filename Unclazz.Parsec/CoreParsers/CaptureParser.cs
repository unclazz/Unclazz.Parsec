using System;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class CaptureParser : Parser<string>
    {
        public CaptureParser(IParserConfiguration conf, Parser parse) : base(conf)
        {
            _parse = parse ?? throw new ArgumentNullException(nameof(parse));
        }

        readonly Parser _parse;

        /// <summary>
        /// パースを行います。
        /// </summary>
        /// <param name="input">入力データ</param>
        /// <returns>パース結果</returns>
        protected override ResultCore<string> DoParse(Reader input)
        {
            input.Mark();
            var r = _parse.Parse(input);
            if (r.Successful)
            {
                return r.Typed(input.Capture(true));
            }
            input.Unmark();
            return r.Typed<string>();
        }
        /// <summary>
        /// このパーサーの文字列表現を返します。
        /// </summary>
        /// <returns>文字列表現</returns>
        public override string ToString()
        {
            return string.Format("Capture({0})", _parse);
        }
    }
}
