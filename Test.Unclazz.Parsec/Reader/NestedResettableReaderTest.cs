using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec.Reader;

namespace Test.Unclazz.Parsec.Reader
{
    [TestFixture]
    public class NestedResettableReaderTest
    {
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void Nest_Case1(int nestCount)
        {
            // Arrange
            var r = new NestedResettableReader(new StringReader("abc"));
            Repeats(r.Nest, nestCount);

            // Act
            // Assert
            Repeats(r.Unnest, nestCount);
            Assert.That(() => r.Unnest(), Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public void Nest_Case2()
        {
            // Arrange
            var r = new NestedResettableReader(new StringReader("0123456789X"));

            // Act
            // Assert
            Repeats(r.Read, 2);
            Assert.That((char)r.Peek(), Is.EqualTo('2'));

            r.Mark(); // Marked at 2, _, _
            r.Nest(); // Nest level 0 -> 1.

            Repeats(r.Read, 2); // 0123[4]56789X
            Assert.That((char)r.Peek(), Is.EqualTo('4'));

            r.Mark(); // Marked at 2, 4, _

            Repeats(r.Read, 2);
            Assert.That((char)r.Peek(), Is.EqualTo('6'));

            r.Reset();
            Assert.That((char)r.Peek(), Is.EqualTo('4'));

            Repeats(r.Read, 2);
            Assert.That((char)r.Peek(), Is.EqualTo('6'));

            r.Nest(); // Nest level 1 -> 2.

            Repeats(r.Read, 2);
            Assert.That((char)r.Peek(), Is.EqualTo('8'));

            r.Mark(); // Marked at 2, 4, 8

            Repeats(r.Read, 2);
            Assert.That((char)r.Peek(), Is.EqualTo('X'));

            r.Reset();
            Assert.That((char)r.Peek(), Is.EqualTo('8'));

            r.Unnest(); // Nest level 2 -> 1. 
            Assert.That((char)r.Peek(), Is.EqualTo('8'));

            r.Reset();
            Assert.That((char)r.Peek(), Is.EqualTo('4'));

            r.Unnest(); // Nest level 1 -> 0. 
            Assert.That((char)r.Peek(), Is.EqualTo('4'));

            r.Reset();
            Assert.That((char)r.Peek(), Is.EqualTo('2'));
        }

        void Repeats(Action act, int times)
        {
            if (times < 1) return;
            foreach(var i in Enumerable.Range(1, times))
            {
                act();
            }
        }
        void Repeats<T>(Func<T> func, int times)
        {
            Repeats(() => { func(); }, times);
        }
    }
}
