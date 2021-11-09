using System;
using Armature.Core;

namespace Armature.Extensibility;

public class BuildActionExtensibility : UnitMatcherExtensibility, IBuildActionExtensibility
{
  protected readonly IBuildAction BuildAction;

  public BuildActionExtensibility(IUnitPattern unitPattern, IBuildAction getPropertyAction, int weight) : base(unitPattern, weight)
    => BuildAction = getPropertyAction ?? throw new ArgumentNullException(nameof(getPropertyAction));

  IBuildAction IBuildActionExtensibility.BuildAction => BuildAction;
}