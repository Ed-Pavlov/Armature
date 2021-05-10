using System.Reflection;
using Armature.Core;

namespace Armature
{
  public static class ForProperty
  {
    /// <summary>
    ///   Matches with property with <see cref="PropertyInfo.PropertyType" /> equals to <typeparamref name="T" />
    /// </summary>
    public static PropertyArgumentTuner<T> OfType<T>()
      => new(parentNode =>
             {
               parentNode.TunePropertyListBuilding(new GetPropertyByType(typeof(T)));
               return parentNode.GetOrAddNode(new IfLastUnit(new IsPropertyWithType(typeof(T), true), InjectPointMatchingWeight.TypedParameter));
             });

    /// <summary>
    ///   Matches with property with <see cref="MemberInfo.Name" /> equals to <paramref name="propertyName" />
    /// </summary>
    public static PropertyArgumentTuner<object?> Named(string propertyName)
      => new(parentNode =>
             {
               parentNode.TunePropertyListBuilding(new GetPropertyListByNames(propertyName));
               return parentNode.GetOrAddNode(new IfLastUnit(new IsPropertyNamed(propertyName), InjectPointMatchingWeight.NamedParameter));
             });

    /// <summary>
    ///   Matches with property marked with <see cref="InjectAttribute" />(<paramref name="injectPointId" />)
    /// </summary>
    public static PropertyArgumentTuner<object?> WithInjectPoint(object? injectPointId)
      => new(parentNode =>
             {
               parentNode.TunePropertyListBuilding(new GetPropertyListByInjectPointId(injectPointId));

               return parentNode.GetOrAddNode(
                 new IfLastUnit(new IsPropertyInfoWithAttribute(injectPointId), InjectPointMatchingWeight.AttributedParameter));
             });

    private static void TunePropertyListBuilding(this IPatternTreeNode patternTreeNode, IBuildAction buildAction)
      => patternTreeNode
        .GetOrAddNode(new IfLastUnit(IsPropertyList.Instance))
        .UseBuildAction(buildAction, BuildStage.Create);
  }
}
