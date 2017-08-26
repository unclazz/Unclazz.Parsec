using NUnit.Framework;
using Unclazz.Parsec;
using static Unclazz.Parsec.Parsers;
using System;
using Unclazz.Parsec.CoreParsers;

namespace Test.Unclazz.Parsec.CoreParsers
{
    [TestFixture]
    public class RepeatExacltyParserTest
    {
        [TestCase("hello", 0, false)]
        [TestCase("hello", 1, false)]
        [TestCase("hello", 2, true)]
        [Description("Constructor - Case #1 - 繰り返し回数1以下の場合例外がスローされること")]
        public void Constructor_Case1(string word, int count, bool okNg)
        {
            // Arrange
            var p0 = new KeywordParser(word, -1);

            // Act
            // Assert
            if (okNg)
            {
                var p1 = RepeatParser<Nil>.Create(p0, exactly: count);
            }
            else
            {
                Assert.That(() =>
                {
                    var p1 = RepeatParser<Nil>.Create(p0, exactly: count);
                }, Throws.InstanceOf<ArgumentOutOfRangeException>());
            }
        }

        [TestCase("hello", 3)]
        [TestCase("123", 2)]
        [Description("Parse - Case #1 - 指定された繰り返し回数分だけパースが行われること")]
        public void Parse_Case1(string word, int count)
        {
            // Arrange
            var p0 = new KeywordParser(word, -1);
            var p1 = RepeatParser<Nil>.Create(p0, exactly: count);
            Reader input = TestUtility.Repeats(word, count + 1);

            // Act
            var r1 = p1.Parse(input);

            // Assert
            Assert.That(r1.Successful, Is.True);
            Assert.That(input.EndOfFile, Is.False);
            Assert.That(input.Position.Index, Is.EqualTo(word.Length * count));
        }
        [TestCase("hello", ",", 3)]
        [TestCase("123", "|", 2)]
        [Description("Parse - Case #2 - 指定された繰り返し回数分だけパースが行われること（セパレーター指定あり）")]
        public void Parse_Case2(string word, string sep, int count)
        {
            // Arrange
            var p0 = new KeywordParser(word, -1);
            var p1 = RepeatParser<Nil>.Create(p0, exactly: count, sep: Keyword(sep));
            Reader input = TestUtility.Repeats(word, count + 1, sep: sep);

            // Act
            var r1 = p1.Parse(input);

            // Assert
            Assert.That(r1.Successful, Is.True);
            Assert.That(input.Position.Index, Is.EqualTo((word + sep).Length * count - 1));
        }
        [TestCase("hello", ",", 4)]
        [TestCase("123", "|", 3)]
        [Description("Parse - Case #3 - 指定された繰り返し回数に満たない場合はパース失敗となること")]
        public void Parse_Case3(string word, string sep, int count)
        {
            // Arrange
            var p0 = new KeywordParser(word, -1);
            var p1 = RepeatParser<Nil>.Create(p0, exactly: count, sep: Keyword(sep));
            Reader input = TestUtility.Repeats(word, count - 1, sep: sep) + sep + "__________";

            // Act
            var r1 = p1.Parse(input);

            // Assert
            Assert.That(r1.Successful, Is.False);
        }
    }
}
