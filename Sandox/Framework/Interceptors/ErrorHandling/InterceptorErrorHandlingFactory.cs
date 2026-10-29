using Castle.DynamicProxy;
using Mortgage.Interceptors.ErrorHandling.Middleware;

namespace Mortgage.Interceptors.ErrorHandling.Factory;

// Local Sandox version of the CRM framework class of the same name (no BusinessLogic.Core package).
// Wraps a component's inner class (Request, Action, State...) so that an exception thrown by one of its
// virtual methods is logged to the console instead of crashing the circuit.
public class InterceptorErrorHandlingFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ProxyGenerator _proxyGenerator = new();

    public InterceptorErrorHandlingFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    // classType: the inner class to wrap. argument: its constructor argument (the component's Dependencies).
    // target: an instance of it. baseClassName: the owning component's name, used in the log line.
    public T Create<T, U>(Type classType, U argument, T target, string baseClassName) where T : class
    {
        var interceptor = new ErrorHandlingInterceptor($"{baseClassName}.{classType.Name}", _serviceProvider);

        return (T)_proxyGenerator.CreateClassProxyWithTarget(
            classType,
            Type.EmptyTypes,
            target,
            ProxyGenerationOptions.Default,
            new object?[] { argument },
            interceptor);
    }
}

public static class FEInterceptorErrorHandlingFactoryExtension
{
    public static void AddFEInterceptorErrorHandlingFactory(this IServiceCollection services)
    {
        services.AddScoped<InterceptorErrorHandlingFactory>();
    }
}
