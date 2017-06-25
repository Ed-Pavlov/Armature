using Armature.Common;
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

      var startIndex = 1;
      var arrayTail = array.GetTail(startIndex);

      // --assert
      Assert.That(arrayTail.Length, Is.EqualTo(arrayLength - startIndex));
    }

    [Test]
    public void GetValue()
    {
      // --arrange
      var array = new []{0, 1, 2, 3};

      const int startIndex = 1;
      var arrayTail = array.GetTail(startIndex);

      var expected = new int[arrayTail.Length];
      for (var i = startIndex; i < array.Length; i++)
        expected[i - startIndex] = array[i];

      // --act
      var actual = new int[arrayTail.Length];
      for (var i = 0; i < arrayTail.Length; i++)
        actual[i] = arrayTail[i];

      // --assert
      Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void LastItem()
    {
      // --arrange
      const int lastItem = 23;
      var array = new []{0, 1, 2, lastItem};

      const int startIndex = 1;
      var arrayTail = array.GetTail(startIndex);

      // --assert
      Assert.That(arrayTail.GetLastItem(), Is.EqualTo(lastItem));
    }
  }
}