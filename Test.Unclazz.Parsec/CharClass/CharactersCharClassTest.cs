using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unclazz.Parsec.CharClasses;

namespace Test.Unclazz.Parsec.CharClass
{
    [TestFixture]
    public class CharactersCharClassTest
    {
        [TestCase(new[] { 'a', 'b', 'c' }, 'a', true)]
        [TestCase(new[] { 'a', 'b', 'c' }, 'b', true)]
        [TestCase(new[] { 'a', 'b', 'c' }, 'c', true)]
        [TestCase(new[] { 'a', 'b', 'c' }, 'd', false)]
        [TestCase(new char[0], 'a', false)]
        [TestCase(new char[0], 'd', false)]
        public void Contains_Case1(char[] chars, char checkTaget, bool checkResult)
        {
            // Arrange
            var clazz0 = new CharactersCharClass(chars);

            // Act
            // Assert
            Assert.That(clazz0.Contains(checkTaget), Is.EqualTo(checkResult));
        }
        [TestCase(new[] { 'a', 'b', 'c' }, 'a')]
        [TestCase(new[] { 'a', 'b', 'c' }, 'd')]
        [TestCase(new char[0], 'b')]
        public void Plus_Case1(char[] chars, char checkTaget)
        {
            // Arrange
            var clazz0 = new CharactersCharClass(chars);

            // Act
            var clazz1 = clazz0.Plus(checkTaget);

            // Assert
            Assert.That(clazz1.Contains(checkTaget), Is.EqualTo(true));
        }
        [TestCase(new[] { 'a', 'b' }, new[] { 'c' }, 'a', true)]
        [TestCase(new[] { 'a', 'b' }, new[] { 'c' }, 'c', true)]
        [TestCase(new[] { 'a', 'b' }, new[] { 'd' }, 'c', false)]
        [TestCase(new[] { 'a', 'b' }, new char[] { }, 'c', false)]
        public void Union_Case1(char[] chars0, char[] chars1, char checkTaget, bool checkResult)
        {
            // Arrange
            var clazz0 = new CharactersCharClass(chars0);
            var clazz1 = new CharactersCharClass(chars1);

            // Act
            var clazz2 = clazz0.Union(clazz1);

            // Assert
            Assert.That(clazz2.Contains(checkTaget), Is.EqualTo(checkResult));
        }
    }
}
