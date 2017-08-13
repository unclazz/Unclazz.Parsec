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
        public static Parser<U> Map<T, U>(this Parser<T> self, Func<T, U> transform)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (transform == null) throw new ArgumentNullException(nameof(transform));

            return null;// TODO MappedParser<T>
        }

        public static Parser<T> Repeat<T>(this Parser<T> self)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));

            return null;// TODO RepeatedParser<T>
        }

        public static Parser<T> Repeat<T>(this Parser<T> self, int min)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (min < 1) throw new ArgumentOutOfRangeException(nameof(min));

            return null;// TODO RepeatedParser<T>
        }

        public static Parser<T> RepeatJust<T>(this Parser<T> self, int times)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (times < 1) throw new ArgumentOutOfRangeException(nameof(times));

            return null;// TODO RepeatedParser<T>
        }

        public static Parser<T> Repeat<T>(this Parser<T> self, int min, int max)
        {
            if (self == null) throw new ArgumentNullException(nameof(self));
            if (min < 1) throw new ArgumentOutOfRangeException(nameof(min));
            if (max < 1) throw new ArgumentOutOfRangeException(nameof(max));

            return null;// TODO RepeatedParser<T>
        }

        public static Parser<T> Or<T>(this Parser<T> self, Parser<T> another)
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
