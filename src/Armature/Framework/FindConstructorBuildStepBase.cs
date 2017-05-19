using System;
using System.Reflection;
using Armature.Core;
using Armature.Logging;

namespace Armature.Framework
{
  public abstract class FindConstructorBuildStepBase : LeafBuildStep
  {
    protected FindConstructorBuildStepBase(int weight) : base(weight)
    {}

    protected override StagedBuildAction GetBuildAction(UnitInfo unitInfo)
    {
      if (!Equals(unitInfo.Token, SpecialToken.FindConstructor))
        return null;

      var constructorInfo = GetConstructor(unitInfo.GetUnitType());
      Log.Verbose("{0}: {1}", GetType().Name, constructorInfo == null ? "is not found" : constructorInfo.ToString());
      
      return constructorInfo == null 
        ? null 
        : new StagedBuildAction(BuildStage.Create, new SingletonBuildAction(constructorInfo));
    }

    protected abstract ConstructorInfo GetConstructor(Type type);
  }
}