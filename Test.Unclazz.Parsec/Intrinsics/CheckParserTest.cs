using System;
using NUnit.Framework;
using Unclazz.Parsec;

namespace Test.Unclazz.Parsec.Intrinsics
{
    [TestFixture]
    public class CheckParserTest : ParserBase
    {
        [Test]
        public void Parse_Case1()
        {
            // Arrange
            var p = CharsWhileIn(CharClass.Alphabetic).Capture().Check(a => a != "hello", "ng");

            // Act
            var r = p.Parse("hello ...");

            // Assert
            Assert.That(r.Successful, Is.False);
            Assert.That(r.Message, Is.EqualTo("ng"));
            Assert.That(r.Start.Index, Is.EqualTo(0));
            Assert.That(r.End.Index, Is.EqualTo(5));
        }
        [Test]
        public void Parse_Case2()
        {
            // Arrange
            var p = CharsWhileIn(CharClass.Alphabetic).Capture().Check(a => a != "hello", "ng");

            // Act
            var r = p.Parse("hallo ...");

            // Assert
            Assert.That(r.Successful, Is.True);
            Assert.That(r.Capture, Is.EqualTo("hallo"));
            Assert.That(r.Start.Index, Is.EqualTo(0));
            Assert.That(r.End.Index, Is.EqualTo(5));
        }
        [Test]
        public void Parse_Case3()
        {
            // Arrange
            var p = CharsWhileIn(CharClass.Alphabetic).Capture().Check(a => a != "hello", "ng");

            // Act
            var r = p.Parse("_hello ...");

            // Assert
            Assert.That(r.Successful, Is.False);
            Assert.That(r.Message, Is.Not.EqualTo("ng"));
            Assert.That(r.Start.Index, Is.EqualTo(0));
            Assert.That(r.End.Index, Is.EqualTo(0));
        }
    }
}
