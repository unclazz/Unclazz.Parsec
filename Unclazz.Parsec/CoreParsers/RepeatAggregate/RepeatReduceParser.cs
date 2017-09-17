using System;
using System.Text;

namespace Unclazz.Parsec.CoreParsers.RepeatAggregate
{
    class RepeatReduceParser<T, U, V> : Parser<V>
    {
        internal RepeatReduceParser(Parser<T> original, RepeatConfiguration repConf,
            ReduceConfiguration<T, U, V> aggConf) 
            : this("RepeatReduce", original, repConf, aggConf) { }
        internal RepeatReduceParser(
            string name, Parser<T> original, RepeatConfiguration repConf, 
            ReduceConfiguration<T, U, V> aggConf) : base(name)
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
        readonly ReduceConfiguration<T, U, V> _aggConf;
        readonly Parser<T> _original;

        readonly int _repMin;
        readonly int _repMax;
        readonly bool _repBreakable;
        readonly Parser _repSep;
        readonly bool _aggNoSeed;

        protected override ResultCore<V> DoParse(Context ctx)
        {
            // シードの有無を確認
            var acc = _aggNoSeed 
                // なしの場合、ダミー値として型のデフォルト値をアサイン
                ? default(U)
                // ありの場合、ファクトリーで値を生成してアサイン
                : _aggConf.SeedFactory();

            // 予め指定された回数のパースを試みる
            for (var i = 1; i <= _repMax; i++)
            {
                // min ＜ ループ回数 ならリセットのための準備
                if (_repBreakable && _repMin < i) ctx.Source.Mark();

                // ループが2回目 かつ セパレーターのパーサーが指定されている場合
                if (1 < i && _repSep != null)
                {
                    // セパレーターのトークンのパース
                    var sepResult = _repSep.Parse(ctx);
                    if (!sepResult.Successful)
                    {
                        if (_repBreakable && _repMin < i)
                        {
                            // min ＜ ループ回数 なら失敗とせずリセットしてループを抜ける
                            ctx.Source.Reset(true);
                            break;
                        }
                        return Failure(sepResult.Message);
                    }
                }

                var mainResult = _original.Parse(ctx);
                if (!mainResult.Successful)
                {
                    if (_repBreakable && _repMin < i)
                    {
                        // min ＜ ループ回数 なら失敗とせずリセットしてループを抜ける
                        ctx.Source.Reset(true);
                        break;
                    }
                    return Failure(mainResult.Message);
                }

                // ループ回数のシードの有無を確認
                acc = (i == 1 && _aggNoSeed) 
                    // ループ1回目 かつ シードなし の場合、初回キャプチャをそのままアキュームレート
                    ? ((U)((object)mainResult.Capture)) 
                    // それ以外の場合、
                    : _aggConf.Accumulator(acc, mainResult.Capture);

                // min ＜ ループ回数 ならリセットのための準備を解除
                if (_repBreakable && _repMin < i) ctx.Source.Unmark();
            }
            return Success(_aggConf.ResultSelector(acc));
        }
        public RepeatReduceParser<T, T, T> ReReduce(Func<T, T, T> accumulator)
        {
            return new RepeatReduceParser<T, T, T>(_original, _repConf,
                new ReduceConfiguration<T, T, T>(accumulator, a => a));
        }
        public RepeatReduceParser<T, U2, U2> ReReduce<U2>(
            Func<U2> seedFactory, Func<U2, T, U2> accumulator)
        {
            return new RepeatReduceParser<T, U2, U2>(_original, _repConf,
                new ReduceConfiguration<T, U2, U2>(seedFactory, accumulator, a => a));
        }
        public RepeatReduceParser<T, U2, V2> ReReduce<U2, V2>(
            Func<U2> seedFactory, Func<U2, T, U2> accumulator,
            Func<U2, V2> resultSelector)
        {
            return new RepeatReduceParser<T, U2, V2> (_original, _repConf,
                new ReduceConfiguration<T, U2, V2>(seedFactory, accumulator, resultSelector));
        }
    }
}
