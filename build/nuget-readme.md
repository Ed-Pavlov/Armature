<p align='right'>If <b>Armature</b> has done you any good, consider support my future initiatives</p>
<p align="right">
  <a href="https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=ed@pavlov.is&lc=US&item_name=Kudos+for+Armature&no_note=0&cn=&currency_code=EUR">
    <img src="https://ed.pavlov.is/Images/donate-button-small.png" />
  </a>
</p>

___
# Armature

Lightweight and extremely easily extensible dependency injection framework

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