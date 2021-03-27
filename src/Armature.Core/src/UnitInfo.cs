using System;
using System.Diagnostics;
using Armature.Core.Logging;
using JetBrains.Annotations;

namespace Armature.Core
{
  /// <summary>
  /// Describes a unit to build. <see cref="IUnitSequenceMatcher" /> matches with passed collection of <see cref="UnitInfo" />
  /// </summary>
  [Serializable]
  public readonly struct UnitInfo
  {
    [CanBeNull]
    public readonly object Id;
    [CanBeNull]
    public readonly object Token;

    [DebuggerStepThrough]
    public UnitInfo([CanBeNull] object id, [CanBeNull] object token)
    {
      if (id == null && token == null) throw new ArgumentNullException(nameof(id), @"Either id or token should be provided");

      Id = id;
      Token = token;
    }
    
    public override string ToString() => string.Format("{0}:{1}", Id.ToLogString(), Token.ToLogString());
  }
}