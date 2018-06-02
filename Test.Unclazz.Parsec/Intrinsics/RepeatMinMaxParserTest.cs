using NUnit.Framework;
using Unclazz.Parsec;
using System;
using Unclazz.Parsec.Intrinsics;

namespace Test.Unclazz.Parsec.Intrinsics
{
    [TestFixture]
    public class RepeatMinMaxParserTest : ParserBase
    {
        [TestCase("hello", -2, -1, false)]
        [TestCase("hello", -2, -2, false)]
        [TestCase("hello", -1, -1, true)]
        [TestCase("hello", 2, 1, false)]
        [TestCase("hello", 1, 2, true)]
        [TestCase("hello", 0, 0, false)]
        [TestCase("hello", 0, 1, true)]
        [Description("Constructor - Case #1 - min/max個別の値および大小関係がNGの場合例外スローされること")]
        public void Constructor_Case1(string word, int min, int max, bool okNg)
        {
            // Arrange
            var p0 = Keyword(word);

            // Act
            // Assert
            if (okNg)
            {
                var p1 = p0.Repeat(min: min, max: max);
            }
            else
            {
                Assert.That(() =>
                {
                    var p1 = p0.Repeat(min: min, max: max);
                }, Throws.InstanceOf<ArgumentOutOfRangeException>());
            }
        }

        [TestCase("hello", 2, 3, 1, false)]
        [TestCase("hello", 2, 3, 2, true)]
        [TestCase("hello", 2, 3, 3, true)]
        [TestCase("hello", 2, 3, 4, true)]
        [Description("Parse - Case #1 - 指定された繰り返し回数範囲だけパースが行われること")]
        public void Parse_Case1(string word, int min, int max, int wordRepeatCount, bool expectedResult)
        {
            // Arrange
            var p0 = Keyword(word);
            var p1 = p0.Repeat(min: min, max: max);
            string text = TestUtility.Repeats(word, wordRepeatCount);
            Reader input = text;

            // Act
            var r1 = p1.Parse(input);

            // Assert
            Assert.That(r1.Successful, Is.EqualTo(expectedResult));
            Assert.That(input.EndOfFile, Is.EqualTo(max >= wordRepeatCount));
            Assert.That(input.Position.Index, Is.EqualTo(word.Length * Math.Min(max, wordRepeatCount)));
        }
        [TestCase("hello", 2, 3, 1, false)]
        [TestCase("hello", 2, 3, 2, true)]
        [TestCase("hello", 2, 3, 3, true)]
        [TestCase("hello", 2, 3, 4, true)]
        [Description("Parse - Case #1 - 指定された繰り返し回数範囲だけパースが行われること（セパレーター指定あり）")]
        public void Parse_Case2(string word, int min, int max, int wordRepeatCount, bool expectedResult)
        {
            // Arrange
            var p0 = Keyword(word);
            var p1 = p0.Repeat(min: min, max: max, sep: Char('$'));
            string text = TestUtility.Repeats(word, wordRepeatCount, "$");
            Reader input = text;

            // Act
            var r1 = p1.Parse(input);

            // Assert
            Assert.That(r1.Successful, Is.EqualTo(expectedResult));
        }
    }
}
