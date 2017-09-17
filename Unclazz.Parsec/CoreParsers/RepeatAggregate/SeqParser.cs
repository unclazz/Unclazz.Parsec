using System.Collections.Generic;

namespace Unclazz.Parsec.CoreParsers.RepeatAggregate
{
    sealed class SeqParser<T> : RepeatReduceParser<T, IList<T>, Seq<T>>
    {
        readonly static ReduceConfiguration<T, IList<T>, Seq<T>> aggConf
            = new ReduceConfiguration<T, IList<T>, Seq<T>>(SeedFactory, Accumulate, Seq<T>.Of);
        static IList<T> SeedFactory() => new List<T>();
        static IList<T> Accumulate(IList<T> a, T b)
        {
            a.Add(b);
            return a;
        }
        internal SeqParser(Parser<T> original, RepeatConfiguration repConf)
            : base("Seq", original, repConf, aggConf) { }
    }
}
