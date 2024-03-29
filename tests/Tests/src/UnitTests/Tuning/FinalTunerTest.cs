using System;
using System.Linq;
using Armature;
using Armature.Core;
using Armature.Core.Sdk;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Tests.Util;

namespace Tests.UnitTests.Tuning;

public class FinalTunerTest
{
  [Test]
  public void AsSingleton()
  {
    var expectedBuildActions = Util.CreateBag(BuildStage.Cache, new Singleton());

    // --arrange
    var actual = new Util.TestBuildChainPattern();
    var target = new FinalTuner(actual);

    // --act
    target.AsSingleton();

    // // --assert
    actual.BuildActions.Should().BeEquivalentTo(expectedBuildActions);
  }

  [Test]
  public void inject_into_should_call_all_passed_tuners()
  {
    var expectedNode = new BuildChainPatternTree();

    // --arrange
    var tuner1 = A.Dummy<IInjectPointTuner>();
    var tuner2 = A.Dummy<IInjectPointTuner>();
    var target = new FinalTuner(expectedNode);

    // --act
    target.InjectInto(tuner1, tuner2);

    // --assert
    A.CallTo(() => tuner1.Tune(expectedNode, 0))
     .MustHaveHappenedOnceAndOnly()
     .Then(A.CallTo(() => tuner2.Tune(expectedNode, 0)).MustHaveHappenedOnceAndOnly());
  }

  [Test]
  public void inject_into_should_throw_if_no_tuner_passed()
  {
    // --arrange
    var target = new FinalTuner(new BuildChainPatternTree());

    // --act
    var actual = () => target.InjectInto();

    // --assert
    actual.Should().ThrowExactly<ArgumentNullException>().WithMessage("Specify one or more inject point tuners *");
  }

  [Test]
  public void using_arguments_should_add_registrations()
  {
    const string expectedString = "expected";
    const int    expectedInt    = 312;

    var tuner1 = A.Dummy<IArgumentTuner>();
    var tuner2 = A.Dummy<IArgumentTuner>();

    var expectedWeight = WeightOf.BuildChainPattern.IfFirstUnit + WeightOf.InjectionPoint.ByTypeAssignability;

    var expectedChildNode =
      new SkipWhileUnit(Static.Of<IsServiceUnit>())
      {
        Children =
        {
          new IfFirstUnit(new IsAssignableFromType(expectedString.GetType()), expectedWeight)
           .UseBuildAction(new Instance<object>(expectedString), BuildStage.Cache),
          new IfFirstUnit(new IsAssignableFromType(expectedInt.GetType()), expectedWeight)
           .UseBuildAction(new Instance<object>(expectedInt), BuildStage.Cache)
        }
      };

    // --arrange
    var actual = new Util.TestBuildChainPattern();
    var target = new FinalTuner(actual);

    // --act
    target.UsingArguments(tuner1, expectedString, expectedInt, tuner2);

    // --assert
    A.CallTo(() => tuner1.Tune(actual, 0)).MustHaveHappenedOnceAndOnly().Then(
      A.CallTo(() => tuner2.Tune(actual, 0)).MustHaveHappenedOnceAndOnly());

    actual.Children.Single().Should().BeEquivalentTo(expectedChildNode);
  }

  [Test]
  public void using_arguments_should_throw_if_not_argument_tuner_is_passed()
  {
    // --arrange
    var target = new FinalTuner(new BuildChainPatternTree());

    // --act
    var actual = () => target.UsingArguments(A.Dummy<ITuner>());

    // --assert
    actual.Should().ThrowExactly<ArgumentException>().WithMessage($"{nameof(IArgumentTuner)} or instance expected");
  }
}