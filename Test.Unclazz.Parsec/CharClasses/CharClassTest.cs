using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec;

namespace Test.Unclazz.Parsec.CharClasses
{
    [TestFixture]
    public class CharClassTest
    {
        [Test]
        public void SpaceAndControl_Case1()
        {
            // Arrange
            var clazz = CharClass.SpaceAndControl;

            // Act
            // Assert
            Assert.That(clazz.Contains(' '), Is.True);
            Assert.That(clazz.Contains('\r'), Is.True);
            Assert.That(clazz.Contains('\n'), Is.True);
            Assert.That(clazz.Contains((char)0), Is.True);
            Assert.That(clazz.Contains((char)31), Is.True);
        }
    }
}
