using System;
using System.Collections.Generic;
using System.Diagnostics;
using Resharper.Annotations;

namespace Armature.Core.Common
{
  internal static class DictionaryExtension
  {
    [DebuggerStepThrough]
    [CanBeNull]
    public static TValue GetValueSafe<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue)) =>
      dictionary.TryGetValue(key, out var value) ? value : defaultValue;
    
    [DebuggerStepThrough]
    public static TValue GetOrCreateValue<TKey, TValue>([NotNull] this Dictionary<TKey, TValue> dictionary, [NotNull] TKey key, [NotNull] Func<TValue> createValue)
    {
      if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));
      if (key == null) throw new ArgumentNullException(nameof(key));
      if (createValue == null) throw new ArgumentNullException(nameof(createValue));

      if (!dictionary.TryGetValue(key, out var value))
      {
        value = createValue();
        dictionary.Add(key, value);
      }

      return value;
    }
  }
}