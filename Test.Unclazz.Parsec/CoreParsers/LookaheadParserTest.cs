using NUnit.Framework;
using Unclazz.Parsec;
using static Unclazz.Parsec.Parsers;

namespace Test.Unclazz.Parsec.CoreParsers
{
    [TestFixture]
    public class LookaheadParserTest
    {
        [Test]
        public void Parse_Case1()
        {
            // Arrange
            var cp = Char('a');
            var cp2 = Char('b');
            var lp = cp & Lookahead(cp2);
            Reader r = "axc";

            // Act
            var res = lp.Parse(r);

            // Assert
            Assert.That(res.Successful, Is.False);
            Assert.That(r.Position.Index, Is.EqualTo(1));
        }
        [Test]
        public void Parse_Case2()
        {
            // Arrange
            var cp = Char('a');
            var cp2 = Char('b');
            var lp = cp & Lookahead(cp2);
            Reader r = "abc";

            // Act
            var res = lp.Parse(r);

            // Assert
            Assert.That(res.Successful, Is.True);
            Assert.That(r.Position.Index, Is.EqualTo(1));
        }
        [Test]
        public void Parse_Case3()
        {
            // Arrange
            var cp = Char('a');
            var cp2 = Char('b');
            var lp = cp & Lookahead(cp2);
            Reader r = "abc";

            // Act
            var res = lp.Capture().Parse(r);

            // Assert
            Assert.That(res.Successful, Is.True);
            Assert.That(res.Capture, Is.EqualTo("a"));
            Assert.That(r.Position.Index, Is.EqualTo(1));
        }
    }
}
