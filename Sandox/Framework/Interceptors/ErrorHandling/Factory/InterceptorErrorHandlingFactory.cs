using Castle.DynamicProxy;
using Mortgage.Interceptors.ErrorHandling.Middleware;

namespace Mortgage.Interceptors.ErrorHandling.Factory
{
    public class InterceptorErrorHandlingFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ProxyGenerator _proxyGenerator;

        public InterceptorErrorHandlingFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _proxyGenerator = new();
        }

        public T Create<T, U>(Type classType, U argument,T target, string baseClassName)
        {
            string fullClassHierachy = $"{baseClassName}.{classType.Name}";

            var interceptor = new ErrorHandlingInterceptor(fullClassHierachy, _serviceProvider);

            var arguments = new object[] { argument };  

            return (T)_proxyGenerator.CreateClassProxyWithTarget(classType, target, new ProxyGenerationOptions(), arguments, interceptor);
        }
    }
}