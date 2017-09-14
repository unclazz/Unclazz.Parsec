using System;

namespace Unclazz.Parsec.CoreParsers.RepeatAggregate
{
    sealed class AggregateConfiguration<TSource, TAccumulate, TResult>
    {
        public AggregateConfiguration(Func<TAccumulate> seedFactory,
            Func<TAccumulate, TSource, TAccumulate> accumulator,
            Func<TAccumulate, TResult> resultSelector)
        {
            if (seedFactory == null && typeof(TSource) != typeof(TAccumulate))
            {
                throw new ArgumentNullException(nameof(seedFactory));
            }
            SeedFactory = seedFactory;
            Accumulator = accumulator ?? throw new ArgumentNullException(nameof(accumulator));
            ResultSelector = resultSelector ?? throw new ArgumentNullException(nameof(resultSelector));
        }
        public AggregateConfiguration(
            Func<TAccumulate, TSource, TAccumulate> accumulator,
            Func<TAccumulate, TResult> resultSelector) : this(null, accumulator, resultSelector) { }
        public Func<TAccumulate> SeedFactory { get; }
        public Func<TAccumulate, TSource, TAccumulate> Accumulator { get; }
        public Func<TAccumulate, TResult> ResultSelector { get; }
    }
}
