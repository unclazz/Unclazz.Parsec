using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec;
using Unclazz.Parsec.CharClass;

namespace Test.Unclazz.Parsec.CharClass
{
    [TestFixture]
    public class CharRangesCharClassTest
    {
        [TestCase('b', 'y', 'a', false)]
        [TestCase('b', 'y', 'b', true)]
        [TestCase('b', 'y', 'c', true)]
        [TestCase('b', 'y', 'x', true)]
        [TestCase('b', 'y', 'y', true)]
        [TestCase('b', 'y', 'z', false)]
        public void Contains_Case1(char start, char end, char checkTarget, bool result)
        {
            // Arrange
            var range = CharRange.Between(start, end);
            var clazz = CharRangesCharClass.AnyOf(range);

            // Act
            // Assert
            Assert.That(clazz.Contains(checkTarget), Is.EqualTo(result));
            Assert.That(clazz.Contains(checkTarget), Is.EqualTo(result));
        }
        [TestCase('b', 'y', '1', '8', 'a', false)]
        [TestCase('b', 'y', '1', '8', 'b', true)]
        [TestCase('b', 'y', '1', '8', 'y', true)]
        [TestCase('b', 'y', '1', '8', 'z', false)]
        [TestCase('b', 'y', '1', '8', '0', false)]
        [TestCase('b', 'y', '1', '8', '1', true)]
        [TestCase('b', 'y', '1', '8', '8', true)]
        [TestCase('b', 'y', '1', '8', '9', false)]
        public void Contains_Case2(char range0Start, char range0End, char range1Start, char range1End, char checkTarget, bool result)
        {
            // Arrange
            var range0 = CharRange.Between(range0Start, range0End);
            var range1 = CharRange.Between(range1Start, range1End);
            var clazz = CharRangesCharClass.AnyOf(range0, range1);

            // Act
            // Assert
            Assert.That(clazz.Contains(checkTarget), Is.EqualTo(result));
            Assert.That(clazz.Contains(checkTarget), Is.EqualTo(result));
        }
        [TestCase('b', 'y', '1', '8', 'a', false)]
        [TestCase('b', 'y', '1', '8', 'b', true)]
        [TestCase('b', 'y', '1', '8', 'y', true)]
        [TestCase('b', 'y', '1', '8', 'z', false)]
        [TestCase('b', 'y', '1', '8', '0', false)]
        [TestCase('b', 'y', '1', '8', '1', true)]
        [TestCase('b', 'y', '1', '8', '8', true)]
        [TestCase('b', 'y', '1', '8', '9', false)]
        public void Or_Case1(char range0Start, char range0End, char range1Start, char range1End, char checkTarget, bool result)
        {
            // Arrange
            var range0 = CharRange.Between(range0Start, range0End);
            var range1 = CharRange.Between(range1Start, range1End);
            var clazz0 = CharRangesCharClass.AnyOf(range0);

            // Act
            var clazz1 = clazz0.Union(CharRangesCharClass.AnyOf(range1));

            // Assert
            Assert.That(clazz1.Contains(checkTarget), Is.EqualTo(result));
            Assert.That(clazz1.Contains(checkTarget), Is.EqualTo(result));
        }
        [TestCase('b', 'y', '1', '8', 'a', false)]
        [TestCase('b', 'y', '1', '8', 'b', true)]
        [TestCase('b', 'y', '1', '8', 'y', true)]
        [TestCase('b', 'y', '1', '8', 'z', false)]
        [TestCase('b', 'y', '1', '8', '0', false)]
        [TestCase('b', 'y', '1', '8', '1', true)]
        [TestCase('b', 'y', '1', '8', '8', true)]
        [TestCase('b', 'y', '1', '8', '9', false)]
        public void Plus_Case1(char range0Start, char range0End, char range1Start, char range1End, char checkTarget, bool result)
        {
            // Arrange
            var range0 = CharRange.Between(range0Start, range0End);
            var range1 = CharRange.Between(range1Start, range1End);
            var clazz0 = CharRangesCharClass.AnyOf(range0);

            // Act
            var clazz1 = clazz0.Plus(range1);

            // Assert
            Assert.That(clazz1.Contains(checkTarget), Is.EqualTo(result));
            Assert.That(clazz1.Contains(checkTarget), Is.EqualTo(result));
        }
        [TestCase('b', 'y', '1', '8', 'a')]
        [TestCase('b', 'y', '1', '8', 'b')]
        [TestCase('b', 'y', '1', '8', 'y')]
        [TestCase('b', 'y', '1', '8', 'z')]
        [TestCase('b', 'y', '1', '8', '0')]
        [TestCase('b', 'y', '1', '8', '1')]
        [TestCase('b', 'y', '1', '8', '8')]
        [TestCase('b', 'y', '1', '8', '9')]
        public void Plus_Case2(char range0Start, char range0End, char range1Start, char range1End, char checkTarget)
        {
            // Arrange
            var range0 = CharRange.Between(range0Start, range0End);
            var range1 = CharRange.Between(range1Start, range1End);
            var clazz0 = CharRangesCharClass.AnyOf(range0);

            // Act
            var clazz1 = clazz0.Plus(checkTarget);

            // Assert
            Assert.That(clazz1.Contains(checkTarget), Is.True);
        }
    }
}
