using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Armature.Framework
{
  /// <summary>
  /// These tokens are used by Armature framework to distinguish internal build steps from possible users for the same types
  /// </summary>
  internal class SpecialToken
  {
    /// <summary>
    /// Is used to build a value for parameter
    /// </summary>
    public static readonly object BuildParameterValue = new SpecialToken("ParameterValue");
    
    /// <summary>
    /// Is use to "build" a <see cref="ConstructorInfo"/> for a type
    /// </summary>
    public static readonly object FindConstructor = new SpecialToken("Constructor");

    private readonly string _description;

    private SpecialToken([NotNull] string description)
    {
       if (description == null) throw new ArgumentNullException("description");
       _description = description;
    }

     public override string ToString()
     {
       return string.Format(typeof(SpecialToken).Name + "." +_description);
     }
  }
}