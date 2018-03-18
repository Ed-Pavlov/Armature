using System.Diagnostics;
using Armature.Core;
using Armature.Logging;

namespace Armature.Framework.Properties
{
  public class PropertyMatcher : IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new PropertyMatcher();

    private PropertyMatcher()
    {
    }

    public bool Matches(UnitInfo unitInfo) => unitInfo.Token == SpecialToken.Property && unitInfo.GetUnitTypeSafe() != null;

    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();

    #region Equality
    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher obj) => obj is PropertyMatcher;

    [DebuggerStepThrough]
    public override bool Equals(object obj) => Equals(obj as IUnitMatcher);

    public override int GetHashCode() => 0;
    #endregion
  }
}