using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
//using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Microsoft.OpenApi.Models;
using hotel.Helpers;
using hotel.Interfaces;
using hotel.Middlewares;
using MassTransit;
using Entities;
using Models;
using System.Collections.Generic;
using AppDbContext;
using Microsoft.EntityFrameworkCore;
using main.Controllers;

namespace hotel
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
            var dbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");

            services.AddDbContext<DatabaseContext>(config => {
                config.UseMySql(dbConnectionString, new MySqlServerVersion(new Version(5, 7)), provider => provider.EnableRetryOnFailure());
            });

            services.AddMassTransit(x =>
            {
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(config =>
                {
                    config.UseHealthCheck(provider);
                    config.Host(new Uri(Environment.GetEnvironmentVariable("RABBIT_HOSTNAME")), h =>
                    {
                        h.Username(Environment.GetEnvironmentVariable("RABBIT_USER"));
                        h.Password(Environment.GetEnvironmentVariable("RABBIT_PASSWORD"));
                    });
                    config.ConfigureEndpoints(provider);
                }));

                // user clients
                x.AddRequestClient<UserListEntity>();
                x.AddRequestClient<RegisterModel>();
                x.AddRequestClient<LoginModel>();
                x.AddRequestClient<DeleteUserModel>();
                x.AddRequestClient<VerifyEmailModel>();

                // room clients
                x.AddRequestClient<RoomListModel>();
                x.AddRequestClient<CreateRoomModel>();
                x.AddRequestClient<DeleteRoomModel>();
                x.AddRequestClient<UpdateRoomModel>();

                //booking clients
                x.AddRequestClient<GetFreeRoomsModel>();
                x.AddRequestClient<MakeBookingModel>();

            });

            services.AddMassTransitHostedService();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "hotel", Version = "v1" });
            });

            services.AddSingleton<ITokenHelper, TokenHelper>();
            services.AddSingleton<IMediaController, MediaController>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "hotel v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<AuthMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
