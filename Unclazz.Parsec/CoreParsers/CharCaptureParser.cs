namespace Unclazz.Parsec.CoreParsers
{
    sealed class CharCaptureParser : Parser<char>
    {
        internal CharCaptureParser(CharParser original) : base(original.Configuration)
        {
            _original = original;
        }
        readonly CharParser _original;
        protected override ResultCore<char> DoParse(Reader input)
        {
            var ch = input.Peek();
            var res = _original.Parse(input);
            return res.Successful ? Success((char)ch) : Failure(res.Message);
        }
        public override string ToString()
        {
            return string.Format("Capture({0})", _original);
        }
    }
}
