<img src="https://repository-images.githubusercontent.com/268701472/8bf84980-a6ce-11ea-83da-e2133c5a3a7a" alt=".NET DevPack" width="300px" />

# SimpleMediator

A lightweight and straightforward mediator implementation for .NET applications, facilitating in-process messaging with minimal setup.

| Package |  Version | Popularity |
| ------- | ----- | ----- |
| `NetDevPack.SimpleMediator` | [![NuGet](https://img.shields.io/nuget/v/NetDevPack.SimpleMediator.svg)](https://nuget.org/packages/NetDevPack.SimpleMediator) | [![Nuget](https://img.shields.io/nuget/dt/NetDevPack.SimpleMediator.svg)](https://nuget.org/packages/NetDevPack.SimpleMediator) |


## Give a Star! ⭐

If you found this project helpful or it assisted you in any way, please give it a star. It helps to support the project and the developers.

## Samples

You can find complete example projects demonstrating how to use the SimpleMediator in the [`/samples`](./samples) folder.

These include:

- ✅ Basic usage with `Send` and `Publish`
- ✅ Modular application structure
- ✅ Manual and automatic registration of handlers

Feel free to explore and run them to see how the mediator works in different scenarios.

## Getting Started

### Installation

You can install the SimpleMediator package via NuGet Package Manager or the .NET CLI:

```bash
dotnet add package NetDevPack.SimpleMediator
```

### Using Contracts-Only Package

To reference only the contracts for SimpleMediator, which includes:

- `IRequest` (including generic variants)
  - Represents a command or query that expects a single response
- `INotification`
  - Represents an event broadcast to multiple handlers (if any)

### Advanced Usage: Request + Notification

This example demonstrates how to combine a `Request` (command/query) and a `Notification` (event) in a real-world use case.

> ✅ This scenario uses only `Microsoft.Extensions.DependencyInjection.Abstractions` for DI registration — no framework-specific packages.

---

#### 1. Define the Request and Notification

```csharp
public class CreateCustomerCommand : IRequest<string>
{
    public string Name { get; set; }
}

public class CustomerCreatedEvent : INotification
{
    public Guid CustomerId { get; }

    public CustomerCreatedEvent(Guid customerId)
    {
        CustomerId = customerId;
    }
}
```

---

#### 2. Implement the Handlers

```csharp
public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, string>
{
    private readonly IMediator _mediator;

    public CreateCustomerHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<string> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();

        // Simulate persistence...

        // Publish event
        await _mediator.Publish(new CustomerCreatedEvent(id), cancellationToken);

        return $"Customer '{request.Name}' created with ID {id}";
    }
}

public class SendWelcomeEmailHandler : INotificationHandler<CustomerCreatedEvent>
{
    public Task Handle(CustomerCreatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Sending welcome email to customer {notification.CustomerId}");
        return Task.CompletedTask;
    }
}
```

---

#### 3. Register the Handlers (Dependency Injection)

You can register everything manually if you want full control:

```csharp
services.AddSingleton<IMediator, Mediator>();

services.AddTransient<IRequestHandler<CreateCustomerCommand, string>, CreateCustomerHandler>();
services.AddTransient<INotificationHandler<CustomerCreatedEvent>, SendWelcomeEmailHandler>();
```

Or use assembly scanning with:

```csharp
services.AddSimpleMediator(ServiceLifetime.Scoped, typeof(DependencyInjection).Assembly);
```

---

#### 4. Execute the Flow

```csharp
public class CustomerAppService
{
    private readonly IMediator _mediator;

    public CustomerAppService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<string> CreateCustomer(string name)
    {
        return await _mediator.Send(new CreateCustomerCommand { Name = name });
    }
}
```

---

When the `CreateCustomer` method is called:

1. `CreateCustomerHandler` handles the request
2. It creates and persists the customer (simulated)
3. It publishes a `CustomerCreatedEvent`
4. `SendWelcomeEmailHandler` handles the event

This structure cleanly separates **commands** (which change state and return a result) from **notifications** (which communicate to the rest of the system that something happened).

## Features

- **Lightweight**: Minimal dependencies and straightforward setup.
- **In-Process Messaging**: Facilitates in-process communication between components.
- **Handler Registration**: Automatically registers handlers from specified assemblies.

## Compatibility

NetDevPack.SimpleMediator targets .NET Standard 2.1, and is compatible with .NET Core 3.1+, .NET 5+, .NET 6+, .NET 7+, .NET 8, and newer versions of the .NET runtime.

---
# 📦 Release 3.0.0 — Pipeline Behaviors Support & Breaking Changes

## 🚨 Breaking Changes

This release introduces **`PipelineBehaviors` support**, which required internal structural changes to the request processing pipeline. Please review your registration and handler setup to ensure compatibility.

If you're upgrading from a previous version, make sure:

- You re-register handlers and pipeline behaviors using `services.AddSimpleMediator(...)` correctly.
- Custom behavior logic aligns with the new `IPipelineBehavior<TRequest, TResponse>` interface.

---

## ✨ New: Pipeline Behaviors Support

SimpleMediator now supports `PipelineBehaviors`, enabling you to intercept and extend the execution pipeline of requests and notifications — similar to what's available in MediatR.

This allows scenarios such as:

- ✅ Request validation (e.g., with FluentValidation)
- ✅ Logging and tracing
- ✅ Performance measurement
- ✅ Authorization checks
- ✅ Error handling

### Example: Creating a Logging Behavior

```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        Console.WriteLine($"[Log] Handling {typeof(TRequest).Name}");
        var response = await next(cancellationToken);
        Console.WriteLine($"[Log] Handled {typeof(TRequest).Name}");
        return response;
    }
}
```
### Registering Behaviors

```csharp
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
```
Multiple behaviors can be registered and are executed in the order they are registered in the container.

### 🧪 FluentValidation Integration Example

```csharp 
services.AddValidatorsFromAssemblyContaining<MyValidator>();
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

```
## About
> `NetDevPack.SimpleMediator` was originally developed by [Eduardo Pires](https://desenvolvedor.io) under the MIT license.  
> **Note:** This project is a **fork** intended to introduce specific improvements.
