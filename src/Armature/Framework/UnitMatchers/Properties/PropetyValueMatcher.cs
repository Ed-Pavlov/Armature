using System.Diagnostics;
using System.Reflection;
using Armature.Core;
using Armature.Logging;

namespace Armature.Framework.UnitMatchers.Properties
{
  public class PropetyValueMatcher : IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new PropetyValueMatcher();

    private PropetyValueMatcher() { }

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