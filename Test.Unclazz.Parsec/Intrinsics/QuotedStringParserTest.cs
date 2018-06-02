using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec;

namespace Test.Unclazz.Parsec.Intrinsics
{
    [TestFixture]
    public class QuotedStringParserTest: ParserBase
    {
        readonly Parser<string> quoted0 = QuotedString();
        readonly Parser<string> quoted1 = QuotedString(escape: ControlEscape() | Utf16UnicodeEscape() | CharEscape("\""));

        [Test]
        public void Parse_Case1_NoEscape_Empty()
        {
            // Arrange
            // Act
            var res = quoted0.Parse("\"\"");

            // Assert
            Assert.That(res.Successful);
            Assert.That(res.Capture, Is.EqualTo(""));
        }

        [Test]
        public void Parse_Case2_NoEscape_Spaces()
        {
            // Arrange
            // Act
            var res = quoted0.Parse("\"  \"");

            // Assert
            Assert.That(res.Successful);
            Assert.That(res.Capture, Is.EqualTo("  "));
        }

        [Test]
        public void Parse_Case3_NoEscape_Alphabet()
        {
            // Arrange
            // Act
            var res = quoted0.Parse("\"abc\"");

            // Assert
            Assert.That(res.Successful);
            Assert.That(res.Capture, Is.EqualTo("abc"));
        }

        [TestCase("\"abc\r\n123\"")]
        [TestCase("\"abc123\r\n\"")]
        [TestCase("\"\r\nabc123\"")]
        public void Parse_Case4_NoEscape_IncludingCrLf(string text)
        {
            // Arrange
            // Act
            var res = quoted0.Parse(text);

            // Assert
            Assert.That(res.Successful);
            Assert.That(res.Capture, Is.EqualTo(text.Substring(1, text.Length -2)));
        }

        [Test]
        public void Parse_Case11_NoEscape_Empty_NoEnd()
        {
            // Arrange
            // Act
            var res = quoted0.Parse("\"");

            // Assert
            Assert.That(res.Successful, Is.False);
        }

        [Test]
        public void Parse_Case12_NoEscape_Spaces_NoEnd()
        {
            // Arrange
            // Act
            var res = quoted0.Parse("\"  ");

            // Assert
            Assert.That(res.Successful, Is.False);
        }

        [TestCase("\"abc")]
        [TestCase("\"  abc")]
        [TestCase("\"ABC")]
        public void Parse_Case13_NoEscape_Alphabet_NoEnd(string text)
        {
            // Arrange
            // Act
            var res = quoted0.Parse(text);

            // Assert
            Assert.That(res.Successful, Is.False);
        }

        [TestCase("\"abc\r\n123")]
        [TestCase("\"abc\r\n")]
        [TestCase("\"\r\n123")]
        public void Parse_Case14_NoEscape_IncludingCrLf_NoEnd(string text)
        {
            // Arrange
            // Act
            var res = quoted0.Parse(text);

            // Assert
            Assert.That(res.Successful, Is.False);
        }

        [Test]
        public void Parse_Case21_Escape_Empty()
        {
            // Arrange
            // Act
            var res = quoted1.Parse("\"\"");

            // Assert
            Assert.That(res.Successful);
            Assert.That(res.Capture, Is.EqualTo(""));
        }

        [Test]
        public void Parse_Case22_Escape_Spaces()
        {
            // Arrange
            // Act
            var res = quoted1.Parse("\"  \"");

            // Assert
            Assert.That(res.Successful);
            Assert.That(res.Capture, Is.EqualTo("  "));
        }

        [Test]
        public void Parse_Case23_Escape_Alphabet()
        {
            // Arrange
            // Act
            var res = quoted1.Parse("\"abc\"");

            // Assert
            Assert.That(res.Successful);
            Assert.That(res.Capture, Is.EqualTo("abc"));
        }

        [TestCase("\"abc\r\n123\"")]
        [TestCase("\"abc123\r\n\"")]
        [TestCase("\"\r\nabc123\"")]
        public void Parse_Case24_Escape_IncludingCrLf(string text)
        {
            // Arrange
            // Act
            var res = quoted1.Parse(text);

            // Assert
            Assert.That(res.Successful);
            Assert.That(res.Capture, Is.EqualTo(text.Substring(1, text.Length - 2)));
        }

        [Test]
        public void Parse_Case25_Escape_Control()
        {
            // Arrange
            // Act
            var res = quoted1.Parse("\"\\r\\n\\t\\v\"");

            // Assert
            Assert.That(res.Successful);
            Assert.That(res.Capture, Is.EqualTo("\r\n\t\v"));
        }

        [TestCase("\"abcd\\\"\"")]
        [TestCase("\"ab\\\"cd\"")]
        [TestCase("\"\\\"abcd\"")]
        public void Parse_Case26_Escape_Quote(string text)
        {
            // Arrange
            // Act
            var res = quoted1.Parse(text);

            // Assert
            Assert.That(res.Successful);
            Assert.That(res.Capture, Is.EqualTo(text.Substring(1, text.Length - 2).Replace("\\", string.Empty)));
        }

        [Test]
        public void Parse_Case30_CSV()
        {
            // Arrange
            var quoteEscape = CharEscape("\"", '"');
            var quoted2 = QuotedString(escape: quoteEscape).Repeat(min: 2, sep: Char(','));

            // Act
            var csv = quoted2.Parse("\"\",\"111\",\"222\",\"\"\"333\",\"4\"\"44\",\"555\"\"\"");

            // Assert
            csv.IfSuccessful(a =>
            {
                Assert.That(a.ToArray(), Is.EqualTo(new[] { "", "111", "222", "\"333", "4\"44", "555\"" }));
            },
            m =>
            {
                Assert.Fail(m + csv);
            });
        }
    }
}
