using System.Diagnostics.CodeAnalysis;
using Armature.Core;
using Armature.Core.BuildActions.Creation;

namespace Armature
{
  public static class Default
  {
    /// <summary>
    ///   This is the default build action used by Treat{...}.As{...} method.
    ///   You can set your own build action which will be used by default.
    /// </summary>
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
    public static IBuildAction CreationBuildAction = CreateByReflectionBuildAction.Instance;
  }
}