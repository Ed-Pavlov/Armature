using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Armature;
using Armature.Core;
using Armature.Core.Sdk;
using NUnit.Framework;

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
       .UsingArguments(Expected);

      // register AddPostfixToString buildAction for any string on the very first stage
      // (postprocessing will be called last and buildAction will add a postfix to created or cached string

      target
       // .GetOrAddNode(new SkipAllUnits())
       .AddNode(new IfFirstUnit(new StringParameterPattern()))
       .UseBuildAction(new AddPostfixToString(Postfix), BuildStage.Intercept);

      // --act
      var actual = target.Build<StringConsumer>()!;

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
               .UsingArguments(Expected)))
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
      => new("test", BuildStage.Intercept, BuildStage.Cache, BuildStage.Create)
         {
             new IfFirstUnit(new IsConstructor())
              .UseBuildAction(Static.Of<GetConstructorWithMaxParametersCount>(), BuildStage.Create),

             new IfFirstUnit(new IsParameterInfoArray())
              .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),

             new IfFirstUnit(new IsParameterInfo())
              .UseBuildAction(new BuildArgumentByParameterType(), BuildStage.Create),
         };

    /// <summary>
    /// Checks if <see cref="UnitId.Kind"/> is <see cref="ParameterInfo"/> with <see cref="ParameterInfo.ParameterType"/> is <see cref="string"/>
    /// </summary>
    private class StringParameterPattern : IUnitPattern
    {
      public bool Matches(UnitId unitId) => unitId.Kind is ParameterInfo parameterInfo && parameterInfo.ParameterType == typeof(string);
    }

    /// <summary>
    /// BuildAction adds a postfix to a string, should be registered only for strings
    /// </summary>
    private class AddPostfixToString : IBuildAction
    {
      private readonly string _postfix;

      public AddPostfixToString(string postfix) => _postfix = postfix ?? throw new ArgumentNullException(nameof(postfix));

      public void Process(IBuildSession buildSession) { }

      public void PostProcess(IBuildSession buildSession)
      {
        var buildResult = buildSession.BuildResult;
        if(buildResult.HasValue)
        {
          var value = (string) buildResult.Value!;
          buildSession.BuildResult = new BuildResult(value + _postfix);
        }
      }
    }

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
    private class StringConsumer
    {
      public readonly string Value;

      public StringConsumer(string value) => Value = value;
    }
  }
}