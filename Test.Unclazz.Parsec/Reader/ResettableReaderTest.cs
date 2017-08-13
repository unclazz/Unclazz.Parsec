using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec;
using Unclazz.Parsec.Reader;

namespace Test.Unclazz.Parsec
{
    [TestFixture]
    public class ResettableReaderTest
    {
        [TestCase("abcdef", 0, 1, 1)]
        [TestCase("abcdef", 0, 3, 1)]
        [TestCase("abcdef", 1, 3, 1)]
        [TestCase("abcdef", 1, 4, 1)]
        [TestCase("abcdef", 0, 1, 2)]
        [TestCase("abcdef", 0, 4, 2)]
        public void Reset_Case1(string text, int willMarkOn, int willResetOn, int willBreakOn)
        {
            // Arrange
            var r = new ResettableReader(new StringReader("abcdef"));

            var markedChar = '\u0000';
            while (!r.EndOfFile)
            {
                if (willMarkOn == r.Position.Index)
                {
                    markedChar = (char)r.Peek();
                    r.Mark();
                }
                if (willResetOn == r.Position.Index)
                {
                    // Act
                    r.Reset();

                    // Assert
                    Assert.That(r.Position.Index, Is.EqualTo(willMarkOn));
                    Assert.That(r.Peek(), Is.EqualTo(markedChar));

                    if (--willBreakOn < 1) break;
                }
                r.Read();
            }
        }
        [TestCase("abcdef", 0)]
        [TestCase("abcdef", 1)]
        public void Reset_Case2(string text, int willMarkOn)
        {
            // Arrange
            var r = new ResettableReader(new StringReader("abcdef"));

            var markedChar = '\u0000';
            while (!r.EndOfFile)
            {
                if (willMarkOn == r.Position.Index)
                {
                    markedChar = (char)r.Peek();
                    r.Mark();
                }
                r.Read();
            }

            // Act
            r.Reset();

            // Assert
            Assert.That(r.Position.Index, Is.EqualTo(willMarkOn));
            Assert.That(r.Peek(), Is.EqualTo(markedChar));
        }

        [Test]
        public void Reset_Case3()
        {
            // Arrange
            var r = new ResettableReader(new StringReader("0123456789X"));

            // Act
            // Assert
            Repeats(r.Read, 2);
            Assert.That((char)r.Peek(), Is.EqualTo('2'));

            r.Mark(); // 01[2]3456789X

            Repeats(r.Read, 2);
            Assert.That((char)r.Peek(), Is.EqualTo('4'));

            r.Reset();
            Assert.That((char)r.Peek(), Is.EqualTo('2'));

            Repeats(r.Read, 2);
            Assert.That((char)r.Peek(), Is.EqualTo('4'));

            r.Reset();
            Assert.That((char) r.Peek(), Is.EqualTo('2'));

            Repeats(r.Read, 2);
            Assert.That((char)r.Peek(), Is.EqualTo('4'));

            Repeats(r.Read, 2);
            Assert.That((char)r.Peek(), Is.EqualTo('6'));

            r.Reset();
            Assert.That((char)r.Peek(), Is.EqualTo('2'));
        }

        void Repeats(Action act, int times)
        {
            foreach (var i in Enumerable.Range(1, times))
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
