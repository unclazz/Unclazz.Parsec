using System;
using System.Collections.Generic;

namespace Unclazz.Parsec
{
    sealed class RepeatExactlyParser<T> : Parser<IEnumerable<T>>
    {
        internal RepeatExactlyParser(Parser<T> original, int exactly, Parser<string> sep)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
            if (exactly < 2) throw new ArgumentOutOfRangeException(nameof(exactly));
            _exactly = exactly;
            _sep = sep;
        }

        readonly Parser<T> _original;
        readonly int _exactly;
        readonly Parser<string> _sep;

        public override ParseResult<IEnumerable<T>> Parse(ParserInput input)
        {
            // パース開始時の文字位置を記憶
            var p = input.Position;
            // 予め指定された回数のパースを試みる
            for (var i = 0; i < _exactly; i++)
            {
                // ループが2回目 かつ セパレーターのパーサーが指定されている場合
                if (0 < i && _sep != null)
                {
                    // セパレーターのトークンのパース
                    var sepResult = _sep.Parse(input);
                    if (!sepResult.Successful)
                    {
                        return Failure(sepResult.Position, sepResult.Message);
                    }
                }

                // 主目的のトークンのパース
                var mainResult = _original.Parse(input);
                // 失敗の場合はそこでパースを中断
                if (!mainResult.Successful)
                {
                    return Failure(mainResult.Position, mainResult.Message);
                }
            }
            // ループを無事抜けたならパースは成功
            return Success(p);
        }
        public override string ToString()
        {
            if (_sep == null)
            {
                return string.Format("Repeat({0}, exactly = {1})", _original, _exactly);
            }
            else
            {
                return string.Format("Repeat({0}, exactly = {1}, sep = {2})", _original, _exactly, _sep);
            }
        }
    }
}
