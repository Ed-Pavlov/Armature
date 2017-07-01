using System.Diagnostics.CodeAnalysis;

namespace Armature.Framework
{
  /// <summary>
  /// Matching weight for different build steps building values for a parameters
  /// </summary>
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  public class ParameterValueBuildStepWeight
  {
    // ReSharper disable once MemberCanBePrivate.Global (introduced to use in inheritors)
    protected const int Step = 100;

    public const int Lowest = 0;
    
    /// <summary>
    /// <see cref="WeakParameterTypeValueBuildStep"/> 
    /// </summary>
    public const int WeakTypedParameter = Lowest + Step;
    
    /// <summary>
    /// <see cref="StrictParameterTypeValueBuildStep"/>
    /// </summary>
    public const int TypedParameter = WeakTypedParameter + Step;
    
    /// <summary>
    /// <see cref="AttributedParameterValueBuildStep"/>
    /// </summary>
    public const int AttributedParameter = TypedParameter + Step;
    
    /// <summary>
    /// <see cref="NamedParameterValueBuildStep"/>
    /// </summary>
    public const int NamedParameter = AttributedParameter + Step;
  }
}