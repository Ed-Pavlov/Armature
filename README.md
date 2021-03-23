[![Nuget](https://img.shields.io/nuget/dt/Armature)](https://www.nuget.org/packages/Armature/)
[![TeamCity](https://edpavlov.beta.teamcity.com/app/rest/builds/buildType:(id:Armature_Build)/statusIcon.svg)](https://edpavlov.beta.teamcity.com/viewType.html?buildTypeId=Armature_Build&guest=1)
___
<p align="center">
  <img src="/build/logo.svg" width="128" height="128">
</p>

# Armature

Lightweight and extremely easily extensible dependency injection framework

## Examples of using

    var builder = new Builder(BuildStage.Cache, BuildStage.Create)
      {
        new AnyUnitSequenceMatcher
        {
          // inject into constructor
          new LastUnitSequenceMatcher(ConstructorMatcher.Instance) // matches if the last unit in a sequence is a constructor
            .AddBuildAction(
              BuildStage.Create,
              new OrderedBuildActionContainer
              {
                new GetInjectPointConstructorBuildAction(), // constructor marked with [Inject] attribute has more priority
                GetLongestConstructorBuildAction.Instance // constructor with largest number of parameters has less priority
              }),


          new LastUnitSequenceMatcher(ParameterValueMatcher.Instance) // matches if the last unit in a sequence is a value for parameter
            .AddBuildAction(
              BuildStage.Create,
              new OrderedBuildActionContainer
              {
                CreateParameterValueForInjectPointBuildAction.Instance,
                CreateParameterValueBuildAction.Instance
              })
        }
      }
    
    
    builder
        .TreatOpenGeneric(typeof(ISubject<>))
        .AsCreated(typeof(Subject<>));
          
    var actual = builder.Build<ISubject<int>>();
    actual.Should().BeOfType<Subject<int>>();
    
    builder
        .Treat<Subject>(token)
        .AsIs()
        .UsingConstructorWithParameters<int, string, Stream>();
    
    builder
        .Treat<Subject>()
        .AsIs()
        .UsingInjectPointConstructor(Subject.InjectPointId);
        
    builder
        .Treat<ISubject>()
        .AsCreated<Subject>()
        .BuildingWhich(_ => _.TreatAll().UsingParameters(logger))
        .AsSingleton();
        
