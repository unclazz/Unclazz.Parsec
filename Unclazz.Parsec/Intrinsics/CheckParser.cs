using System;
namespace Unclazz.Parsec.Intrinsics
{
    sealed class CheckParser<T> : Parser<T>
    {
        internal CheckParser(Parser<T> parser, Func<T, bool> check, string message)
        {
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _check = check ?? throw new ArgumentNullException(nameof(check));
            _message = message ?? throw new ArgumentNullException(nameof(message));
        }

        readonly Parser<T> _parser;
        readonly Func<T, bool> _check;
        readonly string _message;

        protected override ResultCore<T> DoParse(Reader src)
        {
            // 元になったパーサーのパース結果を得る。
            var result = _parser.Parse(src);

            // パース成功の場合、チェックを実施する。
            // その結果がNGならこのパーサーのパース結果も失敗とする。
            // それ以外の場合（元のパーサーのパース結果が成功でチェック結果もOKの場合、
            // もしくは、元のパーサーのパース結果が失敗の場合）は元のパーサーのパース結果をそのまま返す。
            return result.Successful && !_check(result.Capture) ? Failure(_message) : result;
        }
    }
    sealed class CheckParser : Parser<string>
    {
        internal CheckParser(Parser parser, Func<string, bool> check, string message)
        {
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _check = check ?? throw new ArgumentNullException(nameof(check));
            _message = message ?? throw new ArgumentNullException(nameof(message));
        }

        readonly Parser _parser;
        readonly Func<string, bool> _check;
        readonly string _message;

        protected override ResultCore<string> DoParse(Reader src)
        {
            src.Mark();
            var r = _parser.Parse(src);
            if (r.Successful)
            {
                var value = src.Capture(true);
                if (_check(value))
                {
                    return r.Typed(value);
                }
                else
                {
                    return Failure(_message);
                }
            }
            src.Unmark();
            return r.Typed<string>();
        }
    }
}
