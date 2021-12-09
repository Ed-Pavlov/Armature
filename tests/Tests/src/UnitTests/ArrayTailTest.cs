using System;
using Armature.Core;
using Armature.Core.Sdk;
using FluentAssertions;
using NUnit.Framework;
using Tests.Common;

namespace Tests.UnitTests;

public class ArrayTailTest
{
  [Test]
  public void inline()
  {
    var target = new ArrayTail<int>(new[] {1, 2, 34, 6, 3}, 0);

    foreach(var i in target)
    {

    }
  }

  [Test]
  public void Length()
  {
    // --arrange
    const int arrayLength = 3;
    const int startIndex  = 1;

    var array     = new int[arrayLength];
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

    for(var i = startIndex; i < array.Length; i++)
      expected[i - startIndex] = array[i];

    // --act
    var actual = array.GetTail(startIndex);

    // --assert
    var actualArray = new int[actual.Length];

    for(var i = 0; i < actual.Length; i++)
      actualArray[i] = actual[i];

    actualArray.Should().Equal(expected);
  }

  [Test]
  public void LastItem()
  {
    // --arrange
    const int startIndex = 2;
    const int lastItem   = 23;

    var array = new[] {0, 1, 2, lastItem};

    // --act
    var actual = array.GetTail(startIndex);

    // --assert
    actual.Last().Should().Be(lastItem);
  }

  [Test]
  public void should_allow_default()
  {
    // --arrange
    var actual = default(ArrayTail<string>);

    // --assert
    actual.Should().BeOfType<ArrayTail<string>>();
  }
  [Test]
  public void should_not_allow_default_ctor()
  {
    // --arrange
    var actual = () => new ArrayTail<string>();

    // --assert
    actual.Should().ThrowExactly<ArgumentException>();
  }

  [Test]
  public void should_check_array_argument()
  {
    // --arrange
    var actual = () => new ArrayTail<string>(null!, 4);

    // --assert
    actual.Should().ThrowExactly<ArgumentNullException>().WithParameterName("array");
  }

  [Test]
  public void should_check_start_index_argument([Values(-3, 24)] int startIndex)
  {
    var array = new[] {"1", "2"};

    // --arrange
    startIndex = Math.Min(startIndex, array.Length);
    var actual = () => new ArrayTail<string>(array, startIndex);

    // --assert
    actual.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("startIndex");
  }
}