using System;
using Armature;
using Armature.Core;
using Armature.Framework;

namespace Tests.Extensibility.MaybePropagation.Extension
{
  public static class Extension
  {
    /// <summary>
    /// Sugar for registering a build plan of <see cref="Maybe{T}"/>. See usages for details
    /// </summary>
    public static TreatMaybeSugar TreatMaybe(this BuildPlansCollection container) { return new TreatMaybeSugar(container); }

    public static IParameterMatcherSugar UseMaybePropagation<T>(this ParameterMatcherSugar<T> sugar, object token = null)
    {
      ((IParameterMatcherSugar)sugar).BuildAction = new AskMaybeAction<T>(token);
      return sugar;
    }

    
    public class TreatMaybeSugar
    {
      private readonly BuildPlansCollection _container;
      
      public TreatMaybeSugar(BuildPlansCollection container) { _container = container; }

      public TreatSugar<T> Of<T>()
      {
        var uniqueToken = Guid.NewGuid();
        var maybeMatcher = new WeakUnitSequenceMatcher(Match.Type<Maybe<T>>(null), 0)
          .AddBuildAction(BuildStage.Create, new BuildMaybeAction<T>(uniqueToken), 0);

        _container.AddUnitMatcher(maybeMatcher);

        return _container.Treat<T>(uniqueToken);
      }
    }
  }
}