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
    var unit = buildSession.BuildResult.Value;
    var type = buildSession.BuildChain.TargetUnit.GetUnitTypeSafe();

    var unitInfo = new UnitId(type, SpecialTag.PropertyList);
    var unitList = buildSession.BuildAllUnits(unitInfo).OrderByDescending(_ => _.Weight).Select(_ => _.Entity);

    foreach(var property in unitList.Select(result => result.Value!).SelectMany(list => (PropertyInfo[]) list))
    {
      var argument = buildSession.BuildPropertyArgument(property);
      property.SetValue(unit, argument);
    }
  }

  [DebuggerStepThrough]
  public override string ToString() => nameof(InjectDependenciesIntoProperties);
}
