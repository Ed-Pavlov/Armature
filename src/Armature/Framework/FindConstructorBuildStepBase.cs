using System;
using System.Reflection;
using Armature.Core;
using Armature.Logging;

namespace Armature.Framework
{
  /// <summary>
  /// Base class for build steps which "build" <see cref="ConstructorInfo"/> for a <see cref="UnitInfo.Id"/> as Type
  /// </summary>
  public abstract class FindConstructorBuildStepBase : LeafBuildStep
  {
    protected FindConstructorBuildStepBase(int matchingWeight) : base(matchingWeight)
    {}

    protected override StagedBuildAction GetBuildAction(UnitInfo unitInfo)
    {
      if (!Equals(unitInfo.Token, SpecialToken.FindConstructor))
      {
        Log.Trace("does not match unit");
        return null;
      }

      var constructorInfo = GetConstructor(unitInfo.GetUnitType());
      Log.Verbose("Constructor {0}", constructorInfo == null ? "is not found" : string.Format("{0} found", constructorInfo));
      
      return constructorInfo == null 
        ? null 
        : new StagedBuildAction(BuildStage.Cache, new SingletonBuildAction(constructorInfo));
    }

    protected abstract ConstructorInfo GetConstructor(Type type);
  }
}