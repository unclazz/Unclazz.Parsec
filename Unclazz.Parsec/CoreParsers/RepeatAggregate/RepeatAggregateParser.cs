using System;
using System.Text;

namespace Unclazz.Parsec.CoreParsers.RepeatAggregate
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

            _repMin = _repConf.Minimal;
            _repMax = _repConf.Maximum;
            _repBreakable = _repConf.Breakable;
            _repSep = _repConf.Separator;
            _aggNoSeed = _aggConf.SeedFactory == null;
        }

        readonly RepeatConfiguration _repConf;
        readonly AggregateConfiguration<TSource, TAccumulate, TResult> _aggConf;
        readonly Parser<TSource> _original;

        readonly int _repMin;
        readonly int _repMax;
        readonly bool _repBreakable;
        readonly Parser _repSep;
        readonly bool _aggNoSeed;

        protected override ResultCore<TResult> DoParse(Reader input)
        {
            // シードの有無を確認
            var acc = _aggNoSeed 
                // なしの場合、ダミー値として型のデフォルト値をアサイン
                ? default(TAccumulate)
                // ありの場合、ファクトリーで値を生成してアサイン
                : _aggConf.SeedFactory();

            // 予め指定された回数のパースを試みる
            for (var i = 1; i <= _repMax; i++)
            {
                // min ＜ ループ回数 ならリセットのための準備
                if (_repBreakable && _repMin < i) input.Mark();

                // ループが2回目 かつ セパレーターのパーサーが指定されている場合
                if (1 < i && _repSep != null)
                {
                    // セパレーターのトークンのパース
                    var sepResult = _repSep.Parse(input);
                    if (!sepResult.Successful)
                    {
                        if (_repBreakable && _repMin < i)
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
                    if (_repBreakable && _repMin < i)
                    {
                        // min ＜ ループ回数 なら失敗とせずリセットしてループを抜ける
                        input.Reset();
                        input.Unmark();
                        break;
                    }
                    return Failure(mainResult.Message);
                }

                // ループ回数のシードの有無を確認
                acc = (i == 1 && _aggNoSeed) 
                    // ループ1回目 かつ シードなし の場合、初回キャプチャをそのままアキュームレート
                    ? ((TAccumulate)((object)mainResult.Capture)) 
                    // それ以外の場合、
                    : _aggConf.Accumulator(acc, mainResult.Capture);

                // min ＜ ループ回数 ならリセットのための準備を解除
                if (_repBreakable && _repMin < i) input.Unmark();
            }
            return Success(_aggConf.ResultSelector(acc));
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

        public RepeatAggregateParser<TSource, TSource, TSource>
            ReAggregate(Func<TSource, TSource, TSource> accumulator)
        {
            return new RepeatAggregateParser<TSource, TSource, TSource>(_original, _repConf,
                new AggregateConfiguration<TSource, TSource, TSource>(accumulator, a => a));
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
    }
}
