using System;
using Armature.Core;
using Armature.Core.Sdk;

namespace Tests.Extensibility.MaybePropagation.Implementation
{
  /// <summary>
  /// Builds <typeparamref name="T" /> and wraps it into <see cref="Maybe{T}" />
  /// </summary>
  internal class BuildMaybeAction<T> : IBuildAction
  {
    private readonly Guid _uniqueTag;

    public BuildMaybeAction(Guid uniqueTag) => _uniqueTag = uniqueTag;

    public void Process(IBuildSession buildSession)
    {
      try
      {
        var result = buildSession.BuildUnit(Unit.Of(typeof(T), _uniqueTag));

        if(!result.HasValue) throw new InvalidOperationException();

        buildSession.BuildResult = new BuildResult(((T) result.Value).ToMaybe());
      }
      catch(MaybeIsNothingException)
      {
        buildSession.BuildResult = new BuildResult(Maybe<T>.Nothing);
      }
    }

    public void PostProcess(IBuildSession buildSession) { }

    public override string ToString() => GetType().GetShortName();
  }
}