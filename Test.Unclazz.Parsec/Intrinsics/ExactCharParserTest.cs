using NUnit.Framework;
using System;
using Unclazz.Parsec;
using static Unclazz.Parsec.Parsers;

namespace Test.Unclazz.Parsec.Intrinsics
{
    [TestFixture]
    public class ExactCharParserTest
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
            Assert.That(res.Capture, Is.EqualTo('a'));
        }
    }
}
