using Armature.Core;
using Armature.Framework;
using JetBrains.Annotations;

namespace Armature
{
  public static class For
  {
    public static ParameterBuildPlanner Parameter<T>()
    {
      return new TypeParameterBuildPlanner<T>();
    }

    public static ParameterBuildPlanner ParameterName(string parameterName)
    {
      return new NamedParameterBuildPlanner(parameterName);
    }

    public static ParameterBuildPlanner ParameterId([CanBeNull] object injectPointId)
    {
      return new MarkedParameterBuildPlanner(injectPointId);
    }

    public class ParameterBuildPlanner
    {
      public ParameterBuildPlanner UseValue([CanBeNull] object value)
      {
        BuildAction = new SingletonBuildAction(value);
        return this;
      }

      protected IBuildAction BuildAction { get; private set; }
    }

    private class TypeParameterBuildPlanner<T> : ParameterBuildPlanner, IParameterBuildPlanner
    {
      public void RegisterParameterResolver(BuildStepBase buildStep)
      {
        buildStep.AddChildBuildStep(new StrictParameterTypeValueBuildStep(typeof(T), BuildAction));
      }
    }

    private class NamedParameterBuildPlanner : ParameterBuildPlanner, IParameterBuildPlanner
    {
      private readonly string _parameterName;

      public NamedParameterBuildPlanner(string parameterName)
      {
        _parameterName = parameterName;
      }

      public void RegisterParameterResolver(BuildStepBase buildStep)
      {
        buildStep.AddChildBuildStep(new ParameterNameValueBuildStep(_parameterName, BuildAction));
      }
    }

    private class MarkedParameterBuildPlanner : ParameterBuildPlanner, IParameterBuildPlanner
    {
      [CanBeNull]
      private readonly object _injectPointId;

      public MarkedParameterBuildPlanner([CanBeNull] object injectPointId)
      {
        _injectPointId = injectPointId;
      }

      public void RegisterParameterResolver(BuildStepBase buildStep)
      {
        buildStep.AddChildBuildStep(new AttributedParameterValueBuildStep(_injectPointId, BuildAction));
      }
    }
  }
}