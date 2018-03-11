using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Framework;
using JetBrains.Annotations;

namespace Armature
{
  public class TreatOpenGenericSugar
  {
    private readonly BuildPlansCollection _container;
    private readonly IUnitSequenceMatcher _unitSequenceMatcher;

    [DebuggerStepThrough]
    public TreatOpenGenericSugar([NotNull] IUnitSequenceMatcher unitSequenceMatcher, [NotNull] BuildPlansCollection container)
    {
      if (unitSequenceMatcher == null) throw new ArgumentNullException(nameof(unitSequenceMatcher));
      if (container == null) throw new ArgumentNullException(nameof(container));

      _unitSequenceMatcher = unitSequenceMatcher;
      _container = container;
    }

    /// <param name="openGenericType"></param>
    /// <param name="addDefaultCreateAction">
    ///   If <see cref="AddCreationBuildStep.Yes" /> adds a build step
    ///   <see cref="Default.CreationBuildAction" /> for <see cref="UnitInfo" />(<paramref name="openGenericType" />, null) as a creation build step.
    /// </param>
    public AdjusterSugar As(Type openGenericType, AddCreationBuildStep addDefaultCreateAction) => As(openGenericType, null, addDefaultCreateAction);

    /// <param name="openGenericType"></param>
    /// <param name="token"></param>
    /// <param name="addDefaultCreateAction">
    ///   If <see cref="AddCreationBuildStep.Yes" /> adds a build step
    ///   <see cref="Default.CreationBuildAction" /> for <see cref="UnitInfo" />(<paramref name="openGenericType" />, <see cref="token" />)
    ///   as a creation build step.
    /// </param>
    public AdjusterSugar As(Type openGenericType, object token = null, AddCreationBuildStep addDefaultCreateAction = AddCreationBuildStep.Yes)
    {
      _unitSequenceMatcher.AddBuildAction(BuildStage.Redirect, new RedirectOpenGenericTypeBuildAction(openGenericType, token), 0);

      var nextBuildStep = _unitSequenceMatcher;
      if (addDefaultCreateAction == AddCreationBuildStep.Yes)
      {
        nextBuildStep = new WeakUnitSequenceMatcher(Match.OpenGenericType(openGenericType, token), UnitSequenceMatchingWeight.WeakMatchingOpenGenericUnit);

        _unitSequenceMatcher
          .AddOrGetUnitMatcher(nextBuildStep)
          .AddBuildAction(BuildStage.Create, Default.CreationBuildAction, 0);
      }

      return new AdjusterSugar(nextBuildStep, _container);
    }
  }
}