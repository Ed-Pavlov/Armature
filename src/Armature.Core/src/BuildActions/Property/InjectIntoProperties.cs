using System.Diagnostics;
using System.Reflection;

namespace Armature.Core
{
  /// <summary>
  ///   Injects values into building Unit properties specified for injection
  /// </summary>
  public record InjectIntoProperties : IBuildAction
  {
    public static readonly IBuildAction Instance = new InjectIntoProperties();

    private InjectIntoProperties() { }

    [DebuggerStepThrough]
    public void Process(IBuildSession buildSession) { }

    public void PostProcess(IBuildSession buildSession)
    {
      if(buildSession.BuildResult.HasValue)
      {
        var unit = buildSession.BuildResult.Value;
        var type = unit!.GetType();

        var unitInfo = new UnitId(type, SpecialKey.Property);
        var list     = buildSession.BuildAllUnits(unitInfo);

        foreach(var buildResult in list)
        {
          var property = (PropertyInfo) buildResult.Value!;

          var argument = buildSession.BuildArgument(property);
          property.SetValue(unit, argument);
        }
      }
    }
  }
}
