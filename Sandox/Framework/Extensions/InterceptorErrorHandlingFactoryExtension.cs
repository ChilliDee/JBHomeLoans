using Mortgage.Interceptors.ErrorHandling.Factory;

namespace FE.Framework.Extensions
{
    public static class InterceptorErrorHandlingFactoryExtension
    {
        public static void AddFEInterceptorErrorHandlingFactory(this IServiceCollection services)
        {
            services.AddScoped<InterceptorErrorHandlingFactory>();
        }
    }
}
