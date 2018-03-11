using System.Diagnostics.CodeAnalysis;
using Armature.Core;
using Armature.Framework;
using Armature.Framework.BuildActions;

namespace Armature
{
  public static class Default
  {
    /// <summary>
    ///   This is the default build step used by Treat{...}.As{...} method.
    ///   Set a build step which be used by default.
    /// </summary>
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
    public static IBuildAction CreationBuildAction = CreateByReflectionBuildAction.Instance;
  }
}