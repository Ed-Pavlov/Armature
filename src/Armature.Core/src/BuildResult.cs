using System;
using System.Diagnostics;
using Armature.Core.Common;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  /// Represents a result of building of a Unit, null is a valid value of the <see cref="Value" />.
  /// </summary>
  public class BuildResult
  {
    private readonly Func<object?>? _factory;
    private Maybe<object> _value = Maybe<object>.Nothing;
  
    /// <summary>
    /// Armature builds a tree of dependencies of the requested unit in a BFS order. To reach that, dependencies are not instantiated immediately
    /// (because in this case it would be DFS) but a lazy tree of dependencies factories is created. See <see cref="BuildSession.LazyDependenciesTree"/>.
    /// </summary>
    [DebuggerStepThrough]
    public BuildResult(Func<object?> factory) => _factory = factory ?? throw new ArgumentNullException(nameof(factory));

    [DebuggerStepThrough]
    public BuildResult(object? value) => _value = value.ToMaybe();

    public bool IsResolved => _value.HasValue;
    
    public void Resolve()
    {
      if (!_value.HasValue)
        _value = _factory!().ToMaybe();
    }
    
    public object? Value => _value.Value;

    [DebuggerStepThrough]
    public override string ToString() => $"IsResolved: {IsResolved}" + (_value.HasValue ? $", {_value.Value.ToLogString()}" : null);
  }
}