# Armature
**The Lightweight, Intuitive, and Highly Extensible Dependency Injection Framework for .NET**<br>

✔ Empowers you to build robust and maintainable .NET applications with a clear and flexible approach to dependency injection.<br>
✔ It's designed to be easy to learn, powerful to use, and to extend when your project demands it.<br>
✔ It gets out of your way, letting you focus on your application logic while providing robust control over object creation and wiring when you need it.<br>

Dive deeper into the **documentation**: [Armature Wiki](https://github.com/Ed-Pavlov/Armature/wiki)


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

* **🚀 Adaptable & Focused: Your Paradigm, Not Ours**:
  Armature stands apart by *not* imposing its own concepts or abstractions onto your application code. You won't find mandatory `Lifetimes`, `Modules`, `Service Providers`, or the other framework-specific structures that you must conform to.<br>
  **Armature** concentrates on the core DI tasks—**you** develop your product.


* **⚡ Explicit DI: No Assumptions, No Magic**<br>
  Armature operates on the principle of **no default behaviour**.
  There's no hidden magic or predetermined outcomes for crucial aspects like constructor selection, property injection, or default parameter handling – **you define the rules explicitly.**<br>
  Crucially, you achieve this fine-grained control not through numerous configuration flags, but by **setting up Armature with specific rules** that define behaviour exactly where it's necessary.


* **🔍 Transparent Logging:**<br>
  Understand exactly how your dependencies are resolved. offering clear, structured, and human-readable insights into the build process. Debugging dependency issues becomes significantly easier. Debugging dependency issues becomes significantly easier.
*Explore [Logging details](https://github.com/Ed-Pavlov/Armature/wiki/Logging).*


* **🔧 Deep Extensibility:**<br>
  Armature is built from the ground up for customization. Tailor every aspect of the DI process—from object creation logic to dependency resolution strategies—without ever needing to modify the framework's source code. When advanced customization is required, create your own Build Actions, Tuners, and Build Stack Patterns to perfectly match your project's unique requirements.
*Learn more about customization in the [Extensibility](https://github.com/Ed-Pavlov/Armature/wiki/Propagation-of-Maybe.-Subscribing-Events.-And-Many-Many-More) section.*