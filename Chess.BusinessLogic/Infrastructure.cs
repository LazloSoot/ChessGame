using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Chess.BusinessLogic.Interfaces;
using Chess.BusinessLogic.Services;

namespace Chess.BusinessLogic
{
    public static class BuisinessLogicModule
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(ICRUDService<,>), typeof(CRUDService<,>));
        }
    }
}
