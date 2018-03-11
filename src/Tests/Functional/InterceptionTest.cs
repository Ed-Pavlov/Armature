using System;
using System.Collections;
using System.Reflection;
using Armature;
using Armature.Core;
using Armature.Framework;
using JetBrains.Annotations;
using NUnit.Framework;

namespace Tests.Functional
{
  public class InterceptionTest
  {
    private const int InterceptBuildStage = 1;
    private const string Expected = "expected string value";
    private const string Postfix = ".postfix";

    [TestCaseSource(nameof(InterceptUnitTestCases))]
    public void InterceptUnit(Action<Builder> makeRegistration)
    {
      // --arrange
      // create container with another one stage in the very beginning of conveyer
      var target = FunctionalTestHelper.CreateBuilder(null, InterceptBuildStage, BuildStage.Cache, BuildStage.Redirect, BuildStage.Create);

      target
        .Treat<StringConsumer>()
        .AsIs()
        .UsingParameters(Expected);

      // register AddPostfixToString buildAction for any string on the very first stage
      // (postprocessing will be called last and buildAction will add a postfix to created or cached string

      target
        .AddOrGetUnitMatcher(new AnyUnitSequenceMatcher())
        .AddOrGetUnitMatcher(new LeafUnitSequenceMatcher(new AnyStringMatcher(), 0))
        .AddBuildAction(InterceptBuildStage, new AddPostfixToString(Postfix), 0);

      // --act
      var actual = target.Build<StringConsumer>();

      // --assert
      Assert.That(actual.Value, Is.EqualTo(Expected + Postfix));
    }

    private static IEnumerable InterceptUnitTestCases()
    {
      yield return new TestCaseData(
          new Action<Builder>(
            target =>
              target
                .Treat<StringConsumer>()
                .AsIs()
                .UsingParameters(Expected)))
        .SetName("RegisteredAsParameterValue");

      yield return new TestCaseData(
          new Action<Builder>(
            target =>
              {
                target
                  .Treat<StringConsumer>()
                  .AsIs();

                target
                  .Treat<string>()
                  .AsInstance(Expected);
              }))
        .SetName("RegisteredAsInstance");

      yield return new TestCaseData(
          new Action<Builder>(
            target =>
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
    ///   GetBuildAction with any string not depending on token
    /// </summary>
    private class AnyStringMatcher : IUnitMatcher
    {
      public bool Matches(UnitInfo unitInfo)
      {
        var parameterInfo = unitInfo.Id as ParameterInfo;
        var type = parameterInfo == null ? null : parameterInfo.ParameterType;
        return type == typeof(string);
      }

      public bool Equals(IUnitMatcher other) => throw new NotImplementedException();
    }

    /// <summary>
    ///   BuildAction adds a postfix to a string, should be registered only for strings
    /// </summary>
    private class AddPostfixToString : IBuildAction
    {
      private readonly string _postfix;

      public AddPostfixToString([NotNull] string postfix)
      {
        if (postfix == null) throw new ArgumentNullException(nameof(postfix));

        _postfix = postfix;
      }

      public void Process(IBuildSession buildSession) { }

      public void PostProcess(IBuildSession buildSession)
      {
        var assembleResult = buildSession.BuildResult;
        var value = (string)assembleResult.Value;
        buildSession.BuildResult = new BuildResult(value + _postfix);
      }
    }

    [UsedImplicitly]
    private class StringConsumer
    {
      public readonly string Value;

      public StringConsumer(string value) => Value = value;
    }
  }
}