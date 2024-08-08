using Microsoft.EntityFrameworkCore;
using UrlShorten.DataAccess.Context;

namespace WorkHub.Web.Extensions
{
    public static class ServiceCollectionExtension
    {
		public static IServiceCollection AddDatabaseConfig(this IServiceCollection services,
		 IConfiguration configuration)
		{
			services.AddDbContext<UrlShortenDbContext>(
				dbContextOptions => dbContextOptions
					.UseSqlServer(configuration.GetConnectionString("UrlShortenDatabaseConnection")));

			return services;
		}
	}
}
