using NUnit.Framework;
using Unclazz.Parsec;
using static Unclazz.Parsec.Parser;

namespace Test.Unclazz.Parsec.CoreParsers
{
    [TestFixture]
    public class RepeatExacltyParserTest
    {
        [TestCase("hello", 3)]
        [TestCase("123", 2)]
        public void Parse_Case1(string word, int count)
        {
            // Arrange
            var p0 = new KeywordParser(word, -1);
            var p1 = new RepeatExactlyParser<string>(p0, count, null);

            // Act
            var r1 = p1.Parse(TestUtility.Repeats(word, count));

            // Assert
            Assert.That(r1.Successful, Is.True);
        }
        [TestCase("hello", ",", 3)]
        [TestCase("123", "|", 2)]
        public void Parse_Case1(string word, string sep, int count)
        {
            // Arrange
            var p0 = new KeywordParser(word, -1);
            var p1 = new RepeatExactlyParser<string>(p0, count, sep: Keyword(sep));

            // Act
            var r1 = p1.Parse(TestUtility.Repeats(word, count, sep: sep));

            // Assert
            Assert.That(r1.Successful, Is.True);
        }
    }
}
