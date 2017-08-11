using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec;

namespace Test.Unclazz.Parsec
{
    [TestFixture]
    public class ResettableReaderTest
    {
        [TestCase("abcdef", 1, 2, 1)]
        [TestCase("abcdef", 1, 4, 1)]
        [TestCase("abcdef", 2, 4, 1)]
        [TestCase("abcdef", 2, 5, 1)]
        [TestCase("abcdef", 1, 2, 2)]
        [TestCase("abcdef", 1, 5, 2)]
        public void Reset_Case1(string arg0, int arg1, int arg2, int arg3)
        {
            // Arrange
            var r = new ResettableReader(new StringReader("abcdef"));

            var markedChar = '\u0000';
            while (!r.EndOfFile)
            {
                if (arg1 == r.Position)
                {
                    markedChar = (char)r.Peek();
                    r.Mark();
                }
                if (arg2 == r.Position)
                {
                    // Act
                    r.Reset();

                    // Assert
                    Assert.That(r.Position, Is.EqualTo(arg1));
                    Assert.That(r.Peek(), Is.EqualTo(markedChar));

                    if (--arg3 < 1) break;
                }
                r.Read();
            }
        }
        [TestCase("abcdef", 1)]
        [TestCase("abcdef", 2)]
        public void Reset_Case2(string arg0, int arg1)
        {
            // Arrange
            var r = new ResettableReader(new StringReader("abcdef"));

            var markedChar = '\u0000';
            while (!r.EndOfFile)
            {
                if (arg1 == r.Position)
                {
                    markedChar = (char)r.Peek();
                    r.Mark();
                }
                r.Read();
            }

            // Act
            r.Reset();

            // Assert
            Assert.That(r.Position, Is.EqualTo(arg1));
            Assert.That(r.Peek(), Is.EqualTo(markedChar));
        }
    }
}
