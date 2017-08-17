using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec.Readers;

namespace Test.Unclazz.Parsec
{
    static class TestUtility
    {
        public static void ConsumesAll(ITextReader r)
        {
            while (!r.EndOfFile) r.Read();
        }
        public static void Repeats(Action act, int times)
        {
            foreach (var i in Enumerable.Range(1, times))
            {
                act();
            }
        }
        public static void Repeats<T>(Func<T> func, int times)
        {
            Repeats(() => { func(); }, times);
        }
        public static string ReadsAll(ITextReader r)
        {
            var buff = new StringBuilder();
            while (!r.EndOfFile)
            {
                buff.Append((char)r.Read());
            }
            return buff.ToString();
        }
    }
}
