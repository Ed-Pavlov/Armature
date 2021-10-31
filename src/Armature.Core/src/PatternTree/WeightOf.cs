using System.Diagnostics.CodeAnalysis;

namespace Armature.Core
{
  /// <summary>
  /// Inherit this class to extend enum pattern with custom weights
  /// </summary>
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public class WeightOf
  {
    public const byte Step   = 10;
    public const byte Lowest = Step * 2;

    public const byte OpenGenericPattern = Lowest             + Step;
    public const byte SubtypePattern     = OpenGenericPattern + Step;
    public const byte StrictPattern      = SubtypePattern     + Step;

    /// <summary>
    /// <see cref="Core.SkipAllUnits"/> building unit sequence pattern is used to set "default" rules for any building unit if it doesn't
    /// have a specific registration.
    /// That is why by default its weight is set very low, but not to the minimal possible value to leave a gap for user's needs.
    /// </summary>
    public const short SkipAllUnits = -400;

    /// <summary>
    /// By default the weight of <see cref="Core.IfFirstUnit"/> building unit sequence pattern's weight is increased in order to registrations
    /// like
    ///  builder.GetOrAddNode(new SkipTillUnit(new Pattern(typeof(MyType))))
    ///         .GetOrAddNode(new IfFirstUnit(new IsAssignableFromType(typeof(string))))
    ///    // ....
    /// "win" registrations like
    ///
    ///  builder.GetOrAddNode(new SkipTillUnit(new Pattern(typeof(MyType))))
    ///         .GetOrAddNode(new SkipTillUnit(new IsAssignableFromType(typeof(string))))
    ///    // ....
    ///
    /// Because the first one is a "personal" registration whereas the second one will be applied to all units building
    /// in the context of "MyType".
    ///
    /// Note that provided sample is "synthetic" see <see cref="SpecialKey"/> and <see cref="SkipWhileUnit"/> for details.
    /// </summary>
    public const short IfFirstUnit = 200;
  }
}