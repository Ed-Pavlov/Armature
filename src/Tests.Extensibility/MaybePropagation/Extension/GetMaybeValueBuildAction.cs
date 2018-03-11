using Armature.Core;

namespace Tests.Extensibility.MaybePropagation.Extension
{
  /// <summary>
  /// Uses <see cref="Maybe{T}.Value"/> as a build unit
  /// </summary>
  internal class GetMaybeValueBuildAction<T> : IBuildAction
  {
    public void Process(UnitBuilder unitBuilder)
    {}

    public void PostProcess(UnitBuilder unitBuilder)
    {
      var result = unitBuilder.BuildResult;
      if(result?.Value == null)
        throw new ArmatureException(string.Format("Can't build value of {0}", typeof(Maybe<T>)));

      var maybe = (Maybe<T>)result.Value;
      if (maybe.HasValue)
        unitBuilder.BuildResult = new BuildResult(maybe.Value);
      else
        throw new MaybePropagationException();
    }
  }
}