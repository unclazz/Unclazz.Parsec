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
    public class PrefixedReaderTest
    {
        [TestCase("", "abc", 'a', 'b')]
        [TestCase("a", "bcd", 'a', 'b')]
        [TestCase("def", "", 'd', 'e')]
        [TestCase("de", "f", 'd', 'e')]
        [TestCase("", "", -1, -1)]
        public void Read_Case1(string arg0, string arg1, int arg2, int arg3)
        {
            // Arrange
            var r = new PrefixedReader(1, 1, 1, new Queue<char>(arg0), new StringReader(arg1));

            // Act
            var ch0 = r.Read();
            var ch1 = r.Read();

            // Assert
            Assert.That(ch0, Is.EqualTo(arg2));
            Assert.That(ch1, Is.EqualTo(arg3));
        }
        [TestCase("", "abc", 'a', 'b')]
        [TestCase("a", "bcd", 'a', 'b')]
        [TestCase("def", "", 'd', 'e')]
        [TestCase("de", "f", 'd', 'e')]
        [TestCase("", "", -1, -1)]
        public void Peek_Case1(string arg0, string arg1, int arg2, int arg3)
        {
            // Arrange
            var r = new PrefixedReader(1, 1, 1, new Queue<char>(arg0), new StringReader(arg1));

            // Act
            var ch0 = r.Peek();
            r.Read();
            var ch1 = r.Peek();

            // Assert
            Assert.That(ch0, Is.EqualTo(arg2));
            Assert.That(ch1, Is.EqualTo(arg3));
        }
        [TestCase("", "abc", 2)]
        [TestCase("a", "bcd", 2)]
        [TestCase("def", "", 2)]
        [TestCase("de", "f", 2)]
        [TestCase("", "", 1)]
        public void Position_Case1(string arg0, string arg1, int arg2)
        {
            // Arrange
            var r = new PrefixedReader(1, 1, 1, new Queue<char>(arg0), new StringReader(arg1));

            // Act
            var p0 = r.Position;
            var ch = r.Read();
            var p1 = r.Position;

            // Assert
            Assert.That(p0, Is.EqualTo(1));
            Assert.That(p1, Is.EqualTo(arg2));
        }
        [TestCase("", "abc", 4)]
        [TestCase("a", "bcd", 5)]
        [TestCase("def", "", 4)]
        [TestCase("de", "f", 4)]
        [TestCase("", "", 1)]
        public void Position_Case2(string arg0, string arg1, int arg2)
        {
            // Arrange
            var r = new PrefixedReader(1, 1, 1, new Queue<char>(arg0), new StringReader(arg1));
            ConsumesAll(r);

            // Act
            var p1 = r.Position;

            // Assert
            Assert.That(p1, Is.EqualTo(arg2));
        }
        [TestCase("", "ab\rcd")]
        [TestCase("a", "b\ncd")]
        [TestCase("de\rfg", "")]
        [TestCase("de\n", "fg")]
        [TestCase("de", "\nfg")]
        [TestCase("de", "\n")]
        public void LinePosition_Case1(string arg0, string arg1)
        {
            // Arrange
            var r = new PrefixedReader(1, 1, 1, new Queue<char>(arg0), new StringReader(arg1));
            r.Read();
            r.Read();

            // Act
            var l0 = r.LinePosition;
            r.Read();
            var l1 = r.LinePosition;

            // Assert
            Assert.That(l0, Is.EqualTo(1));
            Assert.That(l1, Is.EqualTo(2));
        }
        [TestCase("", "ab\r\ncd")]
        [TestCase("a", "b\r\ncd")]
        [TestCase("de\r\nfg", "")]
        [TestCase("de\r", "\nfg")]
        public void LinePosition_Case2(string arg0, string arg1)
        {
            // Arrange
            var r = new PrefixedReader(1, 1, 1, new Queue<char>(arg0), new StringReader(arg1));
            r.Read();
            r.Read();

            // Act
            var l0 = r.LinePosition;
            r.Read();
            var l1 = r.LinePosition;
            r.Read();
            var l2 = r.LinePosition;

            // Assert
            Assert.That(l0, Is.EqualTo(1));
            Assert.That(l1, Is.EqualTo(1));
            Assert.That(l2, Is.EqualTo(2));
        }
        [TestCase("", "ab\rcd")]
        [TestCase("a", "b\ncd")]
        [TestCase("de\rfg", "")]
        [TestCase("de\n", "fg")]
        [TestCase("de", "\nfg")]
        [TestCase("de", "\n")]
        public void ColumnPosition_Case1(string arg0, string arg1)
        {
            // Arrange
            var r = new PrefixedReader(1, 1, 1, new Queue<char>(arg0), new StringReader(arg1));
            r.Read();
            r.Read();

            // Act
            var c0 = r.ColumnPosition;
            r.Read();
            var c1 = r.ColumnPosition;

            // Assert
            Assert.That(c0, Is.EqualTo(3));
            Assert.That(c1, Is.EqualTo(1));
        }
        [TestCase("", "abc")]
        [TestCase("a", "bc")]
        [TestCase("ab", "c")]
        [TestCase("abc", "")]
        public void Reattach_Case1(string arg0, string arg1)
        {
            // Arrange
            var r = new PrefixedReader(1, 1, 1, new Queue<char>(arg0), new StringReader(arg1));
            r.Read();

            // Act
            r.Reattach(1, 1, 1, new Queue<char>(arg0));
            var p = r.Position;
            var l = r.LinePosition;
            var c = r.ColumnPosition;
            var ch = r.Peek();

            // Assert
            Assert.That(p, Is.EqualTo(1));
            Assert.That(l, Is.EqualTo(1));
            Assert.That(c, Is.EqualTo(1));
            Assert.That(ch, Is.EqualTo(arg0.Length == 0 ? arg1[1] : (arg0 + arg1)[0]));
        }

        void ConsumesAll(PrefixedReader r)
        {
            while (!r.EndOfFile) r.Read();
        }
    }
}
