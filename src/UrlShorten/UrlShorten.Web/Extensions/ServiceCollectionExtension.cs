using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UrlShorten.ApplicationIdentity.Context;
using UrlShorten.ApplicationIdentity.Manager;
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

            services.AddDbContext<ApplicationIdentityDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("UrlShortenDatabaseConnection")));

            return services;
		}

        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<ApplicationIdentityUser, IdentityRole<Guid>>(
                options =>
                {
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireLowercase = false;

                })
                .AddEntityFrameworkStores<ApplicationIdentityDbContext>();

            return services;
        }
    }
}
