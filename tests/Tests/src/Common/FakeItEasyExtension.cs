using FakeItEasy;
using FakeItEasy.Configuration;

namespace Tests.Common;

public static class FakeItEasyExtension
{
  public static void MustHaveHappenedOnceAndOnly<TInterface>(this IReturnValueArgumentValidationConfiguration<TInterface> configuration)
  {
    // once exactly call with specified and with any arguments means only call with specified arguments was performed
    configuration.MustHaveHappenedOnceExactly();
    configuration.WithAnyArguments().MustHaveHappenedOnceExactly();
  }
}