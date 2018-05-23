using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Core.UnitSequenceMatcher;
using Armature.Extensibility;
using Resharper.Annotations;

namespace Armature
{
  public class TreatingOpenGenericTuner : UnitSequenceExtensibility
  {
    [DebuggerStepThrough]
    public TreatingOpenGenericTuner([NotNull] IUnitSequenceMatcher unitSequenceMatcher) : base(unitSequenceMatcher) { }

    /// <param name="openGenericType"></param>
    /// <param name="addDefaultCreateAction">
    ///   If <see cref="AddCreateBuildAction.Yes" /> adds a build action <see cref="Default.CreationBuildAction" /> for
    ///   <see cref="UnitInfo" />(<paramref name="openGenericType" />, null) as a creation build action.
    /// </param>
    public Tuner As(Type openGenericType, AddCreateBuildAction addDefaultCreateAction) => As(openGenericType, null, addDefaultCreateAction);

    /// <param name="openGenericType"></param>
    /// <param name="token"></param>
    /// <param name="addDefaultCreateAction">
    ///   If <see cref="AddCreateBuildAction.Yes" /> adds a build action
    ///   <see cref="Default.CreationBuildAction" /> for <see cref="UnitInfo" />(<paramref name="openGenericType" />, <paramref name="token" />)
    ///   as a creation build action.
    /// </param>
    public Tuner As(Type openGenericType, object token = null, AddCreateBuildAction addDefaultCreateAction = AddCreateBuildAction.Yes)
    {
      UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new RedirectOpenGenericTypeBuildAction(openGenericType, token));

      var childMatcher = UnitSequenceMatcher;
      if (addDefaultCreateAction == AddCreateBuildAction.Yes)
      {
        childMatcher = new WildcardUnitSequenceMatcher(Match.OpenGenericType(openGenericType, token), UnitSequenceMatchingWeight.WildcardMatchingUnit - 1);

        UnitSequenceMatcher
          .AddOrGetUnitSequenceMatcher(childMatcher)
          .AddBuildAction(BuildStage.Create, Default.CreationBuildAction);
      }

      return new Tuner(childMatcher);
    }

    public Tuner AsIs()
    {
      UnitSequenceMatcher.AddBuildAction(BuildStage.Create, Default.CreationBuildAction);
      return new Tuner(UnitSequenceMatcher);
    }
  }
}