using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppDbContext;
using BookingService.Consumers;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BookingService
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var dbConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");

            services.AddDbContext<DatabaseContext>(config => {
                config.UseMySql(
                    dbConnectionString,
                    new MySqlServerVersion(new Version(5, 7)),
                    provider => {
                        provider.EnableRetryOnFailure();
                        provider.MigrationsAssembly("Migrations");
                    });
            });


            services.AddMassTransit(x =>
            {
                x.AddConsumer<GetFreeRoomsConsumer>();

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
