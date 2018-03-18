﻿using System;
using Armature;
using Armature.Core;
using Armature.Extensibility;
using Armature.Framework;

namespace Tests.Extensibility.MaybePropagation.Implementation
{
  public static class Extension
  {
    public const int GetMaybeValueStage = 4;

    /// <summary>
    /// Specifies what unit should be built to fill <see cref="Maybe{T}"/> value.
    /// </summary>
    public static TreatSugar<T> TreatMaybeValue<T>(this TreatSugar<Maybe<T>> treatSugar)
    {
      var treat = treatSugar.AsUnitSequenceExtensibility();
      var uniqueToken = Guid.NewGuid();
      treat.UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new BuildMaybeAction<T>(uniqueToken));
      return new TreatSugar<T>(treat.UnitSequenceMatcher.AddOrGetUnitMatcher(new WildcardUnitSequenceMatcher(Match.Type<T>(uniqueToken), 0)));
    }

    /// <summary>
    /// Specifies that value from built <see cref="Maybe{T}"/> should be used as a unit.
    /// </summary>
    public static TreatSugar<Maybe<T>> AsMaybeValueOf<T>(this TreatSugar<T> treatSugar)
    {
      var treat = treatSugar.AsUnitSequenceExtensibility();
      return new TreatSugar<Maybe<T>>(treat.UnitSequenceMatcher.AddBuildAction(GetMaybeValueStage, new GetMaybeValueBuildAction<T>()));
    }
  }
}