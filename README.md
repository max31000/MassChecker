# MassChecker Documentation

## Overview

MassChecker provides a flexible and powerful mechanism to apply complex filtering logic on collections of objects. It consists of two main components: `MassCheckerBase<TItem, TFilter>` and `CheckerAttribute`. This library allows you to define filters with multiple conditions in a clean and maintainable way. It's particularly useful in scenarios where you need to apply various filtering criteria based on user input or other dynamic data sources.

## Getting Started

### Installation

To use MassChecker, add it to your project as a dependency. If MassChecker is hosted on a package repository, such as NuGet, you can install it using the package manager console:

```bash
Install-Package MassChecker
Install-Package MassChecker.Analyzers
```

Or, if you're using .NET CLI:

```bash
dotnet add package MassChecker
dotnet add package MassChecker.Analyzers
```

### Basic Usage

To create a new filter, extend the `MassCheckerBase<TItem, TFilter>` class and decorate your filtering methods with the `[Checker]` attribute. Here's a simple example:

```csharp
public class RoflanFilter : MassCheckerBase<RoflanDto, RoflanFilterDto>
{
    [Checker]
    private bool CheckNumber(RoflanDto roflanDto, RoflanFilterDto filterDto)
    {
        return filterDto.Number == null || roflanDto.Number == filterDto.Number;
    }

    [Checker]
    private bool CheckName(RoflanDto roflanDto, RoflanFilterDto filterDto)
    {
        return filterDto.Name == null || roflanDto.Name == filterDto.Name;
    }

    [Checker]
    private bool CheckDate(RoflanDto roflanDto, RoflanFilterDto filterDto)
    {
        return (filterDto.DateFrom == null && filterDto.DateTo == null) 
               || (roflanDto.Date >= filterDto.DateFrom && roflanDto.Date <= filterDto.DateTo);
    }
}
```

The `MassCheckerBase` class provides a method `NeedItem` that evaluates all checkers against a given item and filter combination, returning `true` if all checkers pass. You can override this method if you need more complex methods checking logic. For this purpose, the base class provides the internal property `Checkers`.

### Dependency Injection (DI) Setup

To maintain high performance, it is recommended to register your filters with a DI container and resolve them as singletons. This approach ensures that the reflection-heavy constructor of `MassCheckerBase` is called only once. Here's an example of how you might register a filter in a .NET Core application using the default DI container:

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Register your filters as singletons
    services.AddSingleton<RoflanFilter>();
}
```

## Performance Considerations

Direct instantiation of filter classes derived from `MassCheckerBase` is discouraged due to the reflection used in their constructors. Instead, rely on DI containers to manage their lifecycle as singletons. This approach significantly mitigates the performance overhead and ensures that your application remains responsive.

## Conclusion

MassChecker offers a robust solution for implementing complex filtering logic in .NET applications. By following the guidelines provided in this document, you can integrate MassChecker into your project and benefit from its powerful features while maintaining high performance.

For further details, issues, or contributions, please refer to the project repository on GitHub.
