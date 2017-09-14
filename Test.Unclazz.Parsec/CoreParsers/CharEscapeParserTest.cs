using NUnit.Framework;
using Unclazz.Parsec;
using static Unclazz.Parsec.Parsers;

namespace Test.Unclazz.Parsec.CoreParsers
{
    [TestFixture]
    public class CharEscapeParserTest
    {
        [Test]
        [Description("Case1 - バックスラッシュによるエスケープ - 読み取り結果が正しいこと")]
        public void Parse_Case1()
        {
            // Arrange
            var input = Reader.From("\\xyz");
            var parser = CharEscape("abcx", '\\');

            // Act
            var result = parser.Parse(input);

            // Assert
            result.IfSuccessful(a =>
            {
                Assert.That(result.Successful);
                Assert.That(result.Capture, Is.EqualTo('x'));
                Assert.That(result.End.Index, Is.EqualTo(2));
            },
            m => Assert.Fail(m));
        }
        [Test]
        [Description("Case2 - バックスラッシュによるエスケープ - 後続の通常の文字はそのままパースされること")]
        public void Parse_Case2()
        {
            // Arrange
            var input = Reader.From("\\xyz");
            var parser = CharEscape("abcx", '\\') & Char('y');

            // Act
            var result = parser.Parse(input);

            // Assert
            result.IfSuccessful(a =>
            {
                Assert.That(result.Successful);
                Assert.That(result.Capture, Is.EqualTo('x'));
                Assert.That(result.End.Index, Is.EqualTo(3));
            },
            m => Assert.Fail(m));
        }
        [Test]
        [Description("Case3 - その文字そのものによるエスケープ - 読み取りが正しいこと")]
        public void Parse_Case3()
        {
            // Arrange
            var input = Reader.From("xxyz");
            var parser = CharEscape("abcx", 'x') & Char('y');

            // Act
            var result = parser.Parse(input);

            // Assert
            result.IfSuccessful(a =>
            {
                Assert.That(result.Successful);
                Assert.That(result.Capture, Is.EqualTo('x'));
                Assert.That(result.End.Index, Is.EqualTo(3));
            },
            m => Assert.Fail(m));
        }
    }
}
