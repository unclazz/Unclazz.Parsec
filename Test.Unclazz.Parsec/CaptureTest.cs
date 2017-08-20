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
            Assert.That(cap0.Value.Count(), Is.EqualTo(0));
            Assert.That(cap1.Value.Count(), Is.EqualTo(0));
        }
        [Test]
        [Description("OfEmpty - Case #1 - 引数なしコンストラクタと同じ結果になること")]
        public void OfEmpty_Case1()
        {
            // Arrange
            // Act
            var cap0 = Capture<int>.OfEmpty(); // 値型
            var cap1 = Capture<string>.OfEmpty(); // 参照型

            // Assert
            Assert.That(cap0.HasValue, Is.False);
            Assert.That(cap1.HasValue, Is.False);
            Assert.That(cap0.Value.Count(), Is.EqualTo(0));
            Assert.That(cap1.Value.Count(), Is.EqualTo(0));
        }
        [Test]
        [Description("OfSingle - Case #1 - 値を1つだけ持つインスタンスを返すこと")]
        public void OfSingle_Case1()
        {
            // Arrange
            // Act
            var cap0 = Capture<int>.OfSingle(123); // 値型
            var cap1 = Capture<string>.OfSingle("123"); // 参照型

            // Assert
            Assert.That(cap0.HasValue, Is.True);
            Assert.That(cap1.HasValue, Is.True);
            Assert.That(cap0.Value.Count(), Is.EqualTo(1));
            Assert.That(cap1.Value.Count(), Is.EqualTo(1));
        }
        [Test]
        [Description("OfMultiple - Case #1 - 値を複数（引数で指定した数）持つインスタンスを返すこと")]
        public void OfMultiple_Case1()
        {
            // Arrange
            // Act
            var cap0 = Capture<int>.Of(123, 456, 789); // 値型
            var cap1 = Capture<string>.Of("123", "456"); // 参照型

            // Assert
            Assert.That(cap0.HasValue, Is.True);
            Assert.That(cap1.HasValue, Is.True);
            Assert.That(cap0.Value.Count(), Is.EqualTo(3));
            Assert.That(cap1.Value.Count(), Is.EqualTo(2));
            Assert.That(cap0.Value.ToArray(), Is.EqualTo(new int[] { 123, 456, 789 }));
            Assert.That(cap1.Value.ToArray(), Is.EqualTo(new string[] { "123", "456" }));
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
            var cap0 = Capture<int>.Of(123, 456);
            var cap1 = Capture<string>.OfSingle("123");

            // Act
            var cap0_map = cap0.Map(a => a.ToString());
            var cap1_map = cap1.Map(a => int.Parse(a));

            // Assert
            Assert.That(cap0_map.HasValue, Is.True);
            Assert.That(cap1_map.HasValue, Is.True);
            Assert.That(cap0_map.Value.ToArray(), Is.EqualTo(new string[] { "123", "456" }));
            Assert.That(cap1_map.Value.ToArray(), Is.EqualTo(new int[] { 123 }));
        }
        [Test]
        [Description("ToString - Case #1 - 要素0、要素1、要素2以上、それぞれインスタンス内容を示す文字列が返されること")]
        public void ToString_Case1()
        {
            // Arrange
            var cap0 = new Capture<string>();
            var cap1 = Capture<string>.OfSingle("hello");
            var cap2 = Capture<int>.Of(123, 456);

            // Act
            // Assert
            Assert.That(cap0.ToString(), Is.EqualTo("Capture()"));
            Assert.That(cap1.ToString(), Is.EqualTo("Capture(hello)"));
            Assert.That(cap2.ToString(), Is.EqualTo("Capture(123, 456)"));
        }
        [Test]
        [Description("ForEarch - Case #1 - 要素0の場合、何も実行されないこと")]
        public void ForEach_Case1()
        {
            // Arrange
            var cap0 = new Capture<string>();
            var chk0 = 0;
            var chk1 = 0;

            // Act
            cap0.ForEach(e => chk0++);
            cap0.ForEach((e, i) => chk1++);

            // Assert
            Assert.That(chk0, Is.EqualTo(0));
            Assert.That(chk1, Is.EqualTo(0));
        }
        [Test]
        [Description("ForEarch - Case #2 - 要素1以上の場合、要素数分アクション実行されること")]
        public void ForEach_Case2()
        {
            // Arrange
            var cap0 = Capture<int>.Of(1, 2, 3);
            var chk0 = 0;
            var chk1 = 0;

            // Act
            // Assert
            cap0.ForEach(e =>
            {
                Assert.That(e == ++chk0, Is.True);
            });
            cap0.ForEach((e, i) => // 第1引数：要素、第2引数：添字
            {
                Assert.That(chk1 == i, Is.True);
                Assert.That(e == ++chk1, Is.True);
            });

            Assert.That(chk0, Is.EqualTo(3));
            Assert.That(chk1, Is.EqualTo(3));
        }
        [Test]
        [Description("FirstOrElse - Case #1 - 要素があればそれを返し、なければデフォルト値を返すこと")]
        public void FirstOrElse_Case1()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(Capture<int>.OfEmpty().FirstOrElse(123), Is.EqualTo(123));
            Assert.That(Capture<int>.Of().FirstOrElse(123), Is.EqualTo(123));
            Assert.That(Capture<int>.Of(1, 2, 3).FirstOrElse(123), Is.EqualTo(1));
        }
        [Test]
        [Description("LastOrElse - Case #1 - 要素があればそれを返し、なければデフォルト値を返すこと")]
        public void LastOrElse_Case1()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(Capture<int>.OfEmpty().LastOrElse(123), Is.EqualTo(123));
            Assert.That(Capture<int>.Of().LastOrElse(123), Is.EqualTo(123));
            Assert.That(Capture<int>.Of(1, 2, 3).LastOrElse(123), Is.EqualTo(3));
        }
        [Test]
        [Description("GetOrElse - Case #1 - 要素があればそれを返し、なければデフォルト値を返すこと")]
        public void GetOrElse_Case1()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(Capture<int>.OfEmpty().GetOrElse(0, 123), Is.EqualTo(123));
            Assert.That(Capture<int>.OfSingle(1).GetOrElse(0, 123), Is.EqualTo(1));
            Assert.That(Capture<int>.OfSingle(1).GetOrElse(1, 123), Is.EqualTo(123));
            Assert.That(Capture<int>.Of().GetOrElse(0, 123), Is.EqualTo(123));
            Assert.That(Capture<int>.Of(1, 2, 3).GetOrElse(0, 123), Is.EqualTo(1));
            Assert.That(Capture<int>.Of(1, 2, 3).GetOrElse(2, 123), Is.EqualTo(3));
            Assert.That(Capture<int>.Of(1, 2, 3).GetOrElse(3, 123), Is.EqualTo(123));
        }
        [Test]
        [Description("Add - Case #1 - 要素を加えたインスタンスを返すこと")]
        public void Add_Case1()
        {
            // Arrange
            var c0 = Capture<int>.OfEmpty().Add(123);
            var c1 = Capture<int>.OfSingle(1).Add(123);
            var c2 = Capture<int>.Of(1, 2, 3).Add(123);

            // Act
            // Assert
            Assert.That(c0.Last(), Is.EqualTo(123));
            Assert.That(c1.Last(), Is.EqualTo(123));
            Assert.That(c2.Last(), Is.EqualTo(123));
            Assert.That(c0.Count(), Is.EqualTo(1));
            Assert.That(c1.Count(), Is.EqualTo(2));
            Assert.That(c2.Count(), Is.EqualTo(4));
        }
        [Test]
        [Description("Union - Case #1 - 要素を統合したインスタンスを返すこと")]
        public void Union_Case1(
            [Values(new int[0], new []{ 1 }, new []{ 1, 2, 3 })] int[] leftVal,
            [Values(new int[0], new[] { 1 }, new[] { 1, 2, 3 })]  int[] rightVal)
        {
            // Arrange
            var left = Capture<int>.Of(leftVal);
            var right = Capture<int>.Of(rightVal);

            // Act
            var result = left.Union(right);

            // Assert
            Assert.That(result.Count(), Is.EqualTo(leftVal.Length + rightVal.Length));
            Assert.That(result.ToArray(), Is.EqualTo(leftVal.Concat(rightVal).ToArray()));
        }
    }
}
