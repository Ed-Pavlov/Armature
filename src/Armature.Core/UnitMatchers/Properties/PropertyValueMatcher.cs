using System.Diagnostics;
using System.Reflection;
using Armature.Core.Logging;

namespace Armature.Core.UnitMatchers.Properties
{
  /// <summary>
  /// Matches Unit representing "value for property" of the currently building Unit
  /// </summary>
  public class PropertyValueMatcher : IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new PropertyValueMatcher();

    private PropertyValueMatcher() { }

    public bool Matches(UnitInfo unitInfo) => unitInfo.Token == SpecialToken.InjectValue && unitInfo.Id is PropertyInfo;

    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();

    #region Equality
    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher other) => ReferenceEquals(this, other);
    
    [DebuggerStepThrough]
    public override bool Equals(object obj) => Equals(obj as IUnitMatcher);

    [DebuggerStepThrough]
    public override int GetHashCode() => 0;
    #endregion
  }
}