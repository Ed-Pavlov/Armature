using System;
using Armature.Core;

namespace Armature.Extensibility
{
  public class BuildActionExtensibility : UnitMatcherExtensibility, IBuildActionExtensibility
  {
    protected readonly IBuildAction BuildAction;

    public BuildActionExtensibility(IUnitMatcher unitMatcher, IBuildAction getPropertyAction, int weight) : base(unitMatcher, weight) => 
      BuildAction = getPropertyAction ?? throw new ArgumentNullException(nameof(getPropertyAction));

    IBuildAction IBuildActionExtensibility.BuildAction => BuildAction;
  }
}