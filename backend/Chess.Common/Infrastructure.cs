using AutoMapper;
using Chess.Common.Mappings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chess.Common
{
    public static class CommonModule
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IMapper>(m => Automapper.GetDefaultMapper());
        }
    }
}
