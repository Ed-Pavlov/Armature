<p align='right'>If <b>Armature</b> has done you any good, consider supporting my future initiatives</p>
<p align="right">
  <a href="https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=ed@pavlov.is&lc=US&item_name=Kudos+for+Armature&no_note=0&cn=&currency_code=EUR">
    <img src="/.build/button.png" width="76" height="32">
  </a>
</p>

___

# Armature

<p align="center">
  <img src="/.build/logo.svg" width="86" height="86">
</p>

**The Lightweight, Intuitive, and Highly Extensible Dependency Injection Framework for .NET**<br>

✔ Empowers you to build robust and maintainable .NET applications with a clear and flexible approach to dependency injection.<br>
✔ It's designed to be easy to learn, powerful to use, and to extend when your project demands it.<br>
✔ It gets out of your way, letting you focus on your application logic while providing robust control over object creation and wiring when you need it.<br>

Dive deeper into the **documentation**: [Armature Wiki](https://github.com/Ed-Pavlov/Armature/wiki)

[![Build & Test](https://github.com/Ed-Pavlov/Armature/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/Ed-Pavlov/Armature/actions/workflows/build-and-test.yml)
![badge](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/Ed-Pavlov/ad5636de8f9cf631d90e2e85d2f3019c/raw/armature-test-coverage.json)
[![Nuget](https://img.shields.io/nuget/dt/BeatyBit.Armature)](https://www.nuget.org/packages/BeatyBit.Armature/)
___

## Powered by
<p align="right">
  <img src="https://resources.jetbrains.com/storage/products/company/brand/logos/Rider.png" width="185" height="64">
</p>

---

## Why Choose Armature?

Armature stands out by focusing on clarity, flexibility, and developer control.

* **✨ Intuitive Fluent API:**<br>
Configure your dependencies using a fluent API that reads like plain English. Armature's DSL concept makes defining object creation and injection rules straightforward and understandable.

```csharp
// Example:
// a bunch of features just to show them off :)
// in real life, it's much simpler than that.

target
 .Treat<IMyInterface>()
 .AsCreated<MyClass>()
 .UsingArguments(ForParameter.OfType<string>().UseValue("MyId"))
 .UsingInjectionPoints(Constructor.WithParameters<string, int>())
 .AsSingleton()
 .BuildingIt()
 .TreatAll()
 .UsingArguments(childLifetime);
```
See [What You Have Out of the Box](https://github.com/Ed-Pavlov/Armature/wiki/Out-of-the-Box) for all features of Armature DSL.*

* **🚀 Adaptable & Focused: Your Paradigm, Not Ours**:<br>
  Armature stands apart by *not* imposing its own concepts or abstractions onto your application code. You won't find mandatory `Lifetimes`, `Modules`, `Service Providers`, or the other framework-specific structures that you must conform to.<br>
  **Armature** concentrates on the core DI tasks—**you** develop your product.


* **✨ Explicit DI: No Assumptions, No Magic**<br>
  Armature operates on the principle of **no default behaviour**.
  There's no hidden magic or predetermined outcomes for crucial aspects like constructor selection, property injection, or default parameter handling – **you define the rules explicitly.**<br>
Crucially, you achieve this fine-grained control not through numerous configuration flags, but by **setting up Armature with specific rules** that define behaviour exactly where it's necessary.


* **🔍 Transparent Logging:**<br>
Understand exactly how your dependencies are resolved. offering clear, structured, and human-readable insights into the build process. Debugging dependency issues becomes significantly easier. Debugging dependency issues becomes significantly easier.

```hocon
// Example of HOCON log output
BuildUnit {
  Time: "2025-14-31 21:14:03.202"
  Thread: 14
  BuildStack = [{ kind: typeof(Subject), tag: null}]

  GatherBuildActions.Result:  [
    { Action: CreateByReflection, Stage: BuildStage.Create, Weight: "1,030,000" }
    { Action: { Singleton{ Instance: nothing } }, Stage: BuildStage.Cache, Weight: "1,030,000" }
  ]
  Singleton.Process {
  }
  CreateByReflection.Process {
    BuildUnit {
      Time: "2025-14-31 21:14:03.211"
      Thread: 14
      BuildStack = [{ kind: typeof(Subject), tag: ServiceTag.Constructor}, { kind: typeof(Subject), tag: null}]

    GatherBuildActions.Result: { Action: GetConstructorWithMaxParametersCount, Stage: BuildStage.Create, Weight: "1,000,000" }
   //...
    }
  }
}
```
*Explore [Logging details](https://github.com/Ed-Pavlov/Armature/wiki/Logging).*


* **🔧 Deep Extensibility:**<br>
  Armature is built from the ground up for customization. Tailor every aspect of the DI process—from object creation logic to dependency resolution strategies—without ever needing to modify the framework's source code. When advanced customization is required, create your own Build Actions, Tuners, and Build Stack Patterns to perfectly match your project's unique requirements.

*Learn more about customization in the [Extensibility](https://github.com/Ed-Pavlov/Armature/wiki/Propagation-of-Maybe.-Subscribing-Events.-And-Many-Many-More) section.*

---

## Key Features

Armature provides a rich set of features for precise control over dependency injection.

### Tuner Hierarchy & Fluent API
Armature's fluent API is powered by a hierarchy of Tuners (`Treat`, `AsCreated`, `UsingArguments`, etc.). These allow you to chain configuration calls logically, specifying *what* to configure, *how* it should be created, *what* dependencies it needs, and its *lifetime*.

```csharp
builder
  .Treat<IMyService>()             // For requests of IMyService...
  .AsCreated<MyServiceImpl>()      // ...create an instance of MyServiceImpl...
  .UsingArguments(                 // ...injecting these arguments:
      ForParameter.OfType<string>().UseValue("config_value"),
      ForParameter.OfType<ILogger>().UseTag("special_logger")
  )
  .AsSingleton();                 // ...and manage it as a singleton.
```
*See [Tuners](https://github.com/Ed-Pavlov/Armature/wiki/Armature-the-DSL-over-Armature.Core) and [Side Tuners](https://github.com/Ed-Pavlov/Armature/wiki/Side-Tuners-Fine-Grained-Control-Over-Dependency-Injection).*

### Build Actions
These are the workhorses performing the actual object creation and configuration (`CreateByReflection`, `InjectDependenciesIntoProperties`, `Singleton`, `Redirect`). Use built-in actions or implement the `IBuildAction` interface to define custom steps in the build process.

```csharp
// Example: Using a built-in action to redirect requests
builder
  .Treat<ILegacyService>()
  .As<INewService>(); // Redirects requests for ILegacyService to INewService
```
*Learn about [Build Actions](https://github.com/Ed-Pavlov/Armature/wiki/Armature-the-DSL-over-Armature.Core).*

### Build Stack Patterns
Define complex, conditional logic for dependency resolution based on the context (the chain of dependencies being built). Patterns like `IfFirstUnit` and `SkipTillUnit` match against the build stack, enabling highly specific configuration rules. Create custom patterns by implementing `IBuildStackPattern`.

*(Complex concept - best explored in the [Core Concepts](https://github.com/Ed-Pavlov/Armature/wiki/Core-Concepts-of-Armature.Core) wiki page)*

### Side Tuners
Refine configurations with targeted modifiers. Side Tuners like `UsingArguments` and `UsingInjectionPoints` accept further specifiers (e.g., `ForParameter`, `Constructor`, `Property`) to precisely control how dependencies are injected or which members are used.

```csharp
// Example: Specifying a constructor using a Side Tuner
builder
  .Treat<MyComplexObject>()
  .UsingInjectionPoints(Constructor.WithParameters<ILogger, string>());
```
*See [Side Tuners](https://github.com/Ed-Pavlov/Armature/wiki/Side-Tuners-Fine-Grained-Control-Over-Dependency-Injection).*


### `[Inject]` Attribute
The `[Inject]` attribute provides a clear way to mark constructors, properties, methods, or parameters in your code that Armature should consider for injection.
However, **using this specific attribute is entirely optional**. Armature's flexibility means you don't *have* to use it.
You can:
* Configure injection based on conventions or other rules **without using attributes at all**.
* **Leverage your own custom attributes** if they already exist in your codebase. Supporting them simply requires creating custom `Build Actions` tailored to your attributes (you can look at how Armature handles `[Inject]` internally for inspiration and guidance).

Note also that this attribute lives in the separate `Armature.Interface` assembly, ensuring your domain code remains decoupled from the core framework logic.

```csharp
public class DataProcessor
{
    public ILogger Logger { get; }

    [Inject] // Mark this constructor for injection
    public DataProcessor([Inject("db")] IDatabase database, ILogger logger)
    {
        Logger = logger;
        // ...
    }
}
```
*Read about the [InjectAttribute](https://github.com/Ed-Pavlov/Armature/wiki/InjectAttribute.-Marking-Injection-Points-and-Customizing-Injection-Rules).*

### Open Generic Types
Configure rules for open generic types and their implementations. Armature simplifies handling common abstractions like repositories or handlers.

```csharp
// Example: Map all ISubject<T> requests to Subject<T>
target
 .TreatOpenGeneric(typeof(ISubject<>))
 .AsCreated(typeof(Subject<>));

// but override rules for Generic Argument type int
target
 .Treat<ISubject<int>>()
 .AsCreated<AnotherSubject<int>>()
 .Using(Constructor.Parameterless());
```

### Weighted Prioritization
Control the order in which configuration rules are applied. Assign weights to rules (using `AmendWeight` or implicitly by specificity) to resolve ambiguity and prioritize certain configurations over others, ensuring predictable dependency resolution.

```csharp
// Example: Giving a specific rule higher priority
builder
  .Treat<IService>()
  .As<DefaultService>(); // Lower priority default

builder
  .Treat<IService>()            // Higher priority rule for IService
  .WhenBuilding<MyController>() // ...only when MyController needs it
  .AmendWeight(10)              // Increase weight to ensure it "wins"
  .As<SpecialService>();
```
*See [Weighted Build Actions and Patterns](https://github.com/Ed-Pavlov/Armature/wiki/Weighted-Build-Actions-and-Patterns.-Prioritizing-Resolution-Paths).*

### Tags for Specificity
Differentiate between multiple registrations of the same type using tags. This allows injecting specific named instances based on context, perfect for scenarios like multiple database connections or feature-specific implementations.

```csharp
// Example: Registering named loggers
builder.Treat<ILogger>("Console").AsCreated<ConsoleLogger>();
builder.Treat<ILogger>("File").AsCreated<FileLogger>();

// Example: Building a specific named instance
var fileLogger = builder.BuildUnit(new UnitId(typeof(ILogger), "File"));
```
*Learn about [Tags in Core Concepts](https://github.com/Ed-Pavlov/Armature/wiki/Core-Concepts-of-Armature.Core).*

---

## Beyond the DSL: The Power of Armature.Core

While the `Armature` assembly provides a convenient DSL for common dependency injection tasks, the underlying engine, `Armature.Core`, is a versatile toolkit. It's the foundation upon which the DSL is built and can be used directly:

* **Create Your Own DSL:** If Armature's DSL doesn't perfectly fit your team's style or project conventions, use Armature.Core to build a custom DSL tailored to your needs.
* **Build Specialized Tools:** Leverage Armature.Core for tasks beyond traditional DI, such as object composition engines, configuration managers, or complex orchestrators.
* **Integrate with Existing Systems:** Its flexible design allows Armature.Core to be integrated smoothly with other frameworks and systems.

Armature gives you the choice: use the convenient built-in DSL or harness the power of the core engine for ultimate flexibility.

---

## Your Framework, Your Rules

Armature doesn't impose a rigid set of default behaviours. You, the developer, define the rules.

* **No Forced Defaults:** Start with a clean slate and add only the rules you need.
* **Reference Implementation:** While there are no enforced defaults, Armature provides examples of common configurations (like constructor selection strategies) in the [Default Rules](https://github.com/Ed-Pavlov/Armature/wiki/Default-Rules-For-Resolving-Unit-and-Its-Dependencies) documentation to guide you.
* **Total Control:** Implement custom `IBuildAction`, `ITuner`, and `IBuildStackPattern` to craft a dependency injection system that perfectly aligns with your application's architecture.

---

## Get Started

Ready to experience a more intuitive and flexible DI framework?

1.  **Install the NuGet package:** `Armature` (or `Armature.Core` if you want to build your own DSL).
2.  **Explore the [Wiki Documentation](https://github.com/Ed-Pavlov/Armature/wiki)** for detailed guides and examples.
3.  **Start configuring!** Use the fluent API to define your application's dependencies.

Armature provides the building blocks – you’re the architect. Build cleaner, more maintainable, and adaptable .NET applications with Armature.