using NUnit.Framework;
using System;
using Unclazz.Parsec;
using static Unclazz.Parsec.Parser;

namespace Test.Unclazz.Parsec.CoreParsers
{
    [TestFixture]
    public class StringInParserTest
    {
        [Test]
        public void Constructor_Case1()
        {
            // Arrange
            // Act
            // Assert
            var p0 = StringIn("hello", "bonjour");
            var p1 = StringIn("h", "hello");
            var p2 = StringIn("hello");
        }
        [Test]
        public void Constructor_Case2()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => StringIn(null), Throws.InstanceOf<ArgumentNullException>());
            Assert.That(() => StringIn(new string[0]), Throws.InstanceOf<ArgumentException>());
            Assert.That(() => StringIn("hello", string.Empty), Throws.InstanceOf<ArgumentException>());
            Assert.That(() => StringIn("hello", null), Throws.InstanceOf<ArgumentException>());
        }
        [Test]
        public void ToString_Case1()
        {
            // Arrange
            var p0 = StringIn("hello", "bonjour");
            var p1 = StringIn("h", "hello");
            var p2 = StringIn("hello");

            // Act
            var r0 = p0.ToString();
            var r1 = p1.ToString();
            var r2 = p2.ToString();

            // Assert
            Assert.That(r0, Is.EqualTo("StringIn(bonjour, hello)"));
            Assert.That(r1, Is.EqualTo("StringIn(h, hello)"));
            Assert.That(r2, Is.EqualTo("StringIn(hello)"));
        }
        [Test]
        public void Parse_Case1()
        {
            // Arrange
            var input = ParserInput.FromString("hello");
            var parser = StringIn("hallo", "helo");

            // Act
            var result = parser.Parse(input);

            // Assert
            Assert.That(result.Successful, Is.False);
        }
        [TestCase("hello0", new[] { "hello", "hallo" }, true)]
        [TestCase("hello1", new[] { "hallo", "hello" }, true)]
        [TestCase("hello2", new[] { "hallo", "he" }, true)]
        [TestCase("hello3", new[] { "ha", "he" }, true)]
        [TestCase("hello4", new[] { "ha", "h" }, true)]
        [TestCase("hello4", new[] { "h", "he" }, true)]
        [TestCase("hello5", new[] { "hellx", "helly" }, false)]
        [TestCase("hello6", new[] { "hello" }, true)]
        [TestCase("hello7", new[] { "hallo" }, false)]
        public void Parse_Case2(string text, string[] keywords, bool expectedResult)
        {
            // Arrange
            var input = ParserInput.FromString("hellox");
            var parser = Parser.StringIn(keywords);

            // Act
            var result = parser.Parse(input);

            // Assert
            Assert.That(result.Successful, Is.EqualTo(expectedResult));
        }
    }
}
