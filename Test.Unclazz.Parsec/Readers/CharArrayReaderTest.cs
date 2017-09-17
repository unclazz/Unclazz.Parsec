using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec.Readers;

namespace Test.Unclazz.Parsec.Readers
{
    [TestFixture]
    public class CharArrayReaderTest
    {
        [Test]
        public void Empty_Case1()
        {
            // Arrange
            var q = CharArrayReader.Empty;
            // Act
            // Assert
            Assert.That(q.EndOfFile, Is.True);
            Assert.That(q.Count, Is.EqualTo(0));
            Assert.That(q.Peek(), Is.EqualTo(-1));
            Assert.That(q.Read(), Is.EqualTo(-1));
        }
        [TestCase("")]
        [TestCase("0")]
        [TestCase("0123")]
        public void From_Case1(string chars)
        {
            // Arrange
            var q = CharArrayReader.From(chars.ToArray());
            // Act
            // Assert
            Assert.That(q.EndOfFile, Is.EqualTo(chars.Length <= 0));
            Assert.That(q.Count, Is.EqualTo(chars.Length));

            if (chars.Length == 0)
            {
                Assert.That(() => q.Peek(), Is.EqualTo(-1));
                Assert.That(() => q.Read(), Is.EqualTo(-1));
            }
            else
            {
                Assert.That(q.ToArray(), Is.EqualTo(chars.ToArray()));
            }
        }
        [Test]
        public void From_Case2_Null()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => CharArrayReader.From(null), Throws.InstanceOf<ArgumentNullException>());
        }
        [TestCase("")]
        [TestCase("0")]
        [TestCase("0123")]
        public void Prepend_Case1(string chars)
        {
            // Arrange
            var q = CharArrayReader.From("abc".ToArray());

            // Act
            var q2 = q.Prepend(chars.ToArray());

            // Assert
            Assert.That(q2.Count, Is.EqualTo(3 + chars.Length));
            Assert.That(q2.ToArray(), Is.EqualTo((chars + "abc").ToArray()));
        }
        [TestCase("")]
        [TestCase("0")]
        [TestCase("0123")]
        public void Prepend_Case2(string chars)
        {
            // Arrange
            var q = CharArrayReader.From(chars.ToArray());

            // Act
            var q2 = q.Prepend("abc".ToArray());

            // Assert
            Assert.That(q2.Count, Is.EqualTo(3 + chars.Length));
            Assert.That(q2.ToArray(), Is.EqualTo(("abc" + chars).ToArray()));
        }
        [Test]
        public void ReadToEnd_Case1()
        {
            // Arrange
            var q = CharArrayReader.From("hello\n01234\r\nabcde".ToArray());

            // Act
            // Assert
            Assert.That(q.ReadToEnd(), Is.EqualTo("hello\n01234\r\nabcde"));
        }
    }
}
