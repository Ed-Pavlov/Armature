using System;
using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  /// Describes a unit to build. <see cref="IUnitSequenceMatcher" /> matches with passed collection of <see cref="UnitInfo" />
  /// </summary>
  [Serializable]
  public readonly struct UnitInfo
  {
    public readonly object? Id;
    public readonly object? Token;

    [DebuggerStepThrough]
    public UnitInfo(object? id, object? token)
    {
      if (id == null && token == null) throw new ArgumentNullException(nameof(id), @"Either id or token should be provided");

      Id = id;
      Token = token;
    }
    
    public override string ToString() => string.Format("{0}:{1}", Id.ToLogString(), Token.ToLogString());
  }
}