using NUnit.Framework;
using System;
using Unclazz.Parsec;
using static Unclazz.Parsec.Parsers;

namespace Test.Unclazz.Parsec.Intrinsics
{
    [TestFixture]
    public class KeywordInParserTest
    {
        [Test]
        public void Constructor_Case1()
        {
            // Arrange
            // Act
            // Assert
            var p0 = KeywordIn("hello", "bonjour");
            var p1 = KeywordIn("h", "hello");
            var p2 = KeywordIn("hello");
        }
        [Test]
        public void Constructor_Case2()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => KeywordIn(null), Throws.InstanceOf<ArgumentNullException>());
            Assert.That(() => KeywordIn(new string[0]), Throws.InstanceOf<ArgumentException>());
            Assert.That(() => KeywordIn("hello", string.Empty), Throws.InstanceOf<ArgumentException>());
            Assert.That(() => KeywordIn("hello", null), Throws.InstanceOf<ArgumentException>());
        }
        [Test]
        public void Parse_Case1()
        {
            // Arrange
            var input = Reader.From("hello");
            var parser = KeywordIn("hallo", "helo");

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
            var input = Reader.From("hellox");
            var parser = KeywordIn(keywords);

            // Act
            var result = parser.Parse(input);

            // Assert
            Assert.That(result.Successful, Is.EqualTo(expectedResult));
        }
    }
}
