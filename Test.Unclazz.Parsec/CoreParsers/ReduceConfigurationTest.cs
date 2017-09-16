using NUnit.Framework;
using System;
using Unclazz.Parsec.CoreParsers.RepeatAggregate;

namespace Test.Unclazz.Parsec.CoreParsers
{
    [TestFixture]
    public class ReduceConfigurationTest
    {
        [Test]
        public void Ctor_Case1_3Args()
        {
            // Arrange
            // Act
            var c = new ReduceConfiguration<bool, int, string>
                (() => 1, (int a, bool b) => a + (b ? 1 : 0), a => a.ToString());

            // Assert
            Assert.That(c.SeedFactory(), Is.EqualTo(1));
            Assert.That(c.Accumulator(2, true), Is.EqualTo(3));
            Assert.That(c.ResultSelector(3), Is.EqualTo("3"));
        }
        [Test]
        public void Ctor_Case2_NullArg()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => new ReduceConfiguration<bool, int, string>
                (null, (int a, bool b) => a + (b ? 1 : 0), a => a.ToString()),
                Throws.InstanceOf<ArgumentNullException>());
            Assert.That(() => new ReduceConfiguration<bool, int, string>
                (() => 1, null, a => a.ToString()),
                Throws.InstanceOf<ArgumentNullException>());
            Assert.That(() => new ReduceConfiguration<bool, int, string>
                (() => 1, (int a, bool b) => a + (b ? 1 : 0), null),
                Throws.InstanceOf<ArgumentNullException>());
        }
        [Test]
        public void Ctor_Case3_2Args()
        {
            // Arrange
            // Act
            var c = new ReduceConfiguration<bool, bool, string>
                ((bool a, bool b) => a && b, a => a.ToString());

            // Assert
            Assert.That(c.SeedFactory, Is.Null);
            Assert.That(c.Accumulator(false, true), Is.False);
            Assert.That(c.ResultSelector(true), Is.EqualTo(true.ToString()));
        }
        [Test]
        public void Ctor_Case4_2Args_BadTypeParams()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => new ReduceConfiguration<int, bool, string>
                ((bool a, int b) => a && b == 1, a => a.ToString()),
                Throws.InstanceOf<ArgumentNullException>());
        }
    }
}
