using NUnit.Framework;
using System;
using System.Linq;
using Unclazz.Parsec;

namespace Test.Unclazz.Parsec.Intrinsics
{
    [TestFixture]
    public class TypedParserTest : ParserBase
    {
        [Test]
        public void Parse_Case3()
        {
            // Arrange
            var kp = Keyword("0123");
            var cp = kp.Typed("3210");

            // Act
            var res = cp.Parse("0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.True);
            Assert.That(res.Capture, Is.EqualTo("3210"));
        }
    }
}
