using System.Diagnostics;
using Armature.Core.Logging;

namespace Armature.Core
{
  /// <summary>
  /// Base class for matchers matching if a building unit is an argument for an "inject point" marked with with <see cref="InjectAttribute" />
  /// with specified <see cref="InjectAttribute.InjectionPointId" />
  /// </summary>
  public abstract record IsInjectPointAttributeMatcher : IsInjectPointByAttributeMatcher<InjectAttribute>
  {
    private readonly object? _injectPointId;

    /// <param name="injectPointId">An optional id of the inject point. <see cref="InjectAttribute"/> for details.</param>
    [DebuggerStepThrough]
    protected IsInjectPointAttributeMatcher(object? injectPointId = null) : base(attribute => Equals(attribute.InjectionPointId, injectPointId))
      => _injectPointId = injectPointId;

    [DebuggerStepThrough]
    public override string ToString() => string.Format(LogConst.OneParameterFormat, GetType().GetShortName(), _injectPointId.ToLogString());
  }
}
