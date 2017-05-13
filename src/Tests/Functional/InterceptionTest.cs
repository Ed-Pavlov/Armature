using System;
using System.Collections;
using System.Reflection;
using Armature;
using Armature.Core;
using Armature.Framework;
using JetBrains.Annotations;
using NUnit.Framework;
using UnitInfo = Armature.Core.UnitInfo;

namespace Tests.Functional
{
  public class InterceptionTest
  {
    private const int InterceptBuildStage = 1;
    private const string Expected = "expected string value";
    private const string Postfix = ".postfix";

    [TestCaseSource("InterceptUnitTestCases")]
    public void InterceptUnit(Action<Builder> makeRegistration)
    {
      // --arrange
      // create container with another one stage in the very beginning of conveyer
      var target = FunctionalTestHelper.CreateContainer(null, InterceptBuildStage, BuildStage.Cache, BuildStage.Redirect, BuildStage.Create);

      target
        .Treat<StringConsumer>()
        .AsIs()
        .UsingParameters(Expected);

      // register AddPostfixToString buildAction for any string on the very first stage
      // (postprocessing will be called last and buildAction will add a postfix to created or cached string
      var anyUnitBuildStep = new AnyUnitBuildStep();
      anyUnitBuildStep.AddChildBuildStep(new AnyStringBuildStep(InterceptBuildStage, new AddPostfixToString(Postfix)));
      target.AddBuildStep(anyUnitBuildStep);

      // --act
      var actual = target.Build<StringConsumer>();

      // --assert
      Assert.That(actual.Value, Is.EqualTo(Expected + Postfix));
    }

    private static IEnumerable InterceptUnitTestCases()
    {
      yield return new TestCaseData(new Action<Builder>(target =>
        target.Treat<StringConsumer>()
          .AsIs()
          .UsingParameters(Expected)))
        .SetName("RegisteredAsParameterValue");

      yield return new TestCaseData(new Action<Builder>(target =>
        {
          target
            .Treat<StringConsumer>()
            .AsIs();

          target
            .Treat<string>()
            .AsInstance(Expected);
        }))
        .SetName("RegisteredAsInstance");

      yield return new TestCaseData(new Action<Builder>(target =>
        {
          target
            .Treat<StringConsumer>()
            .AsIs();

          target
            .Treat<string>()
            .CreatedBy(_ => Expected);
        }))
        .SetName("RegisteredAsFactoryMethod");
    }

    /// <summary>
    /// GetBuildAction with any string not depending on token
    /// </summary>
    private class AnyStringBuildStep : LeafBuildStep
    {
      private readonly object _stage;
      private readonly IBuildAction _buildAction;

      public AnyStringBuildStep(object stage, [NotNull] IBuildAction buildAction) : base(0)
      {
        if (buildAction == null) throw new ArgumentNullException("buildAction");
        _stage = stage;
        _buildAction = buildAction;
      }

      protected override StagedBuildAction GetBuildAction(UnitInfo unitInfo)
      {
        var parameterInfo = unitInfo.Id as ParameterInfo;
        var type = parameterInfo == null ? null : parameterInfo.ParameterType;

        return type == typeof(string)
          ? new StagedBuildAction(_stage, _buildAction)
          : null;
      }
    }

    /// <summary>
    /// BuildAction adds a postfix to a string, should be registered only for strings
    /// </summary>
    private class AddPostfixToString : IBuildAction
    {
      private readonly string _postfix;

      public AddPostfixToString([NotNull] string postfix)
      {
        if (postfix == null) throw new ArgumentNullException("postfix");
        _postfix = postfix;
      }

      public void Execute(Build.Session buildSession)
      {}

      public void PostProcess(Build.Session buildSession)
      {
        var assembleResult = buildSession.BuildResult;
        var value = (string)assembleResult.Value;
        buildSession.BuildResult = new BuildResult(value + _postfix);
      }
    }

    [UsedImplicitly]
    class StringConsumer
    {
      public readonly string Value;

      public StringConsumer(string value)
      {
        Value = value;
      }
    }
  }
}