using System.Linq;
using System.Reflection;
using Armature.Core;
using FakeItEasy.Configuration;
using Tests.Common;

namespace Tests.UnitTests;

public static class WhenBuildArgumentsOfTypeExtension
{
  public static IReturnValueConfiguration<BuildResult> WhenBuildArgumentsOfType<T1>(
      this IReturnValueArgumentValidationConfiguration<BuildResult> fake)
    => fake.WhenBuildArgumentsOfType<T1, Unit, Unit, Unit, Unit, Unit, Unit>();

  public static IReturnValueConfiguration<BuildResult> WhenBuildArgumentsOfType<T1, T2>(
      this IReturnValueArgumentValidationConfiguration<BuildResult> fake)
    => fake.WhenBuildArgumentsOfType<T1, T2, Unit, Unit, Unit, Unit, Unit>();

  public static IReturnValueConfiguration<BuildResult> WhenBuildArgumentsOfType<T1, T2, T3>(
      this IReturnValueArgumentValidationConfiguration<BuildResult> fake)
    => fake.WhenBuildArgumentsOfType<T1, T2, T3, Unit, Unit, Unit, Unit>();

  public static IReturnValueConfiguration<BuildResult> WhenBuildArgumentsOfType<T1, T2, T3, T4>(
      this IReturnValueArgumentValidationConfiguration<BuildResult> fake)
    => fake.WhenBuildArgumentsOfType<T1, T2, T3, T4, Unit, Unit, Unit>();

  public static IReturnValueConfiguration<BuildResult> WhenBuildArgumentsOfType<T1, T2, T3, T4, T5>(
      this IReturnValueArgumentValidationConfiguration<BuildResult> fake)
    => fake.WhenBuildArgumentsOfType<T1, T2, T3, T4, T5, Unit, Unit>();

  public static IReturnValueConfiguration<BuildResult> WhenBuildArgumentsOfType<T1, T2, T3, T4, T5, T6>(
      this IReturnValueArgumentValidationConfiguration<BuildResult> fake)
    => fake.WhenBuildArgumentsOfType<T1, T2, T3, T4, T5, T6, Unit>();

  public static IReturnValueConfiguration<BuildResult> WhenBuildArgumentsOfType<T1, T2, T3, T4, T5, T6, T7>(
      this IReturnValueArgumentValidationConfiguration<BuildResult> fake) =>
      fake.WhenArgumentsMatch(
        arguments =>
        {
          var types      = new[] {typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7)}.Where(_ => _ != typeof(Unit)).ToArray();
          var unitId     = arguments.Get<UnitId>(0);
          var parameters = unitId.Kind as ParameterInfo[];
          if(unitId.Key != SpecialKey.Argument || parameters.Length != types.Length)
            return false;

          for(var i = 0; i < parameters.Length; i++)
            if(parameters?[i].ParameterType != types[i])
              return false;

          return true;
        });

  public static IReturnValueConfiguration<BuildResult> WhenBuildAnyArguments(this IReturnValueArgumentValidationConfiguration<BuildResult> fake) =>
      fake.WhenArgumentsMatch(
        arguments =>
        {
          var unitId = arguments.Get<UnitId>(0);
          return unitId.Kind is ParameterInfo[];
        });
}