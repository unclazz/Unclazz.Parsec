using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec;
using Unclazz.Parsec.Intrinsics;

namespace Test.Unclazz.Parsec.Intrinsics
{
    [TestFixture]
    public class RepeatConfigurationTest
    {
        [Test]
        public void Ctor_Case01_NoArgs()
        {
            // Arrange
            // Act
            var c = new RepeatConfiguration();

            // Assert
            Assert.That(c.Breakable, Is.True);
            Assert.That(c.Maximum, Is.EqualTo(int.MaxValue));
            Assert.That(c.Minimal, Is.EqualTo(0));
            Assert.That(c.Separator, Is.Null);
        }
        [Test]
        public void Ctor_Case11_Exactly()
        {
            // Arrange
            // Act
            var c = new RepeatConfiguration(exactly: 5);

            // Assert
            Assert.That(c.Breakable, Is.False);
            Assert.That(c.Maximum, Is.EqualTo(5));
            Assert.That(c.Minimal, Is.EqualTo(5));
            Assert.That(c.Separator, Is.Null);
        }
        [Test]
        public void Ctor_Case12_Exactly_MustBeGreaterThan0()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => new RepeatConfiguration(exactly: 0),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
            Assert.That(() => new RepeatConfiguration(exactly: -2),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
        }
        [Test]
        public void Ctor_Case21_MinMax()
        {
            // Arrange
            // Act
            var c = new RepeatConfiguration(min: 5, max: 10);

            // Assert
            Assert.That(c.Breakable, Is.True);
            Assert.That(c.Maximum, Is.EqualTo(10));
            Assert.That(c.Minimal, Is.EqualTo(5));
            Assert.That(c.Separator, Is.Null);
        }
        [Test]
        public void Ctor_Case22_MinMax_SameValue()
        {
            // Arrange
            // Act
            var c = new RepeatConfiguration(min: 5, max: 5);

            // Assert
            Assert.That(c.Breakable, Is.False);
            Assert.That(c.Maximum, Is.EqualTo(5));
            Assert.That(c.Minimal, Is.EqualTo(5));
            Assert.That(c.Separator, Is.Null);
        }
        [Test]
        public void Ctor_Case23_MinMax_OneSideOnly()
        {
            // Arrange
            // Act
            var c1 = new RepeatConfiguration(min: 5);
            var c2 = new RepeatConfiguration(max: 5);

            // Assert
            Assert.That(c1.Breakable, Is.True);
            Assert.That(c1.Maximum, Is.EqualTo(int.MaxValue));
            Assert.That(c1.Minimal, Is.EqualTo(5));
            Assert.That(c1.Separator, Is.Null);
            Assert.That(c2.Breakable, Is.True);
            Assert.That(c2.Maximum, Is.EqualTo(5));
            Assert.That(c2.Minimal, Is.EqualTo(0));
            Assert.That(c2.Separator, Is.Null);
        }
        [Test]
        public void Ctor_Case24_MinMax_Inverted()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => new RepeatConfiguration(min: 6, max: 5),
                Throws.InstanceOf<ArgumentOutOfRangeException>());
        }
        [Test]
        public void Ctor_Case31_HasSeparator()
        {
            // Arrange
            // Act
            var c = new RepeatConfiguration(sep: Parsers.Char('a'));

            // Assert
            Assert.That(c.Breakable, Is.True);
            Assert.That(c.Maximum, Is.EqualTo(int.MaxValue));
            Assert.That(c.Minimal, Is.EqualTo(0));
            Assert.That(c.Separator, Is.Not.Null);
        }
    }
}
