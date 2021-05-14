using System.Diagnostics.CodeAnalysis;
using Armature.Core;

namespace Armature
{
  public static class Default
  {
    /// <summary>
    ///   This is the default build action used by <see cref="CreationTuner.CreatedByDefault" /> and <see cref="TreatingTuner{T}.AsCreated{TRedirect}" />.
    ///   You can set your own build action which will be used by these tuners.
    /// </summary>
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
    public static IBuildAction CreationBuildAction = Static<CreateByReflection>.Instance;
  }
}
