using NUnit.Framework;
using System;
using System.Linq;
using Unclazz.Parsec;
using static Unclazz.Parsec.Parsers;

namespace Test.Unclazz.Parsec.CoreParsers
{
    [TestFixture]
    public class DoubleParserTest
    {
        [Test]
        [Description("Parse - Case#1 - パース失敗時")]
        public void Parse_Case1()
        {
            // Arrange
            var kp = Keyword("012").Capture();
            var dp = kp.Then(Char('3').Capture());

            // Act
            var res = dp.Parse("012XXXXX");

            // Assert
            Assert.That(res.Successful, Is.False);
            Assert.That(() => res.Capture, Throws.InstanceOf<InvalidOperationException>());
        }
        [Test]
        [Description("Parse - Case#2 - パース成功時 かつ キャプチャありのパーサー2つから構築された場合")]
        public void Parse_Case2()
        {
            // Arrange
            var kp = Keyword("012").Capture();
            var dp = kp.Then(Char('3').Capture());

            // Act
            var res = dp.Parse("0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.True);
            Assert.That(res.Capture.Item1, Is.EqualTo("012"));
            Assert.That(res.Capture.Item2, Is.EqualTo('3'));
        }
        [Test]
        [Description("Parse - Case#4 - パース成功時 かつ キャプチャ失敗のパーサーから構築された場合")]
        public void Parse_Case4()
        {
            // Arrange
            var cp = Char('A').Capture().OrNot();
            var kp = Keyword("0123").Capture();
            var dp = cp.Then(kp);

            // Act
            var res = dp.Parse("0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.True);
            Assert.That(res.Capture.Item1, Is.EqualTo(new Optional<char>()));
            Assert.That(res.Capture.Item2, Is.EqualTo("0123"));
        }
        [Test]
        [Description("Parse - Case#5 - パース成功時 かつ パース失敗時もキャプチャするパーサーから構築された場合")]
        public void Parse_Case5()
        {
            // Arrange
            var cp = Char('A').OrNot().Capture();
            var kp = Keyword("0123").Capture();
            var dp = cp.Then(kp);

            // Act
            var res = dp.Parse("0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.True);
            Assert.That(res.Capture.Item1, Is.EqualTo(string.Empty));
            Assert.That(res.Capture.Item2, Is.EqualTo("0123"));
        }
    }
}
