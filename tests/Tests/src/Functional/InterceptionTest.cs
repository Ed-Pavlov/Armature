using System;
using System.Collections;
using System.Reflection;
using Armature;
using Armature.Core;
using Armature.Core.BuildActions.Constructor;
using NUnit.Framework;
using JetBrains.Annotations;

namespace Tests.Functional
{
  public class InterceptionTest
  {
    private const string Expected = "expected string value";
    private const string Postfix  = ".postfix";

    [TestCaseSource(nameof(InterceptUnitTestCases))]
    public void InterceptUnit(Action<Builder> makeRegistration)
    {
      // --arrange
      // create container with another one stage in the very beginning of conveyer
      var target = CreateTarget();

      target
       .Treat<StringConsumer>()
       .AsIs()
       .UsingParameters(Expected);

      // register AddPostfixToString buildAction for any string on the very first stage
      // (postprocessing will be called last and buildAction will add a postfix to created or cached string

      target
       .AddSubQuery(new SkipToLastUnit())
       .AddSubQuery(new IfLastUnit(new IsParameterOfTypeStringPattern()))
       .UseBuildAction(BuildStage.Intercept, new AddPostfixToString(Postfix));

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
               .AsCreatedWith(_ => Expected);
            }))
       .SetName("RegisteredAsFactoryMethod");
    }

    private static Builder CreateTarget()
      => new(BuildStage.Intercept, BuildStage.Cache, BuildStage.Create)
         {
           new SkipToLastUnit
           {
             new IfLastUnit(IsConstructorPattern.Instance)
              .UseBuildAction(BuildStage.Create, GetLongestConstructorBuildAction.Instance)
           }
         };

    /// <summary>
    ///   GetBuildAction with any string not depending on key
    /// </summary>
    private class IsParameterOfTypeStringPattern : IUnitIdPattern
    {
      public bool Matches(UnitId unitId)
      {
        var type = unitId.Kind is ParameterInfo parameterInfo ? parameterInfo.ParameterType : null;

        return type == typeof(string);
      }

      public bool Equals(IUnitIdPattern other) => throw new NotSupportedException();
    }

    /// <summary>
    ///   BuildAction adds a postfix to a string, should be registered only for strings
    /// </summary>
    private class AddPostfixToString : IBuildAction
    {
      private readonly string _postfix;

      public AddPostfixToString([NotNull] string postfix)
      {
        if(postfix is null) throw new ArgumentNullException(nameof(postfix));

        _postfix = postfix;
      }

      public void Process(IBuildSession buildSession) { }

      public void PostProcess(IBuildSession buildSession)
      {
        var assembleResult = buildSession.BuildResult;
        var value          = (string) assembleResult.Value;
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
