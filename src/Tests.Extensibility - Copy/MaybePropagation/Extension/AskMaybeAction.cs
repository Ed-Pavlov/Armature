using System;
using Armature.Core;

namespace Tests.Extensibility.MaybePropagation.Extension
{
  public class AskMaybeAction<T> : IBuildAction
  {
    private readonly object _token;

    public AskMaybeAction(object token) { _token = token; }

    public void Process(UnitBuilder unitBuilder)
    {
      var result = unitBuilder.Build(new UnitInfo(typeof(Maybe<T>), _token));
      if(result == null || result.Value == null)
        throw new ArmatureException(String.Format("Can't build value of {0}", typeof(Maybe<T>)));

      var maybe = (Maybe<T>)result.Value;
      if (maybe.HasValue)
        unitBuilder.BuildResult = new BuildResult(maybe.Value);
      else
        throw new MaybePropagationException();
    }

    public void PostProcess(UnitBuilder unitBuilder) { }
  }
}