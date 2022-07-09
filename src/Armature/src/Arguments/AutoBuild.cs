using Armature.Core;
using Armature.Core.Sdk;
using Armature.Sdk;

namespace Armature;

/// <summary>
/// Shortcut methods to set rules how to build arguments for the constructor and method parameters.
/// </summary>
public static class AutoBuild
{
  private static ParamList? _methodArguments; // a little bit of optimization, don't create object if it's not needed
  public static  ParamList  MethodArguments => _methodArguments ??= Static.Of<ParamList>();

  public class ParamList
  {
    private ByParam? _byParam; // a little bit of optimization, don't create object if it's not needed
    public  ByParam  ByParameter => _byParam ??= Static.Of<ByParam>();

    /// <summary>
    /// Adds the build action which builds arguments for a method in the order as parameters specified in the method signature.
    /// </summary>
    public IArgumentTuner InDirectOrder { get; }
      = new ArgumentTuner(
        tuner =>
          tuner.TreeRoot
                       .GetOrAddNode(new IfFirstUnit(Static.Of<IsParameterInfoList>()))
                       .TryAddContext(tuner)
                       .UseBuildAction(Static.Of<BuildMethodArgumentsInDirectOrder>(), BuildStage.Create));
  }

  public class ByParam
  {
    /// <summary>
    /// Adds the build action which builds an argument using method parameter type as a <see cref="UnitId.Kind"/>
    /// </summary>
    public IArgumentTuner Type { get; } = new ArgumentTuner(
      tuner =>
        tuner.TreeRoot
                     .GetOrAddNode(
                        new IfFirstUnit(
                          Static.Of<IsParameterInfo>(),
                          WeightOf.InjectionPoint.ByExactType + WeightOf.BuildChainPattern.TargetUnit))
                     .TryAddContext(tuner)
                     .UseBuildAction(Static.Of<BuildArgumentByParameterType>(), BuildStage.Create));

    /// <summary>
    /// Adds the build action which builds an argument using method parameter name as a <see cref="UnitId.Kind"/>
    /// </summary>
    public IArgumentTuner Name { get; } = new ArgumentTuner(
      tuner =>
        tuner.TreeRoot
                     .GetOrAddNode(
                        new IfFirstUnit(
                          Static.Of<IsParameterInfo>(),
                          WeightOf.InjectionPoint.ByName + WeightOf.BuildChainPattern.TargetUnit))
                     .TryAddContext(tuner)
                     .UseBuildAction(Static.Of<BuildArgumentByParameterName>(), BuildStage.Create));
  }

  public class ByProperty { }
}