using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec.CharClass;

namespace Unclazz.Parsec
{
    public static class ParserExtension
    {
        public static IParser<U> Map<T, U>(this IParser<T> self, Func<T, U> transform)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (transform == null) throw new ArgumentNullException(nameof(transform));

            return null;// TODO MappedParser<T>
        }

        public static IParser<T> Repeat<T>(this IParser<T> self)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            return null;// TODO RepeatedParser<T>
        }

        public static IParser<T> Repeat<T>(this IParser<T> self, int min)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (min < 1) throw new ArgumentOutOfRangeException(nameof(min));

            return null;// TODO RepeatedParser<T>
        }

        public static IParser<T> RepeatJust<T>(this IParser<T> self, int times)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (times < 1) throw new ArgumentOutOfRangeException(nameof(times));

            return null;// TODO RepeatedParser<T>
        }

        public static IParser<T> Repeat<T>(this IParser<T> self, int min, int max)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (min < 1) throw new ArgumentOutOfRangeException(nameof(min));
            if (max < 1) throw new ArgumentOutOfRangeException(nameof(max));

            return null;// TODO RepeatedParser<T>
        }

        public static IParser<T> Or<T>(this IParser<T> self, IParser<T> another)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (another == null) throw new ArgumentNullException(nameof(another));

            return null;// TODO OrParser<T>
        }

        public static CharRange To(this char self, char another)
        {
            return CharRange.Between(self, another);
        }
    }
}
