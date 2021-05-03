using System;
using System.Reflection;
using Armature.Core;
using Armature.Extensibility;

namespace Armature
{
  public class PropertyArgumentTuner : BuildActionExtensibility
  {
    public PropertyArgumentTuner(IUnitPattern unitIsProperty, IBuildAction getPropertyAction, int weight)
      : base(unitIsProperty, getPropertyAction, weight) { }

    /// <summary>
    ///   Inject the <paramref name="value" /> into the property
    /// </summary>
    public IArgumentTuner UseValue(object? value) => new TunerImpl(UnitPattern, BuildAction, new Instance<object>(value), Weight);

    /// <summary>
    ///   For building a value for the property use <see cref="PropertyInfo.PropertyType" /> and <paramref name="key" />
    /// </summary>
    public IArgumentTuner UseKey(object key)
    {
      if(key is null) throw new ArgumentNullException(nameof(key));

      return new TunerImpl(UnitPattern, BuildAction, new BuildArgumentForProperty(key), Weight);
    }

    /// <summary>
    ///   For building a value for the property use factory method />
    /// </summary>
    public IArgumentTuner UseFactoryMethod(Func<IBuildSession, object> factoryMethod)
      => new TunerImpl(UnitPattern, BuildAction, new CreateWithFactoryMethod<object>(factoryMethod), Weight);

    /// <summary>
    ///   For building a value for the property use <see cref="PropertyInfo.PropertyType" /> and <see cref="InjectAttribute.InjectionPointId" /> as a key
    /// </summary>
    public IArgumentTuner UseInjectPointIdAsKey() => new TunerImpl(UnitPattern, BuildAction, BuildArgumentForPropertyWithInjectPointIdAsKey.Instance, Weight);
      
    protected class TunerImpl : LastUnitTuner, IArgumentTuner
    {
      private readonly IBuildAction _getPropertyAction;

      public TunerImpl(IUnitPattern unitIsProperty, IBuildAction getPropertyAction, IBuildAction buildArgument, int weight)
        : base(unitIsProperty, buildArgument, weight)
        => _getPropertyAction = getPropertyAction ?? throw new ArgumentNullException(nameof(getPropertyAction));

      /// <summary>
      ///   In addition to the base logic adds a logic which provides a properties to inject into
      /// </summary>
      protected override void Apply(IPatternTreeNode patternTreeNode)
        => patternTreeNode
          .GetOrAddNode(new IfLastUnitMatches(PropertiesListPattern.Instance))
          .UseBuildAction(_getPropertyAction, BuildStage.Create);
    }
  }
}
