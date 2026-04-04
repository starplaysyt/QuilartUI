using NatLib.Logging;
using QuilartUI.Exceptions;
using QuilartUI.Interfaces;

namespace QuilartUI.Controllers;

public static class ServiceController
{
    private static readonly Dictionary<Type, QuilartService> Services = new();
    private static ConsoleLogger Logger { get; } = new("ServiceController");

    public static T Get<T>() where T : QuilartService, new()
    {
        var type = typeof(T);
        var services = Services;

        Logger.LogTrace($"[GET] Invoked for {type.Name}...");

        if (services.TryGetValue(type, out var service))
            return (T)service;

        var addedService = new T();
        addedService.Initialize();
        services.Add(type, addedService);
        Logger.LogTrace($"[DONE] Service of type {type.FullName} was registered.");
        return addedService;
    }

    public static void Delete(Type type)
    {
        Logger.LogTrace($"[DELETE] Invoked for {type.FullName}...");
        
        if (!Services.TryGetValue(type, out var service))
            throw new ServiceNotFoundException(type);
        
        service.Exit();
        
        Services.Remove(type);
    }

    public static void Add<T>(QuilartService service) where T : QuilartService
    {
        ArgumentNullException.ThrowIfNull(service);

        var type = typeof(T);

        Logger.LogTrace($"[ADD] Invoked for {type.Name}...");

        if (!Services.TryAdd(type, service))
            throw new ServiceAlreadyExistsException(type);
    }

    public static void Exit()
    {
        Logger.LogTrace("[EXIT] Invoked...");
        foreach (var service in Services)
        {
            Logger.LogTrace($"[EXIT] Invoked for {service.Key}...");
            service.Value.Exit();
        }
        
        Services.Clear();
        Logger.LogTrace($"[DONE] Services exited.");
    }
}