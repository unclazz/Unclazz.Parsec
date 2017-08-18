using NUnit.Framework;
using Unclazz.Parsec;

namespace Test.Unclazz.Parsec.CoreParsers
{
    [TestFixture]
    public class SingleCharParserTest
    {
        [Test]
        public void Parse_Case1()
        {
            // Arrange
            var input = ParserInput.FromString("hello");
            var parser = Parser.Char('a');

            // Act
            var result = parser.Parse(input);

            // Assert
            Assert.That(result.Successful, Is.False);
            Assert.That(input.Position.Index, Is.EqualTo(1));
        }
        [Test]
        public void Parse_Case2()
        {
            // Arrange
            var input = ParserInput.FromString("hello");
            var parser = Parser.Char('h');

            // Act
            var result = parser.Parse(input);

            // Assert
            Assert.That(result.Successful, Is.True);
            Assert.That(result.Capture.HasValue, Is.False);
            Assert.That(input.Position.Index, Is.EqualTo(1));
        }
    }
}
