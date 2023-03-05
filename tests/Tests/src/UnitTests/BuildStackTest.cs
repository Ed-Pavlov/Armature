using System;
using System.Linq;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests;

public class BuildStackTest
{
  [Test]
  public void target_unit_should_not_depend_on_tail()
  {
    // arrange
    var expected    = Unit.Of("Argument");
    var firstOfTail = Unit.Of("ParamInfo");

    var array = new[] {Unit.Of("Interface"), Unit.Of("Class"), firstOfTail, expected};
    var stack = new BuildSession.Stack(array).GetTail(1);

    // act, assert
    stack.TargetUnit.Should().Be(expected);
  }

  [Test]
  public void items_order()
  {
    var expected = new[] {Unit.Of(0), Unit.Of(1), Unit.Of(2), Unit.Of(3)};

    // arrange
    var stack = new BuildSession.Stack(expected.Reverse().ToArray());

    // act, assert
    stack.AsEnumerable().Should().BeEquivalentTo(expected);
  }

  [Test]
  public void get_tail()
  {
    // arrange
    var array = new[] {Unit.Of("Interface"), Unit.Of("Class"), Unit.Of("ParamInfo"), Unit.Of("Argument")};
    var stack = new BuildSession.Stack(array);

    // act
    var tail = stack.GetTail(1);

    // assert
    tail.AsEnumerable().Should().BeEquivalentTo(array.Reverse().Skip(1));
  }

  [Test]
  public void length_of_tail()
  {
    // --arrange
    const int arrayLength    = 3;
    const int tailStartIndex = 1;

    var array     = new UnitId[arrayLength];
    var arrayTail = new BuildSession.Stack(array).GetTail(tailStartIndex);

    // --assert
    arrayTail.Length.Should().Be(arrayLength - tailStartIndex);
  }

  [Test]
  public void should_allow_default()
  {
    // --arrange
    var actual = default(BuildSession.Stack);

    // --assert
    actual.Should().BeOfType<BuildSession.Stack>();
  }

  [Test]
  public void should_not_allow_default_ctor()
  {
    // --arrange
    var actual = () => new BuildSession.Stack();

    // --assert
    actual.Should().ThrowExactly<ArgumentException>();
  }

  [Test]
  public void should_check_array_argument()
  {
    // --arrange
    var actual = () => new BuildSession.Stack(null!);

    // --assert
    actual.Should().ThrowExactly<ArgumentNullException>().WithParameterName("array");
  }

  [Test]
  public void should_check_start_index_argument([Values(-3, 24)] int startIndex)
  {
    var array = new[] {Unit.Of(0), Unit.Of(2, 0)};

    // --arrange
    startIndex = Math.Min(startIndex, array.Length + 1);
    var actual = () => new BuildSession.Stack(array).GetTail(startIndex);

    // --assert
    actual.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("startIndex");
  }
}
