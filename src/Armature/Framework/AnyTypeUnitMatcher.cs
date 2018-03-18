using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Armature.Core;
using Armature.Logging;

namespace Armature.Framework
{
  public class AnyTypeUnitMatcher : IUnitMatcher
  {
    public static readonly IUnitMatcher Instance = new AnyTypeUnitMatcher();
    
    private AnyTypeUnitMatcher()
    {
    }

    public bool Matches(UnitInfo unitInfo) 
    {
      var type = unitInfo.GetUnitTypeSafe();
      return !unitInfo.Token.IsSpecial() && type != null && !type.IsAbstract && !type.IsInterface && !type.IsGenericTypeDefinition;
    }

    [DebuggerStepThrough]
    public override string ToString() => GetType().GetShortName();

    #region Equality
    [DebuggerStepThrough]
    public bool Equals(IUnitMatcher other) => ReferenceEquals(this, other);

    [DebuggerStepThrough]
    public override bool Equals(object obj) => Equals(obj as AnyTypeUnitMatcher );

    [DebuggerStepThrough]
    public override int GetHashCode() => 0;
    #endregion
  }
}