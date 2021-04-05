using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Armature.Core.Logging;

namespace Armature.Core.BuildActions.Constructor
{
  /// <summary>
  ///   "Builds" a constructor Unit of the currently building Unit with biggest number of parameters
  /// </summary>
  public class GetLongestConstructorBuildAction : IBuildAction
  {
    public static readonly IBuildAction Instance = new GetLongestConstructorBuildAction();

    private GetLongestConstructorBuildAction() { }

    public void Process(IBuildSession buildSession)
    {
      var unitType     = buildSession.GetUnitUnderConstruction().GetUnitType();
      var constructors = unitType.GetConstructors();

      if(constructors.Length > 0)
      {
        var ctor = GetConstructor(constructors, unitType);
        buildSession.BuildResult = new BuildResult(ctor);
      }
    }

    [DebuggerStepThrough]
    public void PostProcess(IBuildSession buildSession) { }

    private static ConstructorInfo GetConstructor(ConstructorInfo[] constructors, Type unitType)
    {
      var suitableConstructors = new Dictionary<int, int> {{0, constructors[0].GetParameters().Length}};

      for(var i = 1; i < constructors.Length; i++)
      {
        var parametersCount = constructors[i].GetParameters().Length;

        var maxParametersCount = suitableConstructors.First().Value;

        if(parametersCount == maxParametersCount)
        {
          suitableConstructors.Add(i, parametersCount);
        }
        else if(parametersCount > maxParametersCount)
        {
          suitableConstructors.Clear();
          suitableConstructors.Add(i, parametersCount);
        }
      }

      if(suitableConstructors.Count != 1)
      {
        var message = new StringBuilder($"More than one constructor with max parameters count for type '{unitType.ToLogString()}' found");
        message.AppendLine();

        var counter = 0;

        foreach(var pair in suitableConstructors)
          message.AppendLine($"ctor#{counter++}: {constructors[pair.Key]}");

        var exc = new ArmatureException("ConstructorsAmbiguity");

        counter = 0;

        foreach(var pair in suitableConstructors)
          exc.Data.Add(counter++, constructors[pair.Key]);

        throw exc;
      }

      return constructors[suitableConstructors.First().Key];
    }

    public override string ToString() => GetType().GetShortName();
  }
}
