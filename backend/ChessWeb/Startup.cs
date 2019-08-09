using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Chess.BusinessLogic;
using Chess.Common;
using Chess.DataAccess;
using ChessWeb.Authentication;
using Chess.Common.Interfaces;
using ChessGame.Core;

namespace ChessWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    });
            });

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(
                    options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                    );

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserProvider, CurrentUser>();
            services.AddScoped<IChessGame, ChessGameEngine>();
            services.AddFirebaseAuthentication(Configuration.GetValue<string>("Firebase:ProjectId"));
            BuisinessLogicModule.ConfigureServices(services, Configuration);
            DataAccessModule.ConfigureServices(services, Configuration);
            CommonModule.ConfigureServices(services, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseMvc();

            BuisinessLogicModule.ConfigureMiddleware(app, Configuration);
        }
    }
}
