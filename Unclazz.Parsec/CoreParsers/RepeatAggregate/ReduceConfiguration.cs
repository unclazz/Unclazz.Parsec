using System;

namespace Unclazz.Parsec.CoreParsers.RepeatAggregate
{
    sealed class ReduceConfiguration<T, U, V>
    {
        public ReduceConfiguration(Func<U> seedFactory,
            Func<U, T, U> accumulator,
            Func<U, V> resultSelector)
        {
            if (seedFactory == null && typeof(T) != typeof(U))
            {
                throw new ArgumentNullException(nameof(seedFactory));
            }
            SeedFactory = seedFactory;
            Accumulator = accumulator ?? throw new ArgumentNullException(nameof(accumulator));
            ResultSelector = resultSelector ?? throw new ArgumentNullException(nameof(resultSelector));
        }
        public ReduceConfiguration(Func<U, T, U> accumulator, Func<U, V> resultSelector) : this(null, accumulator, resultSelector) { }
        public Func<U> SeedFactory { get; }
        public Func<U, T, U> Accumulator { get; }
        public Func<U, V> ResultSelector { get; }
    }
}
