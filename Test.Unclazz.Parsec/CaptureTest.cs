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
    public class CaptureTest
    {
        [Test]
        [Description("Constructor - Case #1 - 引数なしコンストラクタで初期化するとキャプチャ値なし（空）のインスタンスになること")]
        public void Constructor_Case1()
        {
            // Arrange
            // Act
            var cap0 = new Capture<int>(); // 値型
            var cap1 = new Capture<string>(); // 参照型

            // Assert
            Assert.That(cap0.HasValue, Is.False);
            Assert.That(cap1.HasValue, Is.False);
        }
        [Test]
        [Description("Constructor - Case #2 - 値を持つインスタンスを返すこと")]
        public void OfSingle_Case1()
        {
            // Arrange
            // Act
            var cap0 = new Capture<int>(123); // 値型
            var cap1 = new Capture<string>("123"); // 参照型

            // Assert
            Assert.That(cap0.HasValue, Is.True);
            Assert.That(cap1.HasValue, Is.True);
        }
        [Test]
        [Description("Map - Case #1 - 空の場合は型パラメータの変更以外何も起きないこと")]
        public void Map_Case1()
        {
            // Arrange
            var cap0 = new Capture<int>();
            var cap1 = new Capture<string>();
            var chk0 = 0;
            var chk1 = 0;

            // Act
            var cap0_map = cap0.Map(a => { chk0++; return a.ToString(); });
            var cap1_map = cap1.Map(a => { chk1++; return int.Parse(a); });

            // Assert
            Assert.That(cap0_map.HasValue, Is.False);
            Assert.That(cap1_map.HasValue, Is.False);
            Assert.That(chk0, Is.EqualTo(0));
            Assert.That(chk1, Is.EqualTo(0));
        }
        [Test]
        [Description("Map - Case #1 - 関数で変換された結果を要素とするインスタンスが返されること")]
        public void Map_Case2()
        {
            // Arrange
            var cap0 = new Capture<int>(123);
            var cap1 = new Capture<string>("123");

            // Act
            var cap0_map = cap0.Map(a => a.ToString());
            var cap1_map = cap1.Map(a => int.Parse(a));

            // Assert
            Assert.That(cap0_map.HasValue, Is.True);
            Assert.That(cap1_map.HasValue, Is.True);
            Assert.That(cap0_map.Value, Is.EqualTo("123"));
            Assert.That(cap1_map.Value, Is.EqualTo(123));
        }
        [Test]
        [Description("ToString - Case #1 - 要素0、要素1、要素2以上、それぞれインスタンス内容を示す文字列が返されること")]
        public void ToString_Case1()
        {
            // Arrange
            var cap0 = new Capture<string>();
            var cap1 = new Capture<string>("hello");

            // Act
            // Assert
            Assert.That(cap0.ToString(), Is.EqualTo("Capture()"));
            Assert.That(cap1.ToString(), Is.EqualTo("Capture(hello)"));
        }
    }
}
