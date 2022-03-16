using Armature.Core;
using Armature.Core.Sdk;

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
    public IArgumentTuner InDirectOrder { get; } = new ArgumentTuner(
      (node, weight) =>
        node
         .GetOrAddNode(new IfFirstUnit(Static.Of<IsParameterInfoList>(), weight))
         .UseBuildAction(Static.Of<BuildMethodArgumentsInDirectOrder>(), BuildStage.Create));
  }

  public class ByParam
  {
    private const short ByNameWeight = 10;
    private const short ByTypeWeight = 5;

    /// <summary>
    /// Adds the build action which builds an argument using method parameter type as a <see cref="UnitId.Kind"/>
    /// </summary>
    public IArgumentTuner Type { get; } = new ArgumentTuner(
      (node, weight) =>
        node
         .GetOrAddNode(new IfFirstUnit(Static.Of<IsParameterInfo>(), weight + WeightOf.BuildChainPattern.IfFirstUnit + ByTypeWeight))
         .UseBuildAction(Static.Of<BuildArgumentByParameterType>(), BuildStage.Create));

    /// <summary>
    /// Adds the build action which builds an argument using method parameter name as a <see cref="UnitId.Kind"/>
    /// </summary>
    public IArgumentTuner Name { get; } = new ArgumentTuner(
      (node, weight) =>
        node
         .GetOrAddNode(new IfFirstUnit(Static.Of<IsParameterInfo>(), weight + WeightOf.BuildChainPattern.IfFirstUnit + ByNameWeight))
         .UseBuildAction(Static.Of<BuildArgumentByParameterName>(), BuildStage.Create));
  }

  public class ByProperty { }
}