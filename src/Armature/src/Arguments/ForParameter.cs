﻿using System;
using Armature.Core;

namespace Armature
{
  /// <summary>
  /// This class provides methods to tune up how to build arguments for method parameters.
  /// </summary>
  public static class ForParameter
  {
    /// <summary>
    ///   Tunes up how to build an argument to inject into method parameter of type <paramref name="type"/>.
    /// </summary>
    public static MethodArgumentTuner OfType(Type type)
      => new(parentNode =>
               parentNode
                .GetOrAddNode(new SkipSpecialUnits())
                .GetOrAddNode(new IfLastUnit(new IsMethodParameterWithType(type, true), InjectPointMatchingWeight.TypedParameter)));

    /// <summary>
    ///   Tunes up what argument inject into method parameter of type <typeparamref name="T" />.
    /// </summary>
    public static MethodArgumentTuner<T> OfType<T>()
      => new(parentNode =>
               parentNode
                .GetOrAddNode(new SkipSpecialUnits())
                .GetOrAddNode(new IfLastUnit(new IsMethodParameterWithType(typeof(T), true), InjectPointMatchingWeight.TypedParameter)));

    /// <summary>
    ///   Tunes up what argument inject into method parameter with the specified <paramref name="parameterName"/>.
    /// </summary>
    public static MethodArgumentTuner Named(string parameterName)
      => new(parentNode =>
               parentNode
                .GetOrAddNode(new SkipSpecialUnits())
                .GetOrAddNode(new IfLastUnit(new IsMethodParameterNamed(parameterName), InjectPointMatchingWeight.NamedParameter)));

    /// <summary>
    ///   Tunes up what argument inject into method parameter marked with <see cref="InjectAttribute"/> with the specified <paramref name="injectPointId"/>. 
    /// </summary>
    public static MethodArgumentTuner WithInjectPoint(object? injectPointId)
      => new(parentNode =>
               parentNode
                .GetOrAddNode(new SkipSpecialUnits())
                .GetOrAddNode(new IfLastUnit(new IsParameterInfoWithAttribute(injectPointId), InjectPointMatchingWeight.AttributedParameter)));
  }
}
