using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using MassTransit;
using RoomService.Consumers;
using AppDbContext;

namespace RoomService
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


            string secret = Environment.GetEnvironmentVariable("JWT_SECRET");

            services.AddMassTransit(x =>
            {
                x.AddConsumer<CreateRoomConsumer>();
                x.AddConsumer<GetRoomConsumer>();

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
            });

            services.AddMassTransitHostedService();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RoomService", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RoomService v1"));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
