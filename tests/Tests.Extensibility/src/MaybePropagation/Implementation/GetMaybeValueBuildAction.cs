using Armature.Core;
using Armature.Core.Logging;

namespace Tests.Extensibility.MaybePropagation.Implementation
{
  /// <summary>
  ///   Uses <see cref="Maybe{T}.Value" /> as a build unit
  /// </summary>
  public class GetMaybeValueBuildAction<T> : IBuildAction
  {
    public void Process(IBuildSession buildSession)
    {
    }

    public void PostProcess(IBuildSession buildSession)
    {
      var result = buildSession.BuildResult;
      if (!result.HasValue)
        throw new ArmatureException(string.Format("Can't build value of {0}", typeof(Maybe<T>)));

      var maybe = (Maybe<T>) result.Value;
      if (maybe.HasValue)
        buildSession.BuildResult = new BuildResult(maybe.Value);
      else
        throw new MaybeIsNothingException();
    }

    public override string ToString() => GetType().GetShortName();
  }
}