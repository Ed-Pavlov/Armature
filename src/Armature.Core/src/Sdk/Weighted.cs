﻿using System;
using System.Diagnostics;
using BeatyBit.Armature.Core.Annotations;

namespace BeatyBit.Armature.Core.Sdk;

/// <summary>
/// Couples an entity with a weight.
/// </summary>
public readonly struct Weighted<T> : IComparable<Weighted<T>>
{
  public readonly T    Entity;
  public readonly long Weight;

  [DebuggerStepThrough]
  public Weighted(T entity, long weight)
  {
    Entity = entity;
    Weight = weight;
  }

  [WithoutTest]
  [DebuggerStepThrough]
  public int CompareTo(Weighted<T> other) => Weight.CompareTo(other.Weight);

  [DebuggerStepThrough]
  public override string ToString() => string.Format("{0}, Weight={1}", Entity.ToHoconString(), Weight.ToHoconString());
}

public static class WeightedExtension
{
  /// <summary>
  /// Couples an entity with a weight. The method is introduced for the sake of readability only.
  /// </summary>
  [DebuggerStepThrough]
  public static Weighted<T> WithWeight<T>(this T entity, long weight) => new(entity, weight);
}