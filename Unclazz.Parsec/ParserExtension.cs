using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unclazz.Parsec
{
    /// <summary>
    /// <see cref="Parser{T}"/>クラスのための拡張メソッドを提供するクラスです。
    /// </summary>
    public static class ParserExtension
    {
        public static Parser<IEnumerable<T>> And<T>(this Parser<IEnumerable<T>> self, Parser<T> another, params Parser<T>[] andOthers)
        {
            Parser<IEnumerable<T>> p = new ManyThenParser<T>(self, another);
            foreach (var o in andOthers)
            {
                p = new ManyThenParser<T>(p, o);
            }
            return p;
        }
        public static Parser<U> Collect<T,U>(this Parser<IEnumerable<T>> self, Func<IEnumerable<T>, U> collector)
        {
            return new MapParser<IEnumerable<T>, U>(self, collector, false);
        }
        public static Parser<string> Concat(this Parser<string> self, 
            Parser<string> another, params Parser<string>[] andOthers)
        {
            Parser<string> tmp = new ConcatParser(self, another);
            foreach (var other in andOthers)
            {
                tmp = new ConcatParser(tmp, other);
            }
            return tmp;
        }
        public static Parser<IEnumerable<T>> Concat<T>(this Parser<IEnumerable<T>> self, Parser<IEnumerable<T>> another)
        {
            return new EnumerableConcatParser<T>(self, another);
        }
    }
}
