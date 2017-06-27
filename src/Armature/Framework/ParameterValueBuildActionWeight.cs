using System.Diagnostics.CodeAnalysis;

namespace Armature.Framework
{
  /// <summary>
  /// Matching weight for different build steps building values for a parameters
  /// </summary>
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  public class ParameterValueBuildActionWeight
  {
    // ReSharper disable once MemberCanBePrivate.Global (introduced to use in inheritors)
    protected const int Step = 100;

    public const int Lowest = 0;
    
    /// <summary>
    /// <see cref="WeakParameterTypeValueBuildStep"/> 
    /// </summary>
    public const int FreeValueResolver = Lowest + Step;
    
    /// <summary>
    /// <see cref="StrictParameterTypeValueBuildStep"/>
    /// </summary>
    public const int TypedParameterResolver = FreeValueResolver + Step;
    
    /// <summary>
    /// <see cref="AttributedParameterResolver"/>
    /// </summary>
    public const int AttributedParameterResolver = TypedParameterResolver + Step;
    
    /// <summary>
    /// <see cref="NamedParameterValueBuildStep"/>
    /// </summary>
    public const int NamedParameterResolver = AttributedParameterResolver + Step;
  }
}