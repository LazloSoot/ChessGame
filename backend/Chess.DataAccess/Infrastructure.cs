using Chess.DataAccess.Interfaces;
using Chess.DataAccess.SqlRepositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chess.DataAccess
{
    public static class DataAccessModule
    {
        /// <summary>
        /// Register DI dependencies
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("ChessGameDb");
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(connectionString);
                options.UseLazyLoadingProxies();
            });

            services.AddScoped(typeof(DbContext), typeof(DataContext));
            services.AddScoped(typeof(IRepository<>), typeof(ChessRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public static void ConfigureMiddleware(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
                context.Database.Migrate();
            }
        }
    }
}
