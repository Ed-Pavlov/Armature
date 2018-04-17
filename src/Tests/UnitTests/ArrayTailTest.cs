using Armature.Core.Common;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.UnitTests
{
  public class ArrayTailTest
  {
    [Test]
    public void Length()
    {
      // --arrange
      const int arrayLength = 3;
      var array = new int[arrayLength];

      const int startIndex = 1;
      var arrayTail = array.GetTail(startIndex);

      // --assert
      arrayTail.Length.Should().Be(arrayLength - startIndex);
    }

    [Test]
    public void Content()
    {
      const int startIndex = 2;
      
      // --arrange
      var array = new[] {0, 1, 2, 3};

      var expected = new int[array.Length - startIndex];
      for (var i = startIndex; i < array.Length; i++)
        expected[i - startIndex] = array[i];
      
      
      // --act
      var actual = array.GetTail(startIndex);

      // --assert
      actual.ShouldBeEquivalentTo(expected);
    }

    [Test]
    public void LastItem()
    {
      // --arrange
      const int startIndex = 2;
      const int lastItem = 23;
      var array = new[] {0, 1, 2, lastItem};

      // --act
      var actual = array.GetTail(startIndex);

      // --assert
      actual.Last().Should().Be(lastItem);
    }
  }
}