using System;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Lifetimes;

namespace Tests.Extensibility.LifetimeRegistration
{
  public static class LifetimesExtension
  {
    public static LifetimeDefinition CreateSubLifetimeDefinition(this Lifetime lifetime,  string id)
    {
      if (id == null) throw new ArgumentNullException(nameof(id));

      var ltd = Lifetime.Define(lifetime, id);
      ltd.Lifetime.AssertEverTerminated(id);
      return ltd;
    }

    public static Lifetime CreateSubLifetime(this Lifetime lifetime,  string id)
    {
      return lifetime.CreateSubLifetimeDefinition(id).Lifetime;
    }
  }
}