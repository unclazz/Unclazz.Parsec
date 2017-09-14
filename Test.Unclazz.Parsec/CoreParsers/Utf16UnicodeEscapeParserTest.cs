using NUnit.Framework;
using static Unclazz.Parsec.Parsers;

namespace Test.Unclazz.Parsec.CoreParsers
{
    [TestFixture]
    public class Utf16UnicodeEscapeParserTest
    {
        [TestCase("\\u0030", '0')]
        [TestCase("\\u0031", '1')]
        [TestCase("\\u0041", 'A')]
        [TestCase("\\u0042", 'B')]
        public void Parse_Case1_Alphanumeric(string escape, char unescape)
        {
            // Arrange
            // Act
            var r = Utf16UnicodeEscape().Parse(escape);

            // Assert
            Assert.That(r.Capture, Is.EqualTo(unescape));
        }
        [TestCase("FOO0030", '0')]
        [TestCase("FOO0031", '1')]
        [TestCase("FOO0041", 'A')]
        [TestCase("FOO0042", 'B')]
        public void Parse_Case2_Alphanumeric_CustomPrefix(string escape, char unescape)
        {
            // Arrange
            // Act
            var r = Utf16UnicodeEscape(prefix: "FOO").Parse(escape);

            // Assert
            Assert.That(r.Capture, Is.EqualTo(unescape));
        }
        [Test]
        public void Parse_Case3_SurrogatePair()
        {
            // Arrange
            // Act
            var r = Utf16UnicodeEscape().Repeat(exactly: 2).Join()
                .Parse("\\uD869\\uDEB2"); // \uD869\uDEB2 = 𪚲

            // Assert
            Assert.That(r.Capture, Is.EqualTo("𪚲"));
        }
    }
}
