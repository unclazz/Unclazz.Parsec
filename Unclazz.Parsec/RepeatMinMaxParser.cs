using System;
using System.Collections.Generic;

namespace Unclazz.Parsec
{
    sealed class RepeatMinMaxParser<T> : Parser<IEnumerable<T>>
    {
        internal RepeatMinMaxParser(Parser<T> original, int min, int max)
        {
            max = max == -1 ? int.MaxValue : max;
            min = min == -1 ? 0 : min;

            if (max < 1) throw new ArgumentOutOfRangeException(nameof(max));
            if (min < 0) throw new ArgumentOutOfRangeException(nameof(max));
            if (max <= min) throw new ArgumentException("max <= min");

            _original = original ?? throw new ArgumentNullException(nameof(original));
            _min = min;
            _max = max;
        }

        readonly int _min;
        readonly int _max;
        readonly Parser<T> _original;

        public override ParseResult<IEnumerable<T>> Parse(ParserInput input)
        {
            var p = input.Position;
            var q = new Queue<T>();
            for (var i = 1; i <= _max && !input.EndOfFile; i++)
            {
                // min ＜ ループ回数 ならリセットのための準備
                if (_min <= i) input.Mark();

                var r = _original.Parse(input);
                if (r.Successful)
                {
                    r.Capture.IfHasValue(q.Enqueue);
                }
                else
                {
                    if (_min < i)
                    {
                        // min ＜ ループ回数 なら失敗とせずリセットしてループを抜ける
                        input.Reset();
                        input.Unmark();
                        break;
                    }
                    return ParseResult.OfFailure<IEnumerable<T>>(r.Position, r.Message);
                }

                // min ＜ ループ回数 ならリセットのための準備を解除
                if (_min <= i) input.Unmark();
            }
            return ParseResult.OfSuccess<IEnumerable<T>>(p, q);
        }
    }
}
