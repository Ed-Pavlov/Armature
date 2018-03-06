﻿namespace Armature.Core
{
  public static class WeightedExtension
  {
    public static Weighted<T> WithWeight<T>(this T entity, int weight)
    {
      return new Weighted<T>(entity, weight);
    }
  }
}