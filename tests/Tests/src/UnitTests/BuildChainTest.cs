using System;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests;

public class BuildChainTest
{
  [Test]
  public void Length()
  {
    // --arrange
    const int arrayLength = 3;
    const int startIndex  = 1;

    var array     = new UnitId[arrayLength];
    var arrayTail = array.ToBuildChain().GetTail(startIndex);

    // --assert
    arrayTail.Length.Should().Be(arrayLength - startIndex);
  }

  [Test]
  public void Content()
  {
    const int startIndex = 2;

    // --arrange
    var array = new UnitId[] {new(0, null), new(1, null), new (2, 0), new(3, 0)};

    var expected = new UnitId[array.Length - startIndex];

    for(var i = startIndex; i < array.Length; i++)
      expected[i - startIndex] = array[i];

    // --act
    var actual = array.ToBuildChain().GetTail(startIndex);

    // --assert
    var actualArray = new UnitId[actual.Length];

    for(var i = 0; i < actual.Length; i++)
      actualArray[i] = actual[i];

    actualArray.Should().Equal(expected);
  }

  [Test]
  public void LastItem()
  {
    // --arrange
    const int startIndex = 2;
    var       lastItem   = new UnitId(23, null);

    var array = new UnitId[] {new(0, null), new(1, null), new (2, 0), lastItem};

    // --act
    var actual = array.ToBuildChain().GetTail(startIndex);

    // --assert
    actual.TargetUnit.Should().Be(lastItem);
  }

  [Test]
  public void should_allow_default()
  {
    // --arrange
    var actual = default(BuildChain);

    // --assert
    actual.Should().BeOfType<BuildChain>();
  }

  [Test]
  public void should_not_allow_default_ctor()
  {
    // --arrange
    var actual = () => new BuildChain();

    // --assert
    actual.Should().ThrowExactly<ArgumentException>();
  }

  [Test]
  public void should_check_array_argument()
  {
    // --arrange
    var actual = () => new BuildChain(null!, 4);

    // --assert
    actual.Should().ThrowExactly<ArgumentNullException>().WithParameterName("array");
  }

  [Test]
  public void should_check_start_index_argument([Values(-3, 24)] int startIndex)
  {
    var array = new UnitId[] {new(0, null), new (2, 0)};

    // --arrange
    startIndex = Math.Min(startIndex, array.Length);
    var actual = () => new BuildChain(array, startIndex);

    // --assert
    actual.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("startIndex");
  }
}