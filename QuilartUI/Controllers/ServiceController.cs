using NatLib.Logging;
using QuilartUI.Interfaces;

namespace QuilartUI.Controllers;

public static class ServiceController
{
    private static readonly Dictionary<Type, IQuilartService> Services = new();
    private static ConsoleLogger Logger { get; } = new("ServiceController");

    public static T GetOrCreateService<T>() where T : IQuilartService, new()
    {
        var type = typeof(T);
        var services = Services;

        Logger.LogTrace($"Invoked GetOrCreateService for {type.Name}...");

        if (services.TryGetValue(type, out var service))
            return (T)service;

        var addedService = new T();
        addedService.Initialize();
        services.Add(type, addedService);
        Logger.LogTrace($"Service of type {type.FullName} was registered.");
        return addedService;
    }

    public static T GetService<T>() where T : IQuilartService
    {
        var type = typeof(T);
        var services = Services;

        Logger.LogTrace($"Invoked GetService for {type.Name}...");

        if (services.TryGetValue(type, out var service))
            return (T)service;
        throw new KeyNotFoundException($"Service of type {type.FullName} was not registered.");
    }

    public static void Create<T>() where T : IQuilartService, new()
    {
        var type = typeof(T);
        var services = Services;

        Logger.LogTrace($"Invoked CreateService for {type.Name}...");

        if (services.ContainsKey(type))
            throw new InvalidOperationException($"Service of type {type.FullName} was already registered.");

        var service = new T();
        service.Initialize();
        services.Add(type, service);
    }

    public static void Delete<T>() where T : IQuilartService
    {
        var type = typeof(T);

        Logger.LogTrace($"Invoked Delete for {type.Name}...");
        
        if (!Services.TryGetValue(type, out var service))
            throw new InvalidOperationException($"Service of type {type.FullName} was not registered.");
        
        service.Exit();
        
        Services.Remove(type);
    }

    public static void Delete(Type type)
    {
        Logger.LogTrace($"Invoked Delete for {type.FullName}...");
        
        if (!Services.TryGetValue(type, out var service))
            throw new InvalidOperationException($"Service of type {type.FullName} was not registered.");
        
        service.Exit();
        
        Services.Remove(type);
    }

    public static void Add<T>(IQuilartService service) where T : IQuilartService
    {
        ArgumentNullException.ThrowIfNull(service);

        var type = typeof(T);

        Logger.LogTrace($"Invoked Add for {type.Name}...");

        if (!Services.TryAdd(type, service))
            throw new InvalidOperationException($"Service of type {type.FullName} was already registered.");
    }

    public static void DeleteAll()
    {
        Logger.LogTrace($"Invoked DeleteAll...");
        foreach (var service in Services)
        {
            Delete(service.Key);
        }
        Logger.LogTrace($"All services disposed and deleted.");
    }
}