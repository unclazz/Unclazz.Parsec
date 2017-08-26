using NUnit.Framework;
using System;
using Unclazz.Parsec;
using static Unclazz.Parsec.Parsers;

namespace Test.Unclazz.Parsec.CoreParsers
{
    [TestFixture]
    public class CharParserTest
    {
        [Test]
        public void Parse_Case1()
        {
            // Arrange
            var kp = Char('a');

            // Act
            var res = kp.Parse("xbc");

            // Assert
            Assert.That(res.Successful, Is.False);
            Assert.That(() => res.Capture.Present, Throws.InstanceOf<InvalidOperationException>());
        }
        [Test]
        public void Parse_Case2()
        {
            // Arrange
            var kp = Char('a');

            // Act
            var res = kp.Parse("abc");

            // Assert
            Assert.That(res.Successful, Is.True);
            Assert.That(res.Capture.Present, Is.False);
        }
        [Test]
        public void Parse_Case3()
        {
            // Arrange
            var kp = Char('a').Capture();

            // Act
            var res = kp.Parse("abc");

            // Assert
            Assert.That(res.Successful, Is.True);
            Assert.That(res.Capture.Present, Is.True);
            Assert.That(res.Capture.Value, Is.EqualTo("a"));
        }
    }
}
