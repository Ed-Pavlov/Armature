using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Armature.Core;

namespace Armature.Framework
{
  /// <summary>
  /// Build steps "builds" <see cref="ConstructorInfo"/> for a <see cref="UnitInfo.Id"/> as Type, returns <see cref="ConstructorInfo"/>
  /// for a public constructor with the maximum number of parameters  
  /// </summary>
  public class FindLongestConstructorBuildStep : FindConstructorBuildStepBase
  {
    private static readonly ConstructorInfo NoParametersConstructor = typeof (DefaultConstructor).GetConstructors()[0];

    public FindLongestConstructorBuildStep(int matchingWeight) : base(matchingWeight)
    {}

    protected override ConstructorInfo GetConstructor(Type type)
    {
      var constructors = type.GetConstructors();
      if (constructors.Length == 0)
        if (type.IsValueType)
          return NoParametersConstructor;
        else
          throw new ArmatureException("DoesNotContainConstructor");

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
        var exc = new ArmatureException("ConstructorsAmbiguty");
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