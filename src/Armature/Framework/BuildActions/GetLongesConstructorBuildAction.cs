using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Armature.Core;
using Armature.Logging;

namespace Armature.Framework.BuildActions
{
  public class GetLongesConstructorBuildAction : IBuildAction
  {
    public void Process(UnitBuilder unitBuilder)
    {
      var unitType = unitBuilder.GetUnitUnderConstruction().GetUnitType();
      var constructor = GetConstructor(unitType.GetConstructors());

      constructor.LogConstructor(this);

      unitBuilder.BuildResult = new BuildResult(constructor);
    }

    [DebuggerStepThrough]
    public void PostProcess(UnitBuilder unitBuilder) { }

    private static ConstructorInfo GetConstructor(ConstructorInfo[] constructors)
    {
      var suitableConstructors = new Dictionary<int, int> {{0, constructors[0].GetParameters().Length}};
      for (var i = 1; i < constructors.Length; i++)
      {
        var paramentersCount = constructors[i].GetParameters().Length;

        var maxParametersCount = suitableConstructors.First().Value;
        if (paramentersCount == maxParametersCount)
        {
          suitableConstructors.Add(i, paramentersCount);
        }
        else if (paramentersCount > maxParametersCount)
        {
          suitableConstructors.Clear();
          suitableConstructors.Add(i, paramentersCount);
        }
      }

      if (suitableConstructors.Count != 1)
      {
        var exc = new ArmatureException("ConstructorsAmbiguty");
        var counter = 0;
        foreach (var pair in suitableConstructors)
          exc.Data.Add("Constructor" + counter++, constructors[pair.Key]);
        throw exc;
      }

      return constructors[suitableConstructors.First().Key];
    }
  }
}