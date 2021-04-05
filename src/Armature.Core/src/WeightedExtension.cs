using System.Diagnostics;

namespace Armature.Core
{
  public static class WeightedExtension
  {
    [DebuggerStepThrough]
    public static Weighted<T> WithWeight<T>(this T entity, int weight) => new(entity, weight);
  }
}
