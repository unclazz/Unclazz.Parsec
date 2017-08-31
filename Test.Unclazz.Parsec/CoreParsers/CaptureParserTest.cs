using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec;
using static Unclazz.Parsec.Parsers;

namespace Test.Unclazz.Parsec.CoreParsers
{
    [TestFixture]
    public class CaptureParserTest
    {
        [Test]
        public void Parse_Case1()
        {
            // Arrange
            var kp = Keyword("0123");
            var cp = kp.Capture();

            // Act
            var res = cp.Parse("012XXXXX");

            // Assert
            Assert.That(res.Successful, Is.False);
            Assert.That(() => res.Value, Throws.InstanceOf<InvalidOperationException>());
        }
        [Test]
        public void Parse_Case2()
        {
            // Arrange
            var kp = Keyword("0123");
            var cp = kp.Capture();

            // Act
            var res = cp.Parse("0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.True);
            Assert.That(res.Value, Is.EqualTo("0123"));
        }
    }
}
