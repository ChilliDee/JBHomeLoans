using System.Reflection;
using Castle.DynamicProxy;

namespace Mortgage.Interceptors.ErrorHandling.Middleware;

// Local Sandox version of the CRM framework classes of the same name (no BusinessLogic.Core package).
// In the CRM an intercepted exception goes to the error log (database + Teams). Here it goes to the console log.

// Put on a method to let its exceptions through untouched.
[AttributeUsage(AttributeTargets.Method)]
public class SkipErrorHandlingAttribute : Attribute
{
}

public class ErrorHandlingInterceptor : IInterceptor
{
    private static readonly MethodInfo _handleAsyncWithResult = typeof(ErrorHandlingInterceptor)
        .GetMethod(nameof(HandleAsyncMethodWithResult), BindingFlags.NonPublic | BindingFlags.Instance)!;

    private readonly string _className;
    private readonly IServiceProvider _serviceProvider;

    public ErrorHandlingInterceptor(string className, IServiceProvider serviceProvider)
    {
        _className = className;
        _serviceProvider = serviceProvider;
    }

    public virtual void Intercept(IInvocation invocation)
    {
        if (invocation.Method.GetCustomAttribute<SkipErrorHandlingAttribute>() != null)
        {
            invocation.Proceed();
            return;
        }

        try
        {
            invocation.Proceed();

            var returnType = invocation.Method.ReturnType;

            if (returnType == typeof(Task) && invocation.ReturnValue is Task task)
            {
                invocation.ReturnValue = HandleAsyncMethod(task);
            }
            else if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>) && invocation.ReturnValue != null)
            {
                invocation.ReturnValue = _handleAsyncWithResult
                    .MakeGenericMethod(returnType.GetGenericArguments()[0])
                    .Invoke(this, new[] { invocation.ReturnValue });
            }
        }
        catch (Exception ex)
        {
            HandleError(ex);
            SetDefaultReturnValue(invocation);
        }
    }

    private async Task HandleAsyncMethod(Task task)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            HandleError(ex);
        }
    }

    private async Task<TResult?> HandleAsyncMethodWithResult<TResult>(Task<TResult> task)
    {
        try
        {
            return await task;
        }
        catch (Exception ex)
        {
            HandleError(ex);
            return default;
        }
    }

    private void HandleError(Exception ex)
    {
        var logger = _serviceProvider.GetService<ILoggerFactory>()?.CreateLogger(_className);

        if (logger != null)
        {
            logger.LogError(ex, "Unhandled error in {ClassName}", _className);
        }
        else
        {
            Console.Error.WriteLine($"[{_className}] {ex}");
        }
    }

    private void SetDefaultReturnValue(IInvocation invocation)
    {
        var returnType = invocation.Method.ReturnType;

        if (returnType == typeof(void))
        {
            return;
        }

        if (returnType == typeof(Task))
        {
            invocation.ReturnValue = Task.CompletedTask;
            return;
        }

        if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var resultType = returnType.GetGenericArguments()[0];
            invocation.ReturnValue = typeof(Task)
                .GetMethod(nameof(Task.FromResult))!
                .MakeGenericMethod(resultType)
                .Invoke(null, new[] { GetDefaultValue(resultType) });
            return;
        }

        invocation.ReturnValue = GetDefaultValue(returnType);
    }

    private object? GetDefaultValue(Type type)
    {
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}
