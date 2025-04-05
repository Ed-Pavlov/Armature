using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Armature.Test.UnitTests;

public class HoconTest
{
  [Test]
  public void ComposeTest()
  {
    // --arrange
    object? tag = null;

    var expected = $"{{ {nameof(BuildArgumentByInjectPointNameBase)}{{ Tag: {tag.ToHoconString()} }} }}";

    // --act
    var actual = Hocon.Object<BuildArgumentByInjectPointNameBase>(("Tag", null));

    // --assert
    actual.Should().Be(expected);
  }

  [Test]
  public void ComposeTest2()
  {
    // --arrange
    object  unitKind = typeof(string);
    object? tag      = 56;

    var expected = $"{{ {nameof(UnitPattern)}{{ kind: {unitKind.ToHoconString()}, tag: {tag.ToHoconString()} }} }}";

    // --act
    var actual = Hocon.Object<UnitPattern>(("kind", unitKind), ("tag", tag));

    // --assert
    actual.Should().Be(expected);
  }

  [Test]
  public void ComposeArrayTest()
  {
    // --arrange
    var names   = new[] {"name1", "name2"};
    var expected = $"{{ {nameof(GetPropertyListByNames)}{{ Names: {names.ToHoconString()} }} }}";

    // --act
    var actual = Hocon.Object<GetPropertyListByNames>(("Names", names));

    // --assert
    actual.Should().Be(expected);
  }
}
