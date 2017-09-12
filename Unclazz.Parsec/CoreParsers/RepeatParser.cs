using System;
using System.Collections.Generic;
using System.Text;

namespace Unclazz.Parsec.CoreParsers
{
    class RepeatAggregateParser<TSource, TAccumulate, TResult> : Parser<TResult>
    {
        internal RepeatAggregateParser(Parser<TSource> original, RepeatConfiguration repConf,
            AggregateConfiguration<TSource, TAccumulate, TResult> aggConf) 
            : this(original.Configuration, original, repConf, aggConf) { }
        internal RepeatAggregateParser(
            IParserConfiguration conf, Parser<TSource> original, RepeatConfiguration repConf, 
            AggregateConfiguration<TSource, TAccumulate, TResult> aggConf) : base(conf)
        {
            _repConf = repConf ?? throw new ArgumentNullException(nameof(repConf));
            _aggConf = aggConf ?? throw new ArgumentNullException(nameof(aggConf));
            _original = original ?? throw new ArgumentNullException(nameof(original));
            _capture = original.GetType() != typeof(TypedParser<TResult>);
        }

        readonly RepeatConfiguration _repConf;
        readonly AggregateConfiguration<TSource, TAccumulate, TResult> _aggConf;
        readonly Parser<TSource> _original;
        readonly bool _capture;

        protected override ResultCore<TResult> DoParse(Reader input)
        {
            // キャプチャ・モードの場合
            // 元のパーサーがキャプチャした内容を格納するためキューを初期化
            var acc = _capture ? _aggConf.SeedFactory() : default(TAccumulate);

            // 予め指定された回数のパースを試みる
            var max = _repConf.Maximum;
            var min = _repConf.Minimal;
            var breakable = _repConf.Breakable;
            var sep = _repConf.Separator;

            for (var i = 1; i <= max; i++)
            {
                // min ＜ ループ回数 ならリセットのための準備
                if (breakable && min < i) input.Mark();

                // ループが2回目 かつ セパレーターのパーサーが指定されている場合
                if (1 < i && sep != null)
                {
                    // セパレーターのトークンのパース
                    var sepResult = sep.Parse(input);
                    if (!sepResult.Successful)
                    {
                        if (breakable && min < i)
                        {
                            // min ＜ ループ回数 なら失敗とせずリセットしてループを抜ける
                            input.Reset();
                            input.Unmark();
                            break;
                        }
                        return Failure(sepResult.Message);
                    }
                }

                var mainResult = _original.Parse(input);
                if (!mainResult.Successful)
                {
                    if (breakable && min < i)
                    {
                        // min ＜ ループ回数 なら失敗とせずリセットしてループを抜ける
                        input.Reset();
                        input.Unmark();
                        break;
                    }
                    return Failure(mainResult.Message);
                }

                // キャプチャ・モードの場合
                // 元のパーサーがキャプチャした内容をキューに追加
                if (_capture)  acc = _aggConf.Accumulator(acc, mainResult.Capture);

                // min ＜ ループ回数 ならリセットのための準備を解除
                if (breakable && min < i) input.Unmark();
            }
            return Success(_capture ? _aggConf.ResultSelector(acc) : default(TResult));
        }

        public override string ToString()
        {
            if (_repConf.Maximum == _repConf.Minimal)
            {
                if (_repConf.Separator == null)
                {
                    return string.Format("Repeat({0}, exactly = {1})", 
                        _original, _repConf.Maximum);
                }
                else
                {
                    return string.Format("Repeat({0}, exactly = {1}, sep = {2})", 
                        _original, _repConf.Maximum, _repConf.Separator);
                }
            }
            else
            {
                if (_repConf.Separator == null)
                {
                    return string.Format("Repeat({0}, min = {1}, max = {2})", 
                        _original, _repConf.Minimal, _repConf.Maximum);
                }
                else
                {
                    return string.Format("Repeat({0}, min = {1}, max = {2}, sep = {3})", 
                        _original, _repConf.Minimal, _repConf.Maximum, _repConf.Separator);
                }
            }
        }

        public RepeatAggregateParser<TSource, UAccumulate, UResult>
            ReAggregate<UAccumulate, UResult>(
            Func<UAccumulate> seedFactory,
            Func<UAccumulate, TSource, UAccumulate> accumulator,
            Func<UAccumulate, UResult> resultSelector)
        {
            return new RepeatAggregateParser<TSource, UAccumulate, UResult>
                (_original, _repConf,
                new AggregateConfiguration<TSource, UAccumulate, UResult>
                (seedFactory, accumulator, resultSelector));
        }
        public RepeatAggregateParser<TSource, UAccumulate, UAccumulate>
            ReAggregate<UAccumulate>(
            Func<UAccumulate> seedFactory,
            Func<UAccumulate, TSource, UAccumulate> accumulator)
        {
            return new RepeatAggregateParser<TSource, UAccumulate, UAccumulate>
                (_original, _repConf,
                new AggregateConfiguration<TSource, UAccumulate, UAccumulate>
                (seedFactory, accumulator, a => a));
        }
    }
    sealed class AggregateConfiguration<TSource,TAccumulate,TResult>
    {
        public AggregateConfiguration(Func<TAccumulate> seedFactory,
            Func<TAccumulate, TSource, TAccumulate> accumulator,
            Func<TAccumulate, TResult> resultSelector)
        {
            SeedFactory = seedFactory ?? throw new ArgumentNullException(nameof(seedFactory));
            Accumulator = accumulator ?? throw new ArgumentNullException(nameof(accumulator));
            ResultSelector = resultSelector ?? throw new ArgumentNullException(nameof(resultSelector));
        }
        public Func<TAccumulate> SeedFactory { get; }
        public Func<TAccumulate, TSource, TAccumulate> Accumulator { get; }
        public Func<TAccumulate, TResult> ResultSelector { get; }
    }
    sealed class RepeatConfiguration
    {
        public RepeatConfiguration(int min = 0, int max = -1, int exactly = -1, Parser sep = null)
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
                        string.Format("arguments conflict. exactly = {0}, but min = {1} and max = {2}.",
                        exactly, min, max));
                }
                Minimal = exactly;
                Maximum = exactly;
                Breakable = false;
            }
            else
            {
                max = max == -1 ? int.MaxValue : max;
                min = min == -1 ? 0 : min;

                if (max < 1) throw new ArgumentOutOfRangeException(nameof(max));
                if (min < 0) throw new ArgumentOutOfRangeException(nameof(max));
                if (max <= min) throw new ArgumentOutOfRangeException("max <= min");

                Minimal = min;
                Maximum = max;
                Breakable = min != max;
            }
            Separator = sep;
        }
        public int Minimal { get; }
        public int Maximum { get; }
        public bool Breakable { get; }
        public Parser Separator { get; }
    }
    sealed class SeqParser<TSource> : RepeatAggregateParser<TSource, IList<TSource>, Seq<TSource>>
    {
        readonly static AggregateConfiguration<TSource, IList<TSource>, Seq<TSource>> aggConf
            = new AggregateConfiguration<TSource, IList<TSource>, Seq<TSource>>
            (() => new List<TSource>(), (a, b) => { a.Add(b); return a; }, a => Seq<TSource>.Of(a));

        internal SeqParser(Parser<TSource> original, RepeatConfiguration repConf)
            : base(original.Configuration, original, repConf, aggConf) { }
    }
    sealed class CountParser<TSource> : RepeatAggregateParser<TSource, int, int>
    {
        readonly static AggregateConfiguration<TSource, int, int> aggConf
            = new AggregateConfiguration<TSource, int, int>(() => 0, (a, b) => a + 1, a => a);

        internal CountParser(Parser<TSource> original, RepeatConfiguration repConf)
            : base(original.Configuration, original, repConf, aggConf) { }
    }
    sealed class StringParser : RepeatAggregateParser<char, StringBuilder, string>
    {
        readonly static AggregateConfiguration<char, StringBuilder, string> aggConf
            = new AggregateConfiguration<char, StringBuilder, string>
            (() => new StringBuilder(), (a, b) => a.Append(b), a => a.ToString());

        internal StringParser(Parser<char> original, RepeatConfiguration repConf)
            : base(original.Configuration, original, repConf, aggConf) { }
    }
}
