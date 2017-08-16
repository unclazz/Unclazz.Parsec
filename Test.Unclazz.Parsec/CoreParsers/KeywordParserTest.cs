using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec;

namespace Test.Unclazz.Parsec
{
    [TestFixture]
    public class KeywordParserTest
    {
        [TestCase("0123456789X", "0", true)]
        [TestCase("0123456789X", "01", true)]
        [TestCase("0123456789X", "1", false)]
        [TestCase("0123456789X", "12", false)]
        [TestCase("0123456789X", "0123456789X", true)]
        [TestCase("0123456789X", "0123456789XY", false)]
        public void Parse_Case1(string text, string word, bool expectedResult)
        {
            // Arrange
            var input = ParserInput.FromString(text);
            var parser = Parser.Keyword(word);

            // Act
            var result = parser.Parse(input);

            // Assert
            Assert.That(result.Successful, Is.EqualTo(expectedResult));
            result.IfSuccessful((pos, cap) =>
            {
                Assert.That(pos.Index, Is.EqualTo(0));
                Assert.That(cap.HasValue, Is.False);
                Assert.That(() => cap.Value, Throws.InstanceOf<InvalidOperationException>());
            });
        }
        [TestCase("0123456789X", "1", true)]
        [TestCase("0123456789X", "12", true)]
        [TestCase("0123456789X", "01", false)]
        public void Parse_Case2(string text, string word, bool expectedResult)
        {
            // Arrange
            var input = ParserInput.FromString(text);
            var parser = Parser.Keyword(word);
            input.Read();

            // Act
            var result = parser.Parse(input);

            // Assert
            Assert.That(result.Successful, Is.EqualTo(expectedResult));
            result.IfSuccessful((pos, cap) =>
            {
                Assert.That(pos.Index, Is.EqualTo(1));
                Assert.That(cap.HasValue, Is.False);
                Assert.That(() => cap.Value, Throws.InstanceOf<InvalidOperationException>());
            });
        }
        [TestCase("0123456789X", "1", true)]
        [TestCase("0123456789X", "12", true)]
        [TestCase("0123456789X", "01", false)]
        public void Parse_Case3(string text, string word, bool expectedResult)
        {
            // Arrange
            var input = ParserInput.FromString(text);
            var parser = Parser.Keyword(word).Capture();
            input.Read();

            // Act
            var result = parser.Parse(input);

            // Assert
            Assert.That(result.Successful, Is.EqualTo(expectedResult));
            result.IfSuccessful((pos, cap) =>
            {
                Assert.That(pos.Index, Is.EqualTo(1));
                Assert.That(cap.HasValue, Is.True);
                Assert.That(cap.Value, Is.EqualTo(word));
            });
        }
    }
}
