<p align='right'>If <b>Armature</b> has done you any good, consider support my future initiatives</p>
<p align="right">
  <a href="https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=ed@pavlov.is&lc=US&item_name=Kudos+for+Armature&no_note=0&cn=&currency_code=EUR">
    <img src="https://ed.pavlov.is/Images/donate-button-small.png" />
  </a>
</p>

___

[![Nuget](https://img.shields.io/nuget/dt/Armature)](https://www.nuget.org/packages/Armature/)
[![Build & Test](https://github.com/Ed-Pavlov/Armature/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/Ed-Pavlov/Armature/actions/workflows/build-and-test.yml)
___
<p align="center">
  <img src="/build/logo.svg" width="128" height="128">
</p>

**Explore the Wiki:** Explore the [Wiki](https://github.com/Ed-Pavlov/Armature/wiki) for full documentation


# Armature
**The Lightweight, Intuitive, and extremely easy Extensible Dependency Injection Framework for .NET**


Armature is not your average dependency injection (DI) framework. We've built it from the ground up to be:

* **Intuitive:** With Armature's Tuner concept and fluent API, you'll write DI configurations that read like plain English, not cryptic code.
* **Extensible:** Armature embraces customization. Easily tailor the framework to your project's specific needs without ever touching the source code of the framework itself.
* **Lightweight:** Armature stays out of your way. It's designed to be minimal, focusing on the core DI tasks without unnecessary overhead.
* **Powerful:** Armature offers robust features like Build Actions, customizable Build Stacks, and precise argument resolution through Side Tuners.
* **Transparent:** Understand exactly what's happening. Armature's logging uses a human-readable format, giving you clear insights into the DI process.

**Key Features**

* **Tuner Hierarchy:** Fine-tune every aspect of the DI process. Specify which types to tune, how they're created, which constructors or properties to inject into, and much more.
* **Fluent API:** Write clean, expressive configurations. Armature's syntax feels natural and guides you through the process, making complex setups straightforward.
* **Build Actions:** Control the exact steps of object creation and configuration. Use built-in actions or create your own to handle unique requirements.
* **Build Stack Patterns:** Define complex, conditional logic for dependency resolution. This powerful feature gives you unprecedented flexibility.
* **Side Tuners:** Refine your configurations even further. These modifiers let you make targeted adjustments to the injection process, like specifying argument values or selecting particular constructors.
* **Inject Attribute:** Mark injection points clearly in your code with the `[Inject]` attribute. Armature will handle the rest.
* **Open Generic Types:** Handle generic types with ease! Armature supports open generics and their inheritors, making it a breeze to configure common abstractions.

**Example: Creating a Builder with "default" rules**
```c#
new Builder("Root Builder", BuildStage.Cache, BuildStage.Initialize, BuildStage.Create)
       {
           // choose which constructor to call to instantiate an object
           new IfFirstUnit(new IsConstructor())
              .UseBuildAction(
                   new TryInOrder
                   {
                       new GetConstructorByInjectPoint(),         // constructor marked with [Inject] attribute has more priority
                       new GetConstructorWithMaxParametersCount() // constructor with the largest number of parameters has less priority
                   },
                   BuildStage.Create),

           new IfFirstUnit(new IsParameterInfoArray())
              .UseBuildAction(new BuildMethodArgumentsInDirectOrder(), BuildStage.Create), // build arguments for a constructor/method in the order of their parameters

           // build each argument for a constructor/method
           new IfFirstUnit(new IsParameterArgument())
              .UseBuildAction(
                   new TryInOrder
                   {
                     new BuildArgumentByParameterInjectPoint(), // parameter marked with [Inject] attribute has more priority
                     new BuildArgumentByParameterType(),        // if not, try to build it by type
                     new GetParameterDefaultValue()
                   },
                   BuildStage.Create),

           new IfFirstUnit(Unit.Any)
            .UseBuildAction(new InjectDependenciesIntoProperties(), BuildStage.Initialize), // try to inject dependencies into property of any built unit

           new IfFirstUnit(new IsPropertyInfoCollection())
            .UseBuildAction(new GetPropertyListByInjectAttribute(), BuildStage.Create), // inject dependencies into all properties marked with [Inject] attribute

           new IfFirstUnit(new IsPropertyArgument())
            .UseBuildAction(new BuildArgumentByPropertyInjectPoint(), BuildStage.Create) // use a property type and InjectAttribute.Tag as UnitId.Tag to build argument for a property
       }
```
---
**Example: Open Generic Types**
```c#
// Create instances of Child<T> for all requests to Base<T>
builder.TreatOpenGeneric(typeof(Base<>)).AsCreated(typeof(Child<>));

// Apply special rules for any requests to Base<int> and its inheritors
builder
  .TreatInheritorsOf<Base<int>>()
  .Using(Constructor.Parameterless(), ForPropety.OfType<int>().UseFactoryMethod(...));
```
**Example: Simple Types**
```c#
builder
  .Treat<IMyInterface>()
  .AsCreated<MyClass>()
  .UsingArguments(ForParameter.OfType<string>().UseValue("identity"))
  .AsSingleton()
  .BuildingIt()
  .Treat<Stream>()
  .UsingFactoryMethod(bs => buildSession... )
```
---
**Armature: A DSL for Dependency Injection, But Not Your Limit**

Armature offers a powerful Domain-Specific Language (DSL) that simplifies the configuration of dependency injection. This DSL, built on top of Armature.Core, provides a fluent and intuitive way to express how your objects should be created, wired together, and managed.
While Armature's DSL is a great starting point, it's not your only option. Armature.Core, the engine behind the DSL, is a versatile toolkit that allows you to build your own custom DI solutions.

* **Create Your Own DSL:** If you have specific requirements or preferences, you can craft a DSL that perfectly aligns with your project's conventions and style.
* **Build Specialized Tools:** Armature.Core can be used to create tools beyond traditional DI frameworks. You could build an object composition engine, a configuration management system, or anything else that requires the flexible assembly of components.
* **Integrate with Existing Systems:** Seamlessly integrate Armature.Core with your existing codebase or other frameworks. Its modular design allows for easy adaptation and extension.

**The Power of Choice**

Armature gives you the freedom to choose the best approach for your project. Whether you prefer the convenience of the built-in DSL or the flexibility of building your own tools, Armature.Core empowers you to create a dependency injection solution that fits your needs perfectly.

**Unlock Your Potential**

Armature is more than just a framework; it's a platform for innovation. By understanding the distinction between the DSL and the underlying core, you can unlock new possibilities and create truly unique solutions for your .NET applications.

---

**Build Your Own Rules with Armature**

Armature gives you the building blocks, but you're the architect. Here's how you can create your own dependency injection masterpiece:

* **Custom Build Actions:** Implement the `IBuildAction` interface to define precisely how your objects are created, their dependencies injected, and any post-creation logic.

* **Custom Tuners:** Extend the `ITuner` interface to create your own tuners. Tailor the injection process to your exact needs, adding new configuration options or modifying existing behavior.

* **Custom Build Stack Patterns:** Dive deeper into the resolution process. By implementing `IBuildStackPattern`, you can create complex patterns to match specific build stacks and execute your custom build actions.

* **Weighted Build Actions and Patterns:** Set priorities for different build actions and patterns using numerical weights. This gives you granular control over which path Armature takes to resolve dependencies.

* **Default Rules as a Guide:** While Armature doesn't enforce defaults, we provide a set of "default-like" rules as a reference implementation.  Learn from these examples and use them as a starting point for your own configurations.


**Under the Hood and Beyond**

Armature is more than just a DI framework. It's a philosophy of extensibility and customization:

* **Internal Logic as Units:** Armature uses its own build process to handle internal logic like constructor selection or argument resolution. This means you can apply the same rules and patterns you use for your code to Armature itself, opening up even more customization possibilities.
* **Tags for Specificity:** Differentiate between multiple implementations of the same interface or type with Tags. This allows you to easily switch between implementations based on the context, like using a mock logger for testing.
* **HOCON Logging:** Armature's logs are formatted in the easy-to-read HOCON format, giving you clear insights into how dependencies are resolved and built.
* **Built on Armature.Core:** Armature's foundation is a powerful, low-level toolkit. You can leverage this core to build your own dependency injection frameworks or other tools that require seamless component integration.
