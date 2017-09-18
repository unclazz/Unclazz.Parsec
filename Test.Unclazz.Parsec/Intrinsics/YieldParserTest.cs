using NUnit.Framework;
using System;
using static Unclazz.Parsec.Parsers;

namespace Test.Unclazz.Parsec.Intrinsics
{
    [TestFixture]
    public class YieldParserTest
    {
        [Test]
        public void Parse_Case1()
        {
            // Arrange
            var kp = Keyword("0123");
            var yp = kp & Yield("hello");

            // Act
            var res = yp.Parse("012XXXXX");

            // Assert
            Assert.That(res.Successful, Is.False);
            Assert.That(() => res.Capture, Throws.InstanceOf<InvalidOperationException>());
        }
        [Test]
        public void Parse_Case2()
        {
            // Arrange
            var kp = Keyword("0123");
            var yp = kp & Yield("hello");

            // Act
            var res = yp.Parse("0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.True);
            Assert.That(res.Capture, Is.EqualTo("hello"));
            Assert.That(res.End.Index, Is.EqualTo(4));
        }
        [Test]
        public void Parse_Case3()
        {
            // Arrange
            var yp = Yield("hello");

            // Act
            var res = yp.Parse("0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.True);
            Assert.That(res.Capture, Is.EqualTo("hello"));
            Assert.That(res.Start.Index, Is.EqualTo(0));
        }
    }
}
