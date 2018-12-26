using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Chess.BusinessLogic.Interfaces;
using Chess.BusinessLogic.Services;
using Microsoft.AspNetCore.Builder;
using Chess.BusinessLogic.Hubs;
using Chess.Common.Helpers;

namespace Chess.BusinessLogic
{
    public static class BuisinessLogicModule
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(ICRUDService<,>), typeof(CRUDService<,>));
            services.AddScoped<IChessMovesService, ChessMovesService>();
            services.AddScoped<IGameDataService, GameDataService>();
            services.AddScoped<IUserService, UserService>();
            
            if (configuration.GetValue<bool>("UseLocalSignalR"))
            {
                services.AddSignalR();
            }
            else
            {
                services.AddSignalR().AddAzureSignalR();
            }
        }

        public static void ConfigureMiddleware(IApplicationBuilder app, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseLocalSignalR"))
            {
                app.UseSignalR(options =>
                {
                    options.MapHub<CommonHub>(HubType.CommonHub.GetStringValue());
                    options.MapHub<NotificationHub>(HubType.NotificationHub.GetStringValue());
                    options.MapHub<ChessGameHub>(HubType.ChessGameHub.GetStringValue());
                });
            }
            else
            {
                app.UseAzureSignalR(options =>
                {
                    options.MapHub<CommonHub>(HubType.CommonHub.GetStringValue());
                    options.MapHub<NotificationHub>(HubType.NotificationHub.GetStringValue());
                    options.MapHub<ChessGameHub>(HubType.ChessGameHub.GetStringValue());
                });
            }
        }
    }
}
