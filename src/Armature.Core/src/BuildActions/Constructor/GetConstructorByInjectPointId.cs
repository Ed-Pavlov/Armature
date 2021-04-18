namespace Armature.Core
{
  /// <summary>
  ///   Gets a constructor of type marked with <see cref="InjectAttribute" /> with the optional <see cref="InjectAttribute.InjectionPointId" />.
  /// </summary>
  public class GetConstructorByInjectPointId : GetConstructorBytAttribute<InjectAttribute>
  {
    public GetConstructorByInjectPointId(object? injectPointId = null) : base(inject => Equals(inject.InjectionPointId, injectPointId)) { }
  }
}
