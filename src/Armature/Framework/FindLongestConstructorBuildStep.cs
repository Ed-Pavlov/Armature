using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Armature.Framework
{
  public class FindLongestConstructorBuildStep : FindConstructorBuildStepBase
  {
    private static readonly ConstructorInfo NoParametersConstructor = typeof (DefaultConstructor).GetConstructors()[0];

    public FindLongestConstructorBuildStep(int weight = 0) : base(weight)
    {}

    protected override ConstructorInfo GetConstructor(Type type)
    {
      var constructors = type.GetConstructors();
      if (constructors.Length == 0)
        if (type.IsValueType)
          return NoParametersConstructor;
        else
          throw new Exception("DoesNotContainConstructor");

      var suitableConstructors = new Dictionary<int, int> {{0, constructors[0].GetParameters().Length}};
      for (var i = 1; i < constructors.Length; i++)
      {
        var paramentersCount = constructors[i].GetParameters().Length;

        var maxParametersCount = suitableConstructors.First().Value;
        if (paramentersCount == maxParametersCount)
          suitableConstructors.Add(i, paramentersCount);
        else if (paramentersCount > maxParametersCount)
        {
          suitableConstructors.Clear();
          suitableConstructors.Add(i, paramentersCount);
        }
      }

      if (suitableConstructors.Count != 1)
      {
        var exc = new Exception("ConstructorsAmbiguty");
        var counter = 0;
        foreach (var pair in suitableConstructors)
          exc.Data.Add("Constructor" + counter++, constructors[pair.Key]);
        throw exc;
      }

      return constructors[suitableConstructors.First().Key];
    }

    private class DefaultConstructor{}
  }
}