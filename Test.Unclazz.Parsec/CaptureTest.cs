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
        public void Constructor_Case1()
        {
            // Arrange
            // Act
            var cap0 = new Capture<int>();
            var cap1 = new Capture<string>();

            // Assert
            Assert.That(cap0.HasValue, Is.False);
            Assert.That(cap1.HasValue, Is.False);
            Assert.That(() => cap0.Value, Throws.InstanceOf<InvalidOperationException>());
            Assert.That(() => cap1.Value, Throws.InstanceOf<InvalidOperationException>());
        }
        [Test]
        public void Constructor_Case2()
        {
            // Arrange
            // Act
            var cap0 = new Capture<int>(123);
            var cap1 = new Capture<string>("123");

            // Assert
            Assert.That(cap0.HasValue, Is.True);
            Assert.That(cap1.HasValue, Is.True);
            Assert.That(cap0.Value, Is.EqualTo(123));
            Assert.That(cap1.Value, Is.EqualTo("123"));
        }
        [Test]
        public void OrElse_Case1()
        {
            // Arrange
            var cap0 = new Capture<int>();
            var cap1 = new Capture<string>();

            // Act
            var val0 = cap0.OrElse(123);
            var val1 = cap1.OrElse("123");

            // Assert
            Assert.That(val0, Is.EqualTo(123));
            Assert.That(val1, Is.EqualTo("123"));
        }
        [Test]
        public void OrElse_Case2()
        {
            // Arrange
            var cap0 = new Capture<int>(123);
            var cap1 = new Capture<string>("123");

            // Act
            var val0 = cap0.OrElse(456);
            var val1 = cap1.OrElse("456");

            // Assert
            Assert.That(val0, Is.EqualTo(123));
            Assert.That(val1, Is.EqualTo("123"));
        }
        [Test]
        public void Map_Case1()
        {
            // Arrange
            var cap0 = new Capture<int>();
            var cap1 = new Capture<string>();

            // Act
            var cap0_map = cap0.Map(a => a.ToString());
            var cap1_map = cap1.Map(a => int.Parse(a));

            // Assert
            Assert.That(cap0_map.HasValue, Is.False);
            Assert.That(cap1_map.HasValue, Is.False);
        }
        [Test]
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
        public void IfHasValue_Case1()
        {
            // Arrange
            var cap0 = new Capture<int>();
            var cap1 = new Capture<string>();
            var val0 = 123;
            var val1 = "123";

            // Act
            cap0.IfHasValue(a => val0 = a);
            cap1.IfHasValue(a => val1 = a, () => val1 = "hello");

            // Assert
            Assert.That(val0, Is.EqualTo(123));
            Assert.That(val1, Is.EqualTo("hello"));
        }
        [Test]
        public void IfHasValue_Case2()
        {
            // Arrange
            var cap0 = new Capture<int>(123);
            var cap1 = new Capture<string>("123");
            var val0 = 456;
            var val1 = "456";

            // Act
            cap0.IfHasValue(a => val0 = a);
            cap1.IfHasValue(a => val1 = a, () => val1 = "hello");

            // Assert
            Assert.That(val0, Is.EqualTo(123));
            Assert.That(val1, Is.EqualTo("123"));
        }
        [Test]
        public void ToString_Case1()
        {
            // Arrange
            var cap0 = new Capture<string>();
            var cap1 = new Capture<string>("hello");

            // Act
            // Assert
            Assert.That(cap0.ToString(), Is.EqualTo("Capture(type = String)"));
            Assert.That(cap1.ToString(), Is.EqualTo("Capture(hello, type = String)"));
        }
    }
}
