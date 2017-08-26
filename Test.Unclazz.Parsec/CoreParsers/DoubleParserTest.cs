using NUnit.Framework;
using System;
using System.Linq;
using Unclazz.Parsec;
using static Unclazz.Parsec.Parsers;

namespace Test.Unclazz.Parsec.CoreParsers
{
    [TestFixture]
    public class DoubleParserTest
    {
        [Test]
        public void Parse_Case1()
        {
            // Arrange
            var kp = Keyword("012").Capture();
            var dp = kp.Then(Char('3').Capture());

            // Act
            var res = dp.Parse("012XXXXX");

            // Assert
            Assert.That(res.Successful, Is.False);
            Assert.That(() => res.Capture.Present, Throws.InstanceOf<InvalidOperationException>());
        }
        [Test]
        public void Parse_Case2()
        {
            // Arrange
            var kp = Keyword("012").Capture();
            var dp = kp.Then(Char('3').Capture());

            // Act
            var res = dp.Parse("0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.True);
            Assert.That(res.Capture.Present, Is.True);
            Assert.That(res.Capture.Value.Item1, Is.EqualTo("012"));
            Assert.That(res.Capture.Value.Item2, Is.EqualTo("3"));
        }
        [Test]
        public void Parse_Case3()
        {
            // Arrange
            var kp = Keyword("012").Capture();
            var dp = kp.Then(Char('3').Cast<string>());

            // Act
            var res = dp.Parse("0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.True);
            Assert.That(res.Capture.Present, Is.True);
            Assert.That(res.Capture.Value.Item1, Is.EqualTo("012"));
            Assert.That(res.Capture.Value.Item2, Is.Null);
        }
    }
}
