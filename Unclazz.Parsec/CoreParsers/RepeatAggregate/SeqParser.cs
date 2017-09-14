using System.Collections.Generic;

namespace Unclazz.Parsec.CoreParsers.RepeatAggregate
{
    sealed class SeqParser<TSource> : RepeatAggregateParser<TSource, IList<TSource>, Seq<TSource>>
    {
        readonly static AggregateConfiguration<TSource, IList<TSource>, Seq<TSource>> aggConf
            = new AggregateConfiguration<TSource, IList<TSource>, Seq<TSource>>
            (() => new List<TSource>(), (a, b) => { a.Add(b); return a; }, a => Seq<TSource>.Of(a));

        internal SeqParser(Parser<TSource> original, RepeatConfiguration repConf)
            : base(original.Configuration, original, repConf, aggConf) { }
    }
}
