using System.Diagnostics.CodeAnalysis;

namespace Armature.Framework
{
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  public class ParameterValueBuildActionWeight
  {
    // ReSharper disable once MemberCanBePrivate.Global (introduced to use in inheritors)
    protected const int Step = 100;

    public const int FreeValueResolver = Step;
    public const int TypedParameterResolver = FreeValueResolver + Step;
    public const int AttributedParameterResolver = TypedParameterResolver + Step;
    public const int NamedParameterResolver = AttributedParameterResolver + Step;
  }
}