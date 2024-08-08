using UrlShorten.DataAccess.UnitOfWork;

namespace WorkHub.Web.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection RegisterWebServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
