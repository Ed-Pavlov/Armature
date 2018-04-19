namespace Armature.Core.BuildActions.Constructor
{
  /// <summary>
  /// "Builds" a constructor Unit of the currently building Unit marked with <see cref="InjectAttribute"/> with
  /// specified <see cref="InjectAttribute.InjectionPointId"/>  
  /// </summary>
  public class GetInjectPointConstructorBuildAction : GetConstructorBytAttributeBuildAction<InjectAttribute>
  {
    public GetInjectPointConstructorBuildAction(object injectPointId = null) : base(inject => Equals(inject.InjectionPointId, injectPointId))
    {
    }
  }
}