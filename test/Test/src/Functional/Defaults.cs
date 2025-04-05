using BeatyBit.Armature;
using BeatyBit.Armature.Core;
using NUnit.Framework;

namespace Armature.Test.Functional;

public class Defaults
{
  [Test]
  public void test()
  {
    var target = new Builder("test", BuildStage.Cache, BuildStage.Create);

    target
     .TreatAll()
     .UsingInjectionPoints(
        Constructor.TryInOrder(
          constructor =>
          {
            constructor.WithMaxParametersCount();
            constructor.MarkedWithInjectAttribute(null);
          }
        ),
        Property.AllProperties(),
        Property.ByInjectPointTag()
      )
     .Using(
        Autowiring.ResolveArgumentsInDirectOrder(),
        ArgumentResolver.TryInOrder(
          build =>
          {
            build.ArgumentByParameterInjectPoint();
            build.ArgumentByParameterType();
            build.ParameterDefaultValue();
          }
        ));

    target.PrintToLog();
  }
}
