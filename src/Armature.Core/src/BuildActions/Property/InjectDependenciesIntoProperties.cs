using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Armature.Core.Annotations;
using Armature.Core.Sdk;

namespace Armature.Core;

/// <summary>
/// Injects values into building Unit properties specified for injection
/// </summary>
public record InjectDependenciesIntoProperties : IBuildAction
{
  [WithoutTest]
  [DebuggerStepThrough]
  public void Process(IBuildSession buildSession) { }

  public void PostProcess(IBuildSession buildSession)
  {
    if(!buildSession.BuildResult.HasValue)
    {
      Log.WriteLine(LogLevel.Info, "Warning: unit is not built");
      return;
    }

    var unit = buildSession.BuildResult.Value;
    if(unit is null)
    {
      Log.WriteLine(LogLevel.Info, "Warning: built unit is null, can't inject into properties");
      return;
    }

    var type = buildSession.BuildChain.TargetUnit.GetUnitTypeSafe() ?? unit.GetType();

    var unitInfo = new UnitId(type, SpecialTag.PropertyCollection);

    var properties = buildSession.BuildAllUnits(unitInfo)
                               .OrderByDescending(_ => _.Weight)
                               .Select(_ => _.Entity)
                               .Where(buildResult => buildResult.HasValue)
                               .Select(buildResult => buildResult.Value!)
                               .SelectMany(list => (PropertyInfo[]) list)
                               .ToArray();

    if(properties.Length == 0)
      Log.WriteLine(LogLevel.Trace, "PropertyList: null");
    else
    {
      Log.WriteLine(LogLevel.Verbose, () => $"PropertyList: {properties.ToHoconString()}");

      foreach(var property in properties)
      {
        var argument = buildSession.BuildPropertyArgument(property);
        property.SetValue(unit, argument);
      }
    }
  }

  [DebuggerStepThrough]
  public override string ToString() => nameof(InjectDependenciesIntoProperties);
}