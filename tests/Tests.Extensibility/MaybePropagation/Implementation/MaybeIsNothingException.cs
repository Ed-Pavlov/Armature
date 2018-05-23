using System;

namespace Tests.Extensibility.MaybePropagation.Implementation
{
  /// <summary>
  ///   Is used by <see cref="GetMaybeValueBuildAction{T}" /> and <see cref="Extension.AsMaybeValueOf{T}" /> to indicate that there is no value in created
  ///   <see cref="Maybe{T}" />.
  /// </summary>
  public class MaybeIsNothingException : Exception
  {
  }
}