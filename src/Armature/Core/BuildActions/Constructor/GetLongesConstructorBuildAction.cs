using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Armature.Logging;

namespace Armature.Core.BuildActions.Constructor
{
  public class GetLongesConstructorBuildAction : IBuildAction
  {
    public static readonly IBuildAction Instance = new GetLongesConstructorBuildAction();

    private GetLongesConstructorBuildAction()
    {
    }

    public void Process(IBuildSession buildSession)
    {
      var unitType = buildSession.GetUnitUnderConstruction().GetUnitType();
      var ctor = GetConstructor(unitType.GetConstructors());
      buildSession.BuildResult = new BuildResult(ctor);
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

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
    
    public override string ToString() => GetType().GetShortName();
  }
}