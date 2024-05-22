If **Armature** has done you any good, consider supporting my future initiatives

[![PayPal](https://camo.githubusercontent.com/4afcb28ce754b30fa245883450d3e3dc42385d67afcbb14b801090ce6d6cfabb/68747470733a2f2f65642e7061766c6f762e69732f496d616765732f646f6e6174652d627574746f6e2d736d616c6c2e706e67)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=ed@pavlov.is&lc=US&item_name=Kudos+for+Armature&no_note=0&cn=&currency_code=EUR)
___
# Armature

Lightweight and extremely easy extensible dependency injection framework

## Example of extensibility
See the project `Tests.Extensibility` to see the example of powerful extensibility possibility without changing the framework itself.
It adds support for `Maybe` as an injected value.

## Examples of using
    var builder = new Builder(BuildStage.Cache, BuildStage.Initialize, BuildStage.Create)
    {
      new SkipAllUnits
      {
        // inject into constructor
        new IfFirstUnit(new IsConstructor())
         .UseBuildAction(
            new TryInOrder(
              new GetConstructorByInjectPointId(),
              new GetConstructorWithMaxParametersCount()
            ),
            BuildStage.Create),
        new IfFirstUnit(new IsParameterInfoList())
         .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create),
        new IfFirstUnit(new IsParameterInfo())
         .UseBuildAction(new BuildArgumentByParameterType(), BuildStage.Create)
      }
    };

    builder
     .TreatOpenGeneric(typeof(ISubject<>))
     .AsCreated(typeof(Subject<>));

    builder
     .Treat<Subject>(tag)
     .AsIs()
     .InjectInto(Constructor.WithParameters<int, string, Stream>());

    builder
     .Treat<Subject>()
     .AsIs()
     .InjectInto(Constructor.MarkedWithInjectAttribute(Subject.InjectPointId));

    builder
     .Treat<ISubject>()
     .AsCreated<Subject>()
     .AsSingleton();

    builder
     .Building<ISubject>()
     .Building<Subject>()
     .TreatAll()
     .UsingArguments(logger);