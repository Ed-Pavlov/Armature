using System.Diagnostics.CodeAnalysis;
using Armature.Core;
using Armature.Core.BuildActions.Creation;

namespace Armature
{
  public static class Default
  {
    /// <summary>
    /// This is the default build action used by <see cref="CreationTuner{T}.ByDefault"/>
    /// and <see cref="TreatingTuner{T}.As{TRedirect}(AddCreateBuildAction)"/> if <see cref="AddCreateBuildAction.Yes"/> passed into it.
    /// 
    /// You can set your own build action which will be used by these tuners.
    /// </summary>
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
    public static IBuildAction CreationBuildAction = CreateByReflectionBuildAction.Instance;
  }
}