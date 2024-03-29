﻿using System.Diagnostics.CodeAnalysis;
using Armature.Core.Sdk;
using JetBrains.Annotations;

namespace Armature.Core;

/// <summary>
/// Inherit this class to extend enum pattern with custom weights if you extend Armature with your own build chain, unit or injection point
/// patterns which require to re-balance the weighting system.
/// </summary>
/// <remarks>For common usage of Armature it's not needed.</remarks>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class WeightOf
{
  [PublicAPI]
  public class InjectionPoint
  {
    protected const byte Step = 10;

    /// <summary>
    /// Weight of argument matched by assignability to a parameter/property.
    /// </summary>
    public static byte ByTypeAssignability { get; protected set; } = Step;

    /// <summary>
    /// Weight of injection point (method parameter or property) matched by strict equality of a parameter/property type.
    /// </summary>
    public static byte ByExactType { get; protected set; } = (byte) (ByTypeAssignability + Step);

    /// <summary>
    /// Weight of argument matched by an attribute used to mark a parameter/property.
    /// </summary>
    public static byte ByInjectPointId { get; protected set; } = (byte) (ByExactType + Step);

    /// <summary>
    /// Weight of argument matched by a parameter name.
    /// </summary>
    public static byte ByName { get; protected set; } = (byte) (ByInjectPointId + Step);
  }

  /// <summary>
  /// Weights of <see cref="IUnitPattern"/> is about two orders of magnitude higher than weights of <see cref="InjectionPoint"/> in order in registrations like
  ///
  /// target.TreatInheritorsOf&lt;BaseType&gt;UsingArguments(ForParameter.Named("param").UseValue("baseArg"));
  /// target.Treat&lt;ChildType&gt;().UsingArguments("childArg");
  ///
  /// ForParameter.Named never helps to TreatInheritorsOf "win" Treat.
  /// </summary>
  [PublicAPI]
  public class UnitPattern
  {
    protected static short Step { get; set; } = 10_000;

    /// <summary>
    /// Weight of type matched by open generic type, <see cref="IsGenericOfDefinition"/> unit pattern and <see cref="RedirectOpenGenericType"/> for details
    /// </summary>
    public static short OpenGenericPattern { get; protected set; } = Step;

    /// <summary>
    /// Weight of type matched by base type, <see cref="IsInheritorOf"/> unit pattern
    /// </summary>
    public static short SubtypePattern { get; protected set; } = (short) (OpenGenericPattern + Step);

    /// <summary>
    /// Weight of type matched by exact type
    /// </summary>
    public static short ExactTypePattern { get; protected set; } = (short) (SubtypePattern + Step);
  }

  [PublicAPI]
  public class BuildChainPattern
  {
    public static int SkipWhileUnit { get; protected set; } = 0;

    public static int SkipTillUnit { get; protected set; } = 0;

    /// <summary>
    /// By default the weight of <see cref="IfFirstUnit"/> build chain pattern's weight is increased in order to registrations
    /// like
    /// builder.GetOrAddNode(new SkipTillUnit(new Pattern(typeof(MyType))))
    ///      .GetOrAddNode(new IfFirstUnit(new IsAssignableFromType(typeof(string))))
    /// // ....
    /// "win" registrations like
    ///
    /// builder.GetOrAddNode(new SkipTillUnit(new Pattern(typeof(MyType))))
    ///      .GetOrAddNode(new SkipTillUnit(new IsAssignableFromType(typeof(string))))
    /// // ....
    ///
    /// Because the first one is a "personal" registration whereas the second one will be applied to all units building
    /// in the context of "MyType".
    ///
    /// Note that provided sample is "synthetic" see <see cref="SpecialTag"/> and <see cref="Core.SkipWhileUnit"/> for details.
    /// </summary>
    public static int IfFirstUnit { get; protected set; } = 10;

    /// <summary>
    /// <see cref="Core.SkipAllUnits"/> build chain pattern is used to set "default" rules for any building unit if it doesn't
    /// have a specific registration.
    /// That is why by default its weight is set very low, but not to the minimal possible value to leave a gap for user's needs.
    ///
    /// It's about two orders of magnitude higher than weights of <see cref="IUnitPattern"/> in order to registration of "any" length
    /// does not compensate one <see cref="Armature.Core.SkipAllUnits"/> build chain pattern.
    /// </summary>
    public static int SkipAllUnits { get; protected set; } = -1_000_000;


    public static int TargetUnit { get; protected set; } = 1_000_000;
  }
}