using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec;
using Unclazz.Parsec.CharClasses;

namespace Test.Unclazz.Parsec.CharClasses
{
    [TestFixture]
    public class CharRangeTest
    {
        [TestCase('a', 'z')]
        [TestCase('0', '9')]
        [TestCase('z', 'a')]
        [TestCase('9', '0')]
        public void Between_Case1(char start, char end)
        {
            // Arrange
            var less = start <= end ? start : end;
            var greater = start <= end ? end : start;

            // Act
            var r = CharRange.Between(start, end);

            // Assert
            Assert.That(r.Start, Is.EqualTo(less));
            Assert.That(r.End, Is.EqualTo(greater));
        }
        [TestCase('b', 'y', 'a', false)]
        [TestCase('b', 'y', 'b', true)]
        [TestCase('b', 'y', 'c', true)]
        [TestCase('b', 'y', 'x', true)]
        [TestCase('b', 'y', 'y', true)]
        [TestCase('b', 'y', 'z', false)]
        public void Contains_Case1(char start, char end, char checkTarget, bool result)
        {
            // Arrange
            var r = CharRange.Between(start, end);

            // Act
            // Assert
            Assert.That(r.Contains(checkTarget), Is.EqualTo(result));
        }
    }
}
