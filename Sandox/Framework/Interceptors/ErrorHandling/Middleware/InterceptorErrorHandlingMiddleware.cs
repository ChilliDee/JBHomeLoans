using Castle.DynamicProxy;
using System.Reflection;

namespace Mortgage.Interceptors.ErrorHandling.Middleware
{

    [AttributeUsage(AttributeTargets.Method)]
    public class SkipErrorHandlingAttribute : Attribute { }

    // Error handling interceptor
    // Copied from BusinessLogic.Core (FEFramework). The CRM version sends errors to IFrontEndErrorLog_B
    // (database + Teams); the sandbox has neither, so errors are written to the console log instead.
    public class ErrorHandlingInterceptor : IInterceptor
    {

        private string? _className;
        private readonly IServiceProvider _serviceProvider;

        public ErrorHandlingInterceptor(string className, IServiceProvider serviceProvider)
        {
            _className = className;
            _serviceProvider = serviceProvider;
        }

        public void Intercept(IInvocation invocation)
        {
            var method = invocation.Method;

            // Check if method should skip error handling
            var skipAttribute = method.GetCustomAttribute<SkipErrorHandlingAttribute>();
            if (skipAttribute != null)
            {
                invocation.Proceed();
                return;
            }

            try
            {
                invocation.Proceed();

                // Handle async methods
                if (invocation.ReturnValue is Task task)
                {
                    invocation.ReturnValue = HandleAsyncMethod(task);
                }
            }
            catch (TaskCanceledException)
            {
                // do nothing here
            }
            catch (Exception ex)
            {
                HandleError(ex);

                // Set default return value for non-void methods
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

        private void HandleError(Exception ex)
        {
            try
            {
                var logger = _serviceProvider.GetService<ILogger<ErrorHandlingInterceptor>>();
                logger?.LogError(ex, "Unhandled error in {ClassName}", _className);
            }
            catch (Exception)
            {
                // do nothing here
            }
        }

        private void SetDefaultReturnValue(IInvocation invocation)
        {
            var returnType = invocation.Method.ReturnType;

            // Handle void methods
            if (returnType == typeof(void))
                return;

            // Handle Task methods (async void equivalent)
            if (returnType == typeof(Task))
            {
                invocation.ReturnValue = Task.CompletedTask;
                return;
            }

            // Handle Task<T> methods
            if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var taskResultType = returnType.GetGenericArguments()[0];
                var defaultValue = GetDefaultValue(taskResultType);
                invocation.ReturnValue = Task.FromResult(defaultValue);
                return;
            }

            // Handle regular methods
            invocation.ReturnValue = GetDefaultValue(returnType);
        }

        private object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            return null;
        }
    }
}
