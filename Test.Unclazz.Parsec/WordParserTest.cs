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
    public class WordParserTest
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
            var parser = Parser.Word(word);

            // Act
            var result = parser.Parse(input);

            // Assert
            Assert.That(result.Successful, Is.EqualTo(expectedResult));
        }
    }
}
