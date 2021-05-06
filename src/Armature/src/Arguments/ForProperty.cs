using System.Reflection;
using Armature.Core;

namespace Armature
{
  public static class ForProperty
  {
    /// <summary>
    ///   Matches with property with <see cref="PropertyInfo.PropertyType" /> equals to <typeparamref name="T" />
    /// </summary>
    public static ArgumentStaticTuner<T> OfType<T>()
      => new(parentNode =>
             {
               parentNode.TunePropertyListBuilding(new GetPropertyByTypeBuildAction(typeof(T)));
               return parentNode.GetOrAddNode(new IfLastUnitMatches(new PropertyByTypePattern(typeof(T), true), InjectPointMatchingWeight.TypedParameter));
             });

    /// <summary>
    ///   Matches with property with <see cref="MemberInfo.Name" /> equals to <paramref name="propertyName" />
    /// </summary>
    public static ArgumentStaticTuner Named(string propertyName)
      => new(parentNode =>
             {
               parentNode.TunePropertyListBuilding(new GetPropertyListByNameBuildAction(propertyName));
               return parentNode.GetOrAddNode(new IfLastUnitMatches(new PropertyWithNamePattern(propertyName), InjectPointMatchingWeight.NamedParameter));
             });

    /// <summary>
    ///   Matches with property marked with <see cref="InjectAttribute" />(<paramref name="injectPointId" />)
    /// </summary>
    public static ArgumentStaticTuner WithInjectPoint(object? injectPointId)
      => new(parentNode =>
             {
               parentNode.TunePropertyListBuilding(new GetPropertyListByInjectPointId(injectPointId));

               return parentNode.GetOrAddNode(
                 new IfLastUnitMatches(new PropertyWithInjectIdPattern(injectPointId), InjectPointMatchingWeight.AttributedParameter));
             });

    private static void TunePropertyListBuilding(this IPatternTreeNode patternTreeNode, IBuildAction buildAction)
      => patternTreeNode
        .GetOrAddNode(new IfLastUnitMatches(PropertiesListPattern.Instance))
        .UseBuildAction(buildAction, BuildStage.Create);
  }
}
