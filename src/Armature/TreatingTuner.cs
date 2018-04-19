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
    /// For all who depends on <typeparamref name="T"/> inject object of type <typeparamref name="T"/>.
    /// Tune plan of building it by subsequence calls.  
    /// </summary>
    public Tuner AsIs()
    {
      UnitSequenceMatcher.AddBuildAction(BuildStage.Create, CreateByReflectionBuildAction.Instance);
      return new Tuner(UnitSequenceMatcher);
    }

    /// <summary>
    ///   For all who depends on <typeparamref name="T"/> inject <paramref name="instance"/>.
    /// </summary>
    public void AsInstance([CanBeNull] T instance) => UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new SingletonBuildAction(instance));

    /// <summary>
    /// For all who depends on <typeparamref name="T"/> inject object of type <typeparamref name="TRedirect"/>.
    /// Tune plan of building it by subsequence calls.  
    /// </summary>
    /// <param name="addDefaultCreateAction">
    ///   If <see cref="AddCreateBuildAction.Yes" /> adds a build action <see cref="Default.CreationBuildAction" /> for
    ///   <see cref="UnitInfo" />(<see name="TRedirect" />, null) as a creation build action.
    /// </param>
    public Tuner As<TRedirect>(AddCreateBuildAction addDefaultCreateAction) => As<TRedirect>(null, addDefaultCreateAction);

    ///<inheritdoc cref="As{TRedirect}(Armature.AddCreateBuildAction)"/>
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

    /// <summary>
    /// For all who depends on <typeparamref name="T"/> inject object of type <typeparamref name="TRedirect"/>.
    /// Tune plan of building it by subsequence calls.  
    /// </summary>
    public CreationTuner<T> Created<TRedirect>(object token = null)
    {
      var redirectTo = typeof(TRedirect);

      //Todo: should this check be moved inside RedirectTypeBuildAction?
      if (!typeof(T).IsAssignableFrom(redirectTo))
        throw new Exception("Not assignable");

      UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new RedirectTypeBuildAction(redirectTo, token));
      return new CreationTuner<T>(UnitSequenceMatcher, token);
    }
    
    /// <summary>
    /// For all who depends on <typeparamref name="T"/> inject object created by specified factory method.
    /// </summary>
    public void CreatedBy([NotNull] Func<IBuildSession, T> factoryMethod) =>
      UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T>(factoryMethod));

    /// <inheritdoc cref="CreatedBy"/>
    public Tuner CreatedBy<T1>([NotNull] Func<IBuildSession, T1, T> factoryMethod) => 
      new Tuner(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T>(factoryMethod)));

    /// <inheritdoc cref="CreatedBy"/>
    public Tuner CreatedBy<T1, T2>([NotNull] Func<IBuildSession, T1, T2, T> factoryMethod) =>
      new Tuner(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T2, T>(factoryMethod)));

    /// <inheritdoc cref="CreatedBy"/>
    public Tuner CreatedBy<T1, T2, T3>([NotNull] Func<IBuildSession, T1, T2, T3, T> factoryMethod) => 
      new Tuner(UnitSequenceMatcher.AddBuildAction(BuildStage.Create, new CreateByFactoryMethodBuildAction<T1, T2, T3, T>(factoryMethod)));
 }
}