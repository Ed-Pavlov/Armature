using System;
using System.Collections.Generic;
using System.Text;
using Armature;
using Armature.Core;
using FluentAssertions;
using NUnit.Framework;

namespace Tests.Functional
{
  public class WeightOfTest
  {
    [Test]
    public void subtype_pattern_should_be_greater_than_open_generic() => WeightOf.SubtypePattern.Should().BeGreaterThan(WeightOf.OpenGenericPattern);
    
    [Test]
    public void strict_pattern_should_be_greater_than_subtype() => WeightOf.StrictPattern.Should().BeGreaterThan(WeightOf.SubtypePattern);

    [Test]
    public void find_unit_should_be_greater_than_skip_to_last_unit_with_any_pattern()
      => WeightOf.FindUnit.Should().BeGreaterThan(WeightOf.SkipToLastUnit | byte.MaxValue);

    [Test]
    public void first_unit_should_be_greater_than_find_unit_with_any_pattern()
      => WeightOf.FirstUnit.Should().BeGreaterThan(WeightOf.FindUnit | byte.MaxValue);

    [Test]
    public void test()
    {
      var createByReflection1 = Static<CreateByReflection>.Instance;
      Console.WriteLine(createByReflection1);
      Console.WriteLine(Static<GetConstructorWithMaxParametersCount>.Instance);
      var createByReflection2 = Static<CreateByReflection>.Instance;
      Console.WriteLine(createByReflection2);
      
      Console.WriteLine(ReferenceEquals(createByReflection1, createByReflection2));
    }
  }
}
