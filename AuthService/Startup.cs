using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;
using MassTransit;
using AuthService.Consumers;
using Entities;
using AppDbContext;
using Models;

namespace AuthService
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
                config.UseMySql(
                    dbConnectionString,
                    new MySqlServerVersion(new Version(5, 7)),
                    provider => {
                        provider.EnableRetryOnFailure();
                        provider.MigrationsAssembly("Migrations");
                    });
            });

            

            services
                .AddIdentityCore<UserEntity>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DatabaseContext>()
                .AddDefaultTokenProviders();

            string secret = Environment.GetEnvironmentVariable("JWT_SECRET");

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
                    };
                }); ;

            services.AddMassTransit(x =>
            {
                x.AddConsumer<UserConsumer>();
                x.AddConsumer<LoginConsumer>();
                x.AddConsumer<RegisterConsumer>();
                x.AddConsumer<DeleteUserConsumer>();
                x.AddConsumer<VerifyEmailConsumer>();

                x.AddRequestClient<SendMailModel>();

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
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AuthService", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AuthService v1"));
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            CreateRoles(serviceProvider);
            CreateAdmin(serviceProvider);
        }

        private void CreateRoles(IServiceProvider serviceProvider)
        {

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            Task<IdentityResult> roleResult;

            Task<bool> hasAdminRole = roleManager.RoleExistsAsync("Administrator");
            hasAdminRole.Wait();

            if (!hasAdminRole.Result)
            {
                roleResult = roleManager.CreateAsync(new IdentityRole("Administrator"));
                roleResult.Wait();
            }

            Task<bool> hasUserRole = roleManager.RoleExistsAsync("User");
            hasUserRole.Wait();

            if (!hasUserRole.Result)
            {
                roleResult = roleManager.CreateAsync(new IdentityRole("User"));
                roleResult.Wait();
            }

            Task<bool> hasDeletedRole = roleManager.RoleExistsAsync("Deleted");
            hasDeletedRole.Wait();

            if (!hasDeletedRole.Result)
            {
                roleResult = roleManager.CreateAsync(new IdentityRole("Deleted"));
                roleResult.Wait();
            }
        }

        private void CreateAdmin(IServiceProvider serviceProvider)
        {
            Task<IdentityResult> addAdminResult;

            var userManager = serviceProvider.GetRequiredService<UserManager<UserEntity>>();

            Task<UserEntity> adminExists = userManager.FindByNameAsync("admin@houserent.pl");
            adminExists.Wait();

            if(adminExists.Result == null)
            {
                UserEntity admin = new()
                {
                    Email = Environment.GetEnvironmentVariable("DEFAULT_ADMIN_MAIL"),
                    UserName = Environment.GetEnvironmentVariable("DEFAULT_ADMIN_MAIL"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = "Admin",
                    LastName = "Admin",
                    EmailConfirmed = true
                };


                addAdminResult = userManager.CreateAsync(admin, Environment.GetEnvironmentVariable("DEFAULT_ADMIN_PASSWORD"));
                addAdminResult.Wait();

                addAdminResult = userManager.AddToRoleAsync(admin, "Administrator");
                addAdminResult.Wait();
               

                if (!addAdminResult.IsCompletedSuccessfully)
                    Console.WriteLine("Couldn't create default admin account");
            }

        }
    }
}
