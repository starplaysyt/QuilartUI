namespace QuilartUI.Exceptions;

public class ServiceNotFoundException(Type serviceType)
    : Exception($"Service of type {serviceType.FullName} was not found.");

public class ServiceAlreadyExistsException(Type serviceType)
    : Exception($"Service of type {serviceType.FullName} was already exists.");