using System;
using Armature.Core;

namespace Armature
{
  public static class Constructor
  {
    /// <summary>
    ///   Instantiate a Unit using a constructor with the biggest number of parameters
    /// </summary>
    public static IInjectPointTuner WithMaxParametersCount(short weight = 0)
      => new InjectPointTuner(
        node => node.GetOrAddNode(new IfFirstUnit(Static<IsConstructor>.Instance, weight))
                    .UseBuildAction(Static<GetConstructorWithMaxParametersCount>.Instance, BuildStage.Create));

    /// <summary>
    ///   Instantiate a Unit using a constructor marked with <see cref="InjectAttribute" />(<paramref name="injectionPointId" />)
    /// </summary>
    public static IInjectPointTuner MarkedWithInjectAttribute(object? injectionPointId, short weight = 0)
      => new InjectPointTuner(
        node => node.GetOrAddNode(new IfFirstUnit(Static<IsConstructor>.Instance, weight))
                    .UseBuildAction(new GetConstructorByInjectPointId(injectionPointId), BuildStage.Create));

    /// <summary>
    ///   Instantiate a Unit using constructor without parameters
    /// </summary>
    public static IInjectPointTuner Parameterless(short weight = 0) => WithParameters(weight);

    /// <summary>
    ///   Instantiate a Unit using constructor with exact set of parameters provided as generic arguments
    /// </summary>
    public static IInjectPointTuner WithParameters<T1>(short weight = 0) => WithParameters(weight, typeof(T1));

    /// <summary>
    ///   Instantiate a Unit using constructor with exact set of parameters provided as generic arguments
    /// </summary>
    public static IInjectPointTuner WithParameters<T1, T2>(short weight = 0) => WithParameters(weight, typeof(T1), typeof(T2));

    /// <summary>
    ///   Instantiate a Unit using constructor with exact set of parameters provided as generic arguments
    /// </summary>
    public static IInjectPointTuner WithParameters<T1, T2, T3>(short weight = 0) => WithParameters(weight, typeof(T1), typeof(T2), typeof(T3));

    /// <summary>
    ///   Instantiate a Unit using constructor with exact set of parameters provided as generic arguments
    /// </summary>
    public static IInjectPointTuner WithParameters<T1, T2, T3, T4>(short weight = 0)
      => WithParameters(weight, typeof(T1), typeof(T2), typeof(T3), typeof(T4));

    /// <summary>
    ///   Instantiate a Unit using constructor with exact set of parameters specified in <paramref name="parameterTypes" />
    /// </summary>
    public static IInjectPointTuner WithParameters(params Type[] parameterTypes) => WithParameters(0, parameterTypes);

    /// <summary>
    ///   Instantiate a Unit using constructor with exact set of parameters specified in <paramref name="parameterTypes" />
    /// </summary>
    public static IInjectPointTuner WithParameters(short weight, params Type[] parameterTypes)
      => new InjectPointTuner(
        node => node.GetOrAddNode(new IfFirstUnit(Static<IsConstructor>.Instance, weight))
                    .UseBuildAction(new GetConstructorByParameterTypes(parameterTypes), BuildStage.Create));
  }
}
