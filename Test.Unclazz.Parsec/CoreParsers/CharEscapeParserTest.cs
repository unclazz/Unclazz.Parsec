using NUnit.Framework;
using System.Linq;
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
        [Test]
        [Description("Case4 - その文字そのものによるエスケープ - 読み取りが正しいこと")]
        public void Parse_Case4()
        {
            // Arrange
            var input = Reader.From("xaaxzxbbx_");
            var parser = (Char('x') & (CharEscape("x", 'x') | CharIn("ab").Capture()).Repeat().Join() & Char('x'))
                .Repeat(sep: Char('z'));

            // Act
            var result = parser.Parse(input);

            // Assert
            result.IfSuccessful(a =>
            {
                Assert.That(result.Successful);
                Assert.That(result.Capture.ToArray(), Is.EqualTo(new[] { "aa", "bb" }));
                Assert.That(result.End.Index, Is.EqualTo(9));
            },
            m => Assert.Fail(m));
        }
        [Test]
        [Description("Case5 - その文字そのものによるエスケープ - 読み取りが正しいこと")]
        public void Parse_Case5()
        {
            // Arrange
            var input = Reader.From("xaxxaxzxbxxbx_");
            var parser = (Char('x') & (CharEscape("x", 'x') | CharIn("ab").Capture()).Repeat().Join() & Char('x'))
                .Repeat(sep: Char('z'));

            // Act
            var result = parser.Parse(input);

            // Assert
            result.IfSuccessful(a =>
            {
                Assert.That(result.Successful);
                Assert.That(result.Capture.ToArray(), Is.EqualTo(new[] { "axa", "bxb" }));
                Assert.That(result.End.Index, Is.EqualTo(13));
            },
            m => Assert.Fail(m));
        }
    }
}
