using System;
using System.Linq;
using Armature.Core;
using Armature.Core.Sdk;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests;

public class BuildChainTest
{
  [Test]
  public void target_unit_should_not_depend_on_tail()
  {
    // arrange
    var expected    = Kind.Is("Argument");
    var firstOfTail = Kind.Is("ParamInfo");

    var array      = new[] {Kind.Is("Interface"), Kind.Is("Class"), firstOfTail, expected};
    var buildChain = new BuildChain(array).GetTail(1);

    // act, assert
    buildChain.TargetUnit.Should().Be(expected);
  }

  [Test]
  public void items_order()
  {
    var expected = new[] {Kind.Is(0), Kind.Is(1), Kind.Is(2), Kind.Is(3)};

    // arrange
    var buildChain = new BuildChain(expected.Reverse().ToArray());

    // act, assert
    buildChain.Should().BeEquivalentTo(expected);
  }

  [Test]
  public void get_tail()
  {
    // arrange
    var array      = new[] {Kind.Is("Interface"), Kind.Is("Class"), Kind.Is("ParamInfo"), Kind.Is("Argument")};
    var buildChain = new BuildChain(array);

    // act
    var tail = buildChain.GetTail(1);

    // assert
    tail.Should().BeEquivalentTo(array.Reverse().Skip(1));
  }

  [Test]
  public void length_of_tail()
  {
    // --arrange
    const int arrayLength    = 3;
    const int tailStartIndex = 1;

    var array     = new UnitId[arrayLength];
    var arrayTail = new BuildChain(array).GetTail(tailStartIndex);

    // --assert
    arrayTail.Length.Should().Be(arrayLength - tailStartIndex);
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
    var actual = () => new BuildChain(null!);

    // --assert
    actual.Should().ThrowExactly<ArgumentNullException>().WithParameterName("array");
  }

  [Test]
  public void should_check_start_index_argument([Values(-3, 24)] int startIndex)
  {
    var array = new UnitId[] {new(0, null), new (2, 0)};

    // --arrange
    startIndex = Math.Min(startIndex, array.Length + 1);
    var actual = () => new BuildChain(array).GetTail(startIndex);

    // --assert
    actual.Should().ThrowExactly<ArgumentOutOfRangeException>().WithParameterName("startIndex");
  }
}
