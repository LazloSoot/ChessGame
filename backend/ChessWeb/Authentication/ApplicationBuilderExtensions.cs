using Chess.Common.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace ChessWeb.Authentication
{
    public static class ApplicationBuilderExtensions
    {
        public static IServiceCollection AddFirebaseAuthentication(this IServiceCollection services, string projectId)
        {
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = "https://securetoken.google.com/" + projectId;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        //LifetimeValidator = (before, expires, token, param) =>
                        //{
                        //    return expires > DateTime.UtcNow;
                        //},
                        ValidateIssuer = true,
                        ValidIssuer = "https://securetoken.google.com/" + projectId,
                        ValidateAudience = true,
                        ValidAudience = projectId,
                        ValidateLifetime = true
                    };
                    // for signalR
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithHubsPathSegments()))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            return services;
        }

        private static bool StartsWithHubsPathSegments(this PathString path)
        {
            HubType currentHubType;
            foreach (var name in Enum.GetNames(typeof(HubType)))
            {
                currentHubType = (HubType)Enum.Parse(typeof(HubType), name);
                if (path.StartsWithSegments(currentHubType.GetStringValue()))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
