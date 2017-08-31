using NUnit.Framework;
using System;
using Unclazz.Parsec;
using static Unclazz.Parsec.Parsers;

namespace Test.Unclazz.Parsec.CoreParsers
{
    [TestFixture]
    public class SkipSpaceParserTest
    {
        [Test]
        [Description("Parse - Case#1 - 元のクラスの読み取り結果に影響しない（失敗ケース）")]
        public void Parse_Case1()
        {
            // Arrange
            var kp = Keyword("0123");
            var ssp = kp.Skip();

            // Act
            var res = ssp.Parse("012XXXXX");

            // Assert
            Assert.That(res.Successful, Is.False);
        }
        [Test]
        [Description("Parse - Case#2 - 元のクラスの読み取り結果に影響しない（成功ケース）")]
        public void Parse_Case2()
        {
            // Arrange
            var kp = Keyword("0123");
            var ssp = kp.Skip();

            // Act
            var res = ssp.Parse("0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.True);
        }
        [Test]
        [Description("Parse - Case#3 - トークンに先行する空白文字はすべてスキップ、キャプチャ結果にも影響しない")]
        public void Parse_Case3()
        {
            var kp = Keyword("0123");
            var cp = kp.Capture();
            var ssp = cp.Skip();

            // Act
            var res = ssp.Parse("  0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.True);
            Assert.That(res.Value, Is.EqualTo("0123"));
        }
        [Test]
        [Description("Parse - Case#4 - スキップ設定は継承される")]
        public void Parse_Case4()
        {
            var kp = Keyword("0123");
            var ssp = kp.Skip();
            var cp = ssp.Capture(); // sspのCaptureを呼び出すことでcpへとコンフィギュレーションが継承される

            // Act
            var res = cp.Parse("  0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.True);
            Assert.That(res.Value, Is.EqualTo("0123")); // kpではなくcpの事前処理でスキップが行われている
        }
        [Test]
        [Description("Parse - Case#5 - " +
            "スキップなしとスキップありの2つのパーサーを連結すると" +
            "間のスキップ対象文字列はキャプチャ対象となる")]
        public void Parse_Case5()
        {
            var kp = Keyword("0123");
            var ssp = kp & kp.Skip();
            var cp = ssp.Capture();

            // Act
            var res = cp.Parse("0123  0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.True);
            Assert.That(res.Value, Is.EqualTo("0123  0123"));
        }
        [Test]
        [Description("Parse - Case#6 - スキップ設定の継承は下方のみで上方には起こらない")]
        public void Parse_Case6()
        {
            var kp = Keyword("0123");
            var ssp = kp.Skip();
            var cp = ssp.Capture(); // sspのCaptureを呼び出すことでcpへとコンフィギュレーションが継承される
            var ssp2 = cp & kp;

            // Act
            var res = ssp2.Parse("  0123  0123XXXX");

            // Assert
            Assert.That(res.Successful, Is.False);
            Assert.That(() => res.Value, Throws.InstanceOf<InvalidOperationException>());
        }
    }
}
