using System.Diagnostics.CodeAnalysis;

namespace Armature.Framework
{
  /// <summary>
  /// Matching weight for different parameter matchers
  /// </summary>
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  public class ParameterMatcherWeight
  {
    // ReSharper disable once MemberCanBePrivate.Global (introduced to use in inheritors)
    protected const int Step = 100;

    public const int Lowest = 0;
    
    /// <summary>
    /// <see cref="BuildWeakTypedParameterValueActionFactory"/> 
    /// </summary>
    public const int WeakTypedParameter = Lowest + Step;
    
    /// <summary>
    /// <see cref="BuildStrictTypedParameterValueActionFactory"/>
    /// </summary>
    public const int TypedParameter = WeakTypedParameter + Step;
    
    /// <summary>
    /// <see cref="BuildAttributedParameterValueActionFactory"/>
    /// </summary>
    public const int AttributedParameter = TypedParameter + Step;
    
    /// <summary>
    /// <see cref="BuildNamedParameterValueActionFactory"/>
    /// </summary>
    public const int NamedParameter = AttributedParameter + Step;
  }
}