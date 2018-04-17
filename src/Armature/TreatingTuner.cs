using System;
using Armature.Core;
using Armature.Core.BuildActions;
using Armature.Core.BuildActions.Creation;
using Armature.Core.Common;
using Armature.Core.UnitSequenceMatcher;
using Resharper.Annotations;

namespace Armature
{
  public class TreatingTuner<T> : Tuner
  {
    public TreatingTuner([NotNull] IUnitSequenceMatcher unitSequenceMatcher) : base(unitSequenceMatcher) {}

    /// <summary>
    ///   Treat Unit as is w/o any redirections
    /// </summary>
    public Tuner AsIs()
    {
      UnitSequenceMatcher.AddBuildAction(BuildStage.Create, CreateByReflectionBuildAction.Instance);
      return new Tuner(UnitSequenceMatcher);
    }

    /// <summary>
    ///   Pass the <see cref="instance" /> to any consumer of an Unit
    /// </summary>
    public void AsInstance([CanBeNull] T instance) => UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new SingletonBuildAction(instance));

    /// <param name="addDefaultCreateAction">
    ///   If <see cref="AddCreateBuildAction.Yes" /> adds a build action
    ///   <see cref="Default.CreationBuildAction" /> for <see cref="UnitInfo" />(<see name="TRedirect" />, null)
    ///   as a creation build action.
    /// </param>
    public Tuner As<TRedirect>(AddCreateBuildAction addDefaultCreateAction) => As<TRedirect>(null, addDefaultCreateAction);

    /// <param name="token"></param>
    /// <param name="addDefaultCreateAction">
    ///   If <see cref="AddCreateBuildAction.Yes" /> adds a build action
    ///   <see cref="Default.CreationBuildAction" /> for <see cref="UnitInfo" />(<see name="TRedirect" />, <see cref="token" />)
    ///   as a creation build action.
    /// </param>
    /// <typeparam name="TRedirect"></typeparam>
    public Tuner As<TRedirect>(object token = null, AddCreateBuildAction addDefaultCreateAction = AddCreateBuildAction.Yes)
    {
      var from = typeof(T);
      var to = typeof(TRedirect);

      if (!from.IsAssignableFrom(to))
        throw new Exception(string.Format("Object of type {0} can't be used as value of variable of type {1}", from, to))
          .AddData("From", from)
          .AddData("To", to);

      UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new RedirectTypeBuildAction(to, token));

      var childMatcher = UnitSequenceMatcher;
      if (addDefaultCreateAction == AddCreateBuildAction.Yes)
      {
        childMatcher = new StrictUnitSequenceMatcher(Match.Type<TRedirect>(token));

        UnitSequenceMatcher
          .AddOrGetUnitSequenceMatcher(childMatcher)
          .AddBuildAction(BuildStage.Create, Default.CreationBuildAction);
      }

      return new Tuner(childMatcher);
    }

    public CreationTuner<T> Created<TRedirect>(object token = null)
    {
      var redirectTo = typeof(TRedirect);

      //Todo: should this check be moved inside RedirectTypeBuildAction?
      if (!typeof(T).IsAssignableFrom(redirectTo))
        throw new Exception("Not assignable");

      UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new RedirectTypeBuildAction(redirectTo, token));
      return new CreationTuner<T>(UnitSequenceMatcher, token);
    }
    
    public void CreatedBy([NotNull] Func<IBuildSession, T> factoryMethod) =>
      UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T>(factoryMethod));

    public Tuner CreatedBy<T1>([NotNull] Func<IBuildSession, T1, T> factoryMethod) => 
      new Tuner(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T>(factoryMethod)));

    public Tuner CreatedBy<T1, T2>([NotNull] Func<IBuildSession, T1, T2, T> factoryMethod) =>
      new Tuner(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T2, T>(factoryMethod)));

    public Tuner CreatedBy<T1, T2, T3>([NotNull] Func<IBuildSession, T1, T2, T3, T> factoryMethod) => 
      new Tuner(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T2, T3, T>(factoryMethod)));
 }
}