using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Armature.Framework
{
  /// <summary>
  ///   These tokens are used by Armature framework to distinguish internal build steps from possible users for the same types
  /// </summary>
  internal class SpecialToken
  {
    /// <summary>
    ///   Is used to build a value for parameter
    /// </summary>
    public static readonly object ParameterValue = new SpecialToken("ParameterValue");

    /// <summary>
    ///   Is used to "build" a <see cref="ConstructorInfo" /> for a type
    /// </summary>
    public static readonly object Constructor = new SpecialToken("Constructor");

    private readonly string _description;

    private SpecialToken([NotNull] string description)
    {
      if (description == null) throw new ArgumentNullException(nameof(description));

      _description = description;
    }

    public override string ToString() => string.Format(typeof(SpecialToken).Name + "." + _description);
  }
}