using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core.UnitMatchers
{
  /// <summary>
  ///   Base class for matchers matching an "inject point" marked with <see cref="InjectAttribute" /> with specified <see cref="InjectAttribute.InjectionPointId" />
  /// </summary>
  public abstract record UnitByInjectPointIdMatcherBase : UnitByAttributeMatcherBase<InjectAttribute>
  {
    private readonly object? _injectPointId;

    [DebuggerStepThrough]
    protected UnitByInjectPointIdMatcherBase(object? injectPointId = null) : base(attribute => Equals(attribute.InjectionPointId, injectPointId))
      => _injectPointId = injectPointId;

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _injectPointId.ToLogString());
  }
}
