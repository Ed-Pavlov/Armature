using Armature;
using Armature.Core;
using Armature.Framework;
using Armature.Interface;
using FluentAssertions;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Tests.Functional
{
  public class AutoWiringTest
  {
    [Test]
    public void inject_pointid_should_be_treated_as_token()
    {
      const string expected = "expected 09765";
      
      // --arrange
      var target = CreateTarget();

      target
        .Treat<Consumer>()
        .AsIs();
      
      target
        .Treat<string>(Consumer.PointId)
        .AsInstance(expected);
      
      target
        .Treat<string>()
        .AsInstance("lsdjfkl");
      
      // --act
      var actual = target.Build<Consumer>();
      
      // --assert
      actual.StringValue.Should().Be(expected, "Because expected value registered with the Inject Point Id value as Token");
    }

    [Test]
    public void explicit_parameter_value_should_have_advantage()
    {
      const string expected = "expected 09765";
      
      // --arrange
      var target = FunctionalTestHelper.CreateBuilder();
      var rootBuildStep = target.AddOrGetBuildStep(new AnyUnitBuildStep());
      rootBuildStep.AddBuildStep(new BuildValueForInjectPointParameterUsingTokenBuildStep(ParameterValueBuildStepWeight.Lowest + 1));

      target
        .Treat<Consumer>()
        .AsIs()
        .UsingParameters(
          For.ParameterId(Consumer.PointId).UseValue(expected));
      
      target
        .Treat<string>(Consumer.PointId)
        .AsInstance("938754");
      
      target
        .Treat<string>()
        .AsInstance("lsdjfkl");
      
      // --act
      var actual = target.Build<Consumer>();
      
      // --assert
      actual.StringValue.Should().Be(expected, "Because expected value registered UsingParameters for Consumer");
    }
    
    private static Builder CreateTarget()
    {
      var target = FunctionalTestHelper.CreateBuilder();
      var rootBuildStep = target.AddOrGetBuildStep(new AnyUnitBuildStep());
      rootBuildStep.AddBuildStep(new BuildValueForInjectPointParameterUsingTokenBuildStep(ParameterValueBuildStepWeight.Lowest + 1));
      return target;
    }
    
    [UsedImplicitly]
    private class Consumer
    {
      public readonly string StringValue;
      public const string PointId = "PointId 9387";
      public Consumer([Inject(PointId)] string stringValue)
      {
        StringValue = stringValue;
      }
    }
  }
}