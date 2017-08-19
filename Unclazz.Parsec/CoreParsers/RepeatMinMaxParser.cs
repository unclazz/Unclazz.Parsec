using System;
using System.Collections.Generic;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class RepeatMinMaxParser<T> : Parser<IEnumerable<T>>
    {
        internal RepeatMinMaxParser(IParser<T> original, int min, int max, Parser sep)
        {
            max = max == -1 ? int.MaxValue : max;
            min = min == -1 ? 0 : min;

            if (max < 1) throw new ArgumentOutOfRangeException(nameof(max));
            if (min < 0) throw new ArgumentOutOfRangeException(nameof(max));
            if (max <= min) throw new ArgumentOutOfRangeException("max <= min");

            _original = original ?? throw new ArgumentNullException(nameof(original));
            _min = min;
            _max = max;
            _sep = sep;
        }

        readonly int _min;
        readonly int _max;
        readonly IParser<T> _original;
        readonly Parser _sep;

        public override ParseResult<IEnumerable<T>> Parse(ParserInput input)
        {
            // パース開始時の文字位置を記憶
            var p = input.Position;
            // 予め指定された回数のパースを試みる
            for (var i = 1; i <= _max; i++)
            {
                // min ＜ ループ回数 ならリセットのための準備
                if (_min < i) input.Mark();

                // ループが2回目 かつ セパレーターのパーサーが指定されている場合
                if (1 < i && _sep != null)
                {
                    // セパレーターのトークンのパース
                    var sepResult = _sep.Parse(input);
                    if (!sepResult.Successful)
                    {
                        if (_min < i)
                        {
                            // min ＜ ループ回数 なら失敗とせずリセットしてループを抜ける
                            input.Reset();
                            input.Unmark();
                            break;
                        }
                        return Failure(sepResult.Position, sepResult.Message);
                    }
                }

                var r = _original.Parse(input);
                if (!r.Successful)
                {
                    if (_min < i)
                    {
                        // min ＜ ループ回数 なら失敗とせずリセットしてループを抜ける
                        input.Reset();
                        input.Unmark();
                        break;
                    }
                    return Failure(r.Position, r.Message);
                }

                // min ＜ ループ回数 ならリセットのための準備を解除
                if (_min < i) input.Unmark();
            }
            return Success(p);
        }

        public override string ToString()
        {
            if (_sep == null)
            {
                return string.Format("Repeat({0}, min = {1}, max = {2})", _original, _min, _max);
            }
            else
            {
                return string.Format("Repeat({0}, min = {1}, max = {2}, sep = {3})", _original, _min, _max, _sep);
            }
        }
    }
}
