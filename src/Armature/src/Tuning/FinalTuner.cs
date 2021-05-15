using System;
using System.Diagnostics;
using Armature.Core;
using Armature.Extensibility;

namespace Armature
{
  public class FinalTuner : UnitSequenceExtensibility
  {
    [DebuggerStepThrough]
    public FinalTuner(IPatternTreeNode parentNode) : base(parentNode) { }

    /// <summary>
    ///   Provides arguments to inject into building unit. See <see cref="ForParameter" /> for details.
    /// </summary>
    public FinalTuner UsingArguments(params object[] arguments)
    {
      if(arguments is null || arguments.Length == 0) throw new ArgumentNullException(nameof(arguments));

      foreach(var argument in arguments)
        if(argument is IArgumentTuner buildPlan)
          buildPlan.Tune(ParentNode);
        else if(argument is ITuner)
          throw new ArgumentException("IParameterValueBuildPlan or plain object value expected");
        else
          ParentNode
           .GetOrAddNode(new SkipWhileUnit(Static<IsServiceUnit>.Instance))
           .GetOrAddNode(new IfLastUnit(new IsBaseTypeOf(argument.GetType(), null), InjectPointMatchingWeight.WeakTypedParameter))
           .UseBuildAction(new Instance<object>(argument), BuildStage.Cache);

      return this;
    }

    public FinalTuner InjectInto(params IInjectPointTuner[] propertyIds)
    {
      foreach(var injectPointTuner in propertyIds)
        injectPointTuner.Tune(ParentNode);
      return this;
    }

    /// <summary>
    ///   Register Unit as an singleton with a lifetime equal to parent <see cref="PatternTree"/>. See <see cref="Singleton" /> for details
    /// </summary>
    public void AsSingleton() => ParentNode.UseBuildAction(new Singleton(), BuildStage.Cache);

    /// <summary>
    ///   Doing the same as <see cref="PatternTreeTunerExtension.Building{T}" /> but w/o breaking fluent syntax
    /// </summary>
    public void BuildingWhich(Action<RootTuner> tuneAction)
    {
      if(tuneAction is null) throw new ArgumentNullException(nameof(tuneAction));

      tuneAction(new RootTuner(ParentNode));
    }
  }
}
