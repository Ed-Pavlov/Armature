/* MIT License

Copyright (c) 2016 JetBrains http://www.jetbrains.com

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE. */

using System;
using System.Diagnostics;

// ReSharper disable UnusedType.Global
// ReSharper disable ArrangeConstructorOrDestructorBody

#pragma warning disable 1591

// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable IntroduceOptionalParameters.Global
// ReSharper disable MemberCanBeProtected.Global

// ReSharper disable once CheckNamespace
namespace JetBrains.Annotations
{
  /// <summary>
  /// Tells the code analysis engine if the parameter is completely handled when the invoked method is on stack.
  /// If the parameter is a delegate, indicates that delegate is executed while the method is executed.
  /// If the parameter is an enumerable, indicates that it is enumerated while the method is executed.
  /// </summary>
  [AttributeUsage(AttributeTargets.Parameter)]
  [Conditional("JETBRAINS_ANNOTATIONS")]
  internal sealed class InstantHandleAttribute : Attribute { }

  /// <summary>
  /// Indicates that the marked method builds string by the format pattern and (optional) arguments.
  /// The parameter, which contains the format string, should be given in the constructor. The format string
  /// should be in <see cref="string.Format(IFormatProvider,string,object[])"/>-like form.
  /// </summary>
  /// <example><code>
  /// [StringFormatMethod("message")]
  /// void ShowError(string message, params object[] args) { /* do something */ }
  ///
  /// void Foo() {
  ///   ShowError("Failed: {0}"); // Warning: Non-existing argument in format string
  /// }
  /// </code></example>
  [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Delegate)]
  [Conditional("JETBRAINS_ANNOTATIONS")]
  internal sealed class StringFormatMethodAttribute : Attribute
  {
    /// <param name="formatParameterName">
    /// Specifies which parameter of an annotated method should be treated as the format string
    /// </param>
    public StringFormatMethodAttribute(string formatParameterName)
    {
      FormatParameterName = formatParameterName;
    }

    public string FormatParameterName { get; }
  }

  /// <summary>
  /// Indicates that the marked parameter is a message template where placeholders are to be replaced by the following arguments
  /// in the order in which they appear
  /// </summary>
  /// <example><code>
  /// void LogInfo([StructuredMessageTemplate]string message, params object[] args) { /* do something */ }
  ///
  /// void Foo() {
  ///   LogInfo("User created: {username}"); // Warning: Non-existing argument in format string
  /// }
  /// </code></example>
  [AttributeUsage(AttributeTargets.Parameter)]
  [Conditional("JETBRAINS_ANNOTATIONS")]
  internal sealed class StructuredMessageTemplateAttribute : Attribute { }
}