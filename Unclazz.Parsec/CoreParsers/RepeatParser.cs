using System;
using System.Collections.Generic;

namespace Unclazz.Parsec.CoreParsers
{

    abstract class RepeatParser<T> : Parser<IList<T>>
    {
        public static RepeatParser<T> Create(IParser<T> parser, int min = 0, int max = -1, int exactly = -1, Parser sep = null)
        {
            // 後続処理のためexactlyだけはまず範囲チェック
            if (exactly == 0 | exactly < -1) throw new ArgumentOutOfRangeException(nameof(exactly));
            // exactlyが明示的に指定されているかチェック
            if (exactly > 0)
            {
                //　指定されている場合
                // min・maxが明示的に指定されているか（デフォルト値以外が設定されているか）をチェック
                if (min != 0 || max != -1)
                {
                    // exactlyが明示的に指定され かつ min・maxがデフォルト値でない場合
                    // 3値が整合性を持っているかチェックし、結果がNGの場合は引数の矛盾として例外スロー
                    // OKの例：　(exactly = 3, min = 3, max = 3)
                    // NGの例：　(exactly = 3, min = 2, max = 3)
                    if (exactly != min || min != max) throw new ArgumentException(
                        string.Format("conflicted arguments. exactly = {0}, but min = {1} and max = {2}.",
                        exactly, min, max));
                }
                // チェックをパスした場合のみ、RepeatExactlyParserを返す
                return new RepeatExactlyParser<T>(parser, exactly, sep);
            }
            // exactlyが明示的に指定されていない場合
            // RepeatMinMaxParserを返す
            return new RepeatMinMaxParser<T>(parser, min, max, sep);
        }



    }

    sealed class RepeatExactlyParser<T> : RepeatParser<T>
    {
        internal RepeatExactlyParser(IParser<T> original, int exactly, Parser sep)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
            if (exactly < 2) throw new ArgumentOutOfRangeException(nameof(exactly));
            _exactly = exactly;
            _sep = sep;
            _capture = typeof(T) != typeof(Nil);
        }

        readonly IParser<T> _original;
        readonly int _exactly;
        readonly Parser _sep;
        readonly bool _capture;

        public override ParseResult<IList<T>> Parse(ParserInput input)
        {
            // キャプチャ・モードの場合
            // 元のパーサーがキャプチャした内容を格納するためキューを初期化
            var list = _capture ? new List<T>() : null;
            // パース開始時の文字位置を記憶
            var pos = input.Position;
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

                // キャプチャ・モードの場合
                // 元のパーサーがキャプチャした内容をキューに追加
                if (_capture) mainResult.Capture.IfHasValue(list.Add);
            }
            // ループを無事抜けたならパースは成功
            return _capture ? Success(pos, list) : Success(pos);
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
    sealed class RepeatMinMaxParser<T> : RepeatParser<T>
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
            _capture = typeof(T) != typeof(Nil);
        }

        readonly int _min;
        readonly int _max;
        readonly IParser<T> _original;
        readonly Parser _sep;
        readonly bool _capture;

        public override ParseResult<IList<T>> Parse(ParserInput input)
        {
            // キャプチャ・モードの場合
            // 元のパーサーがキャプチャした内容を格納するためキューを初期化
            var list = _capture ? new List<T>() : null;
            // パース開始時の文字位置を記憶
            var pos = input.Position;
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

                // キャプチャ・モードの場合
                // 元のパーサーがキャプチャした内容をキューに追加
                if (_capture) r.Capture.IfHasValue(list.Add);

                // min ＜ ループ回数 ならリセットのための準備を解除
                if (_min < i) input.Unmark();
            }
            return _capture ? Success(pos, list) : Success(pos);
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
