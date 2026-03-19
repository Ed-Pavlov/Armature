# Project Architecture & Coding Guidelines for AI Agents

Follow these strict rules when generating or modifying code for this project.


## 1. Test Style & Formatting
**RULE:** Use `// --arrange`, `// --act`, `// --assert` comments to clearly separate test steps.
**RULE:** Use `should_be_%assertion%_when_%condition%` or `%subject%_should_be_%assertion%_when_%condition%` naming convention for test methods.
**RULE:** Use FluentAssertions for assertions.
**RULE**: Do not use [SetUp] and [TearDown] NUnit methods to create temporary assets for tests, create them in each test method and use `Lifetime` to cleanup
resources.