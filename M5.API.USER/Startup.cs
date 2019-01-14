using System;
using System.IdentityModel.Tokens.Jwt;
using Consul;
using M5.API.USER.Framework.Application.Queries;
using M5.API.USER.Framework.Domain;
using M5.API.USER.Framework.Infrastructure;
using M5.API.USER.Framework.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace M5.API.USER
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.Audience = "api";
                        options.Authority = "http://127.0.0.1:5000";
                        options.SaveToken = true;
                    });

            services.AddDbContext<EFContext>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("ConnectionMySQL"));
            });

            services.AddScoped<IUserQuery, UserQuery>()
                    .AddScoped<IUserRepository, UserRepository>(serviceProvider =>
                    {
                        return new UserRepository(serviceProvider.GetRequiredService<EFContext>());
                    });

            services.AddCap(options =>
            {
                options.UseEntityFramework<EFContext>();
                options.UseRabbitMQ("localhost");// http://localhost:15672
                options.UseDashboard();// http://localhost:5002/cap
                options.UseDiscovery(v =>
                {
                    v.DiscoveryServerHostName = "localhost";
                    v.DiscoveryServerPort = 8500;
                    v.CurrentNodeHostName = "localhost";
                    v.CurrentNodePort = 5002;
                    v.NodeId = 5002;
                    v.NodeName = "m5_api_user_cap";
                });
            });

            services.AddMediatR();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region Service Register
            var name = "m5_api_user";
            var address = "127.0.0.1";
            var port = 5002;
            var id = $"{name}_{address.Replace('.', '_')}_{port}";

            var consulClient = new ConsulClient();

            consulClient.Agent.ServiceRegister(new AgentServiceRegistration
            {
                ID = id,
                Name = name,
                Address = address,
                Port = port,
                Checks = new[] {
                    new AgentServiceCheck
                    {
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                        Interval = TimeSpan.FromSeconds(10),
                        HTTP = $"http://{address}:{port}/api/health/detect",
                        Timeout = TimeSpan.FromSeconds(5)
                    }
                }
            }).GetAwaiter().GetResult();

            lifetime.ApplicationStopped.Register(() =>
            {
                consulClient.Agent.ServiceDeregister(id).GetAwaiter().GetResult();
            });
            #endregion

            app.UseAuthentication();
            app.UseMvc();
        }
    }
}

// Install-Package Microsoft.EntityFrameworkCore
// Install-Package Microsoft.EntityFrameworkCore.Tools
// Install-Package Mysql.Data.EntityFrameworkCore
// Install-Package Pomelo.EntityFrameworkCore.MySql
// Install-Package Consul
// Install-Package DotNetCore.CAP
// Install-Package DotNetCore.CAP.RabbitMQ
// Install-Package DotNetCore.CAP.MySql
// Install-Package MediatR
// Install-Package MediatR.Extensions.Microsoft.DependencyInjection