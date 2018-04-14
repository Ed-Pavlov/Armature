using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Core.UnitSequenceMatcher;
using Resharper.Annotations;

namespace Armature
{
  public class TreatingOpenGenericTuner
  {
    private readonly IUnitSequenceMatcher _unitSequenceMatcher;

    [DebuggerStepThrough]
    public TreatingOpenGenericTuner([NotNull] IUnitSequenceMatcher unitSequenceMatcher)
    {
      if (unitSequenceMatcher == null) throw new ArgumentNullException(nameof(unitSequenceMatcher));

      _unitSequenceMatcher = unitSequenceMatcher;
    }

    /// <param name="openGenericType"></param>
    /// <param name="addDefaultCreateAction">
    ///   If <see cref="AddCreateBuildAction.Yes" /> adds a build action <see cref="Default.CreationBuildAction" /> for
    /// <see cref="UnitInfo" />(<paramref name="openGenericType" />, null) as a creation build action.
    /// </param>
    public Tuner As(Type openGenericType, AddCreateBuildAction addDefaultCreateAction) => As(openGenericType, null, addDefaultCreateAction);

    /// <param name="openGenericType"></param>
    /// <param name="token"></param>
    /// <param name="addDefaultCreateAction">
    ///   If <see cref="AddCreateBuildAction.Yes" /> adds a build action
    ///   <see cref="Default.CreationBuildAction" /> for <see cref="UnitInfo" />(<paramref name="openGenericType" />, <see cref="token" />)
    ///   as a creation build action.
    /// </param>
    public Tuner As(Type openGenericType, object token = null, AddCreateBuildAction addDefaultCreateAction = AddCreateBuildAction.Yes)
    {
      _unitSequenceMatcher.AddBuildAction(BuildStage.Create, new RedirectOpenGenericTypeBuildAction(openGenericType, token));

      var childMatcher = _unitSequenceMatcher;
      if (addDefaultCreateAction == AddCreateBuildAction.Yes)
      {
        childMatcher = new WildcardUnitSequenceMatcher(Match.OpenGenericType(openGenericType, token), UnitSequenceMatchingWeight.WildcardMatchingUnit - 1);

        _unitSequenceMatcher
          .AddOrGetUnitSequenceMatcher(childMatcher)
          .AddBuildAction(BuildStage.Create, Default.CreationBuildAction);
      }

      return new Tuner(childMatcher);
    }
  }
}