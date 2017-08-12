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
    public class PrependableReaderTest
    {
        [TestCase("", "abc", 'a', 'b')]
        [TestCase("a", "bcd", 'a', 'b')]
        [TestCase("def", "", 'd', 'e')]
        [TestCase("de", "f", 'd', 'e')]
        [TestCase("", "", -1, -1)]
        public void Read_Case1(string prefixText, string readerText, int firstChar, int secondChar)
        {
            // Arrange
            var r = new PrependableReader(CharacterPosition.StartOfFile, new Queue<char>(prefixText), new StringReader(readerText));

            // Act
            var ch0 = r.Read();
            var ch1 = r.Read();

            // Assert
            Assert.That(ch0, Is.EqualTo(firstChar));
            Assert.That(ch1, Is.EqualTo(secondChar));
        }
        [TestCase("", "abc", 'a', 'b')]
        [TestCase("a", "bcd", 'a', 'b')]
        [TestCase("def", "", 'd', 'e')]
        [TestCase("de", "f", 'd', 'e')]
        [TestCase("", "", -1, -1)]
        public void Peek_Case1(string prefixText, string readerText, int firstChar, int secondChar)
        {
            // Arrange
            var r = new PrependableReader(CharacterPosition.StartOfFile, new Queue<char>(prefixText), new StringReader(readerText));

            // Act
            var ch0 = r.Peek();
            r.Read();
            var ch1 = r.Peek();

            // Assert
            Assert.That(ch0, Is.EqualTo(firstChar));
            Assert.That(ch1, Is.EqualTo(secondChar));
        }
        [TestCase("", "abc", 1)]
        [TestCase("a", "bcd", 1)]
        [TestCase("def", "", 1)]
        [TestCase("de", "f", 1)]
        [TestCase("", "", 0)]
        public void Position_Case1(string prefixText, string readerText, int resultIndex)
        {
            // Arrange
            var r = new PrependableReader(CharacterPosition.StartOfFile, new Queue<char>(prefixText), new StringReader(readerText));

            // Act
            var p0 = r.Position.Index;
            var ch = r.Read();
            var p1 = r.Position.Index;

            // Assert
            Assert.That(p0, Is.EqualTo(0));
            Assert.That(p1, Is.EqualTo(resultIndex));
        }
        [TestCase("", "abc", 3)]
        [TestCase("a", "bcd", 4)]
        [TestCase("def", "", 3)]
        [TestCase("de", "f", 3)]
        [TestCase("", "", 0)]
        public void Position_Case2(string prefixText, string readerText, int resultIndex)
        {
            // Arrange
            var r = new PrependableReader(CharacterPosition.StartOfFile, new Queue<char>(prefixText), new StringReader(readerText));
            ConsumesAll(r);

            // Act
            var p1 = r.Position.Index;

            // Assert
            Assert.That(p1, Is.EqualTo(resultIndex));
        }
        [TestCase("", "ab\rcd")]
        [TestCase("a", "b\ncd")]
        [TestCase("de\rfg", "")]
        [TestCase("de\n", "fg")]
        [TestCase("de", "\nfg")]
        [TestCase("de", "\n")]
        public void LinePosition_Case1(string prefixText, string readerText)
        {
            // Arrange
            var r = new PrependableReader(CharacterPosition.StartOfFile, new Queue<char>(prefixText), new StringReader(readerText));
            r.Read();
            r.Read();

            // Act
            var l0 = r.Position.Line;
            r.Read();
            var l1 = r.Position.Line;

            // Assert
            Assert.That(l0, Is.EqualTo(1));
            Assert.That(l1, Is.EqualTo(2));
        }
        [TestCase("", "ab\r\ncd")]
        [TestCase("a", "b\r\ncd")]
        [TestCase("de\r\nfg", "")]
        [TestCase("de\r", "\nfg")]
        public void LinePosition_Case2(string prefixText, string readerText)
        {
            // Arrange
            var r = new PrependableReader(CharacterPosition.StartOfFile, new Queue<char>(prefixText), new StringReader(readerText));
            r.Read();
            r.Read();

            // Act
            var l0 = r.Position.Line;
            r.Read();
            var l1 = r.Position.Line;
            r.Read();
            var l2 = r.Position.Line;

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
        public void ColumnPosition_Case1(string prefixText, string readerText)
        {
            // Arrange
            var r = new PrependableReader(CharacterPosition.StartOfFile, new Queue<char>(prefixText), new StringReader(readerText));
            r.Read();
            r.Read();

            // Act
            var c0 = r.Position.Column;
            r.Read();
            var c1 = r.Position.Column;

            // Assert
            Assert.That(c0, Is.EqualTo(3));
            Assert.That(c1, Is.EqualTo(1));
        }
        [TestCase("", "abc")]
        [TestCase("a", "bc")]
        [TestCase("ab", "c")]
        [TestCase("abc", "")]
        public void Reattach_Case1(string prefixText, string readerText)
        {
            // Arrange
            var r = new PrependableReader(CharacterPosition.StartOfFile, new Queue<char>(prefixText), new StringReader(readerText));
            r.Read();

            // Act
            r.Reattach(CharacterPosition.StartOfFile, new Queue<char>(prefixText));
            var p = r.Position.Index;
            var l = r.Position.Line;
            var c = r.Position.Column;
            var ch = r.Peek();

            // Assert
            Assert.That(p, Is.EqualTo(0));
            Assert.That(l, Is.EqualTo(1));
            Assert.That(c, Is.EqualTo(1));
            Assert.That(ch, Is.EqualTo(prefixText.Length == 0 ? readerText[1] : (prefixText + readerText)[0]));
        }

        void ConsumesAll(PrependableReader r)
        {
            while (!r.EndOfFile) r.Read();
        }
    }
}
