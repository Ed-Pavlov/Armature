using System;
using Armature.Core;

namespace Tests.Extensibility.MaybePropagation.Extension
{
  internal class BuildMaybeAction<T> : IBuildAction
  {
    private readonly Guid _uniqueToken;
      
    public BuildMaybeAction(Guid uniqueToken) { _uniqueToken = uniqueToken; }

    public void Process(UnitBuilder unitBuilder)
    {
      try
      {
        var result = unitBuilder.Build(new UnitInfo(typeof(T), _uniqueToken));
        if(result == null) throw new InvalidOperationException();
            
        unitBuilder.BuildResult = new BuildResult(((T)result.Value).ToMaybe());
      }
      catch (MaybePropagationException )
      {
        unitBuilder.BuildResult = new BuildResult(Maybe<T>.Nothing);            
      }
    }

    public void PostProcess(UnitBuilder unitBuilder) { }
  }
}