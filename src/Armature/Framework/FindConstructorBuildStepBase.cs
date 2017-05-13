using System;
using System.Reflection;
using Armature.Core;

namespace Armature.Framework
{
  public abstract class FindConstructorBuildStepBase : LeafBuildStep
  {
    protected FindConstructorBuildStepBase(int weight) : base(weight)
    {}


    protected override StagedBuildAction GetBuildAction(UnitInfo unitInfo)
    {
      if (!Equals(unitInfo.Token, SpecialToken.FindConstructor))
      {
//        this.LogDoesNotMatch(buildSequence);
        return null;
      }

      var constructorInfo = GetConstructor(unitInfo.GetUnitType());
      if (constructorInfo == null)
      {
//        this.LogInfo("constructor is not found");
        return null;
      }

//      this.LogBuildStepMatch(buildSequence);
      return new StagedBuildAction(BuildStage.Create, new SingletonBuildAction(constructorInfo));
    }

    protected abstract ConstructorInfo GetConstructor(Type type);
  }
}