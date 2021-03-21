using System;
using System.Collections.Generic;
using Armature.Core.Common;

namespace Armature.Core
{
  public partial class BuildSession
  {
    /// <summary>
    /// Armature builds dependencies of the requested unit in a BFS order. To reach that, dependencies are not instantiated immediately
    /// (because in this case it would be DFS) but a lazy tree of dependencies factories is created. In the end of the build session
    /// <see cref="ResolveInTopologicallySortedOrder"/> method is called and only at this moment all objects are created. See <see cref="BuildResult.Resolve"/>
    /// for details.
    /// </summary>
    private class LazyDependenciesTree
    {
      private int _minLevel = int.MaxValue;
      private int _maxLevel = int.MinValue;
      private readonly Dictionary<int, List<BuildResult>> _dictionary = new();

      public void AddTreeNode(int level, BuildResult result)
      {
        _minLevel = Math.Min(_minLevel, level);
        _maxLevel = Math.Max(_maxLevel, level);

        _dictionary
          .GetOrCreateValue(level, () => new List<BuildResult>())
          .Add(result);
      }
      
      public void ResolveInTopologicallySortedOrder()
      {
        for (var level = _maxLevel; level >= _minLevel; level--)
          if (_dictionary.TryGetValue(level, out var list))
            foreach (var result in list)
              result.Resolve();
      }
    }
  }
}