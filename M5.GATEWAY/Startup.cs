using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

namespace M5.GATEWAY
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
            services.AddAuthentication().AddIdentityServerAuthentication("m5", options =>
            {
                options.Authority = "http://localhost:5001";
                options.ApiName = "gateway";
                options.SupportedTokens = SupportedTokens.Both;
                options.ApiSecret = "secret";
                options.RequireHttpsMetadata = false;
            });

            services.AddOcelot().AddConsul();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseOcelot();
        }
    }
}

// Install-Package Ocelot
// Install-Package IdentityServer4.AccessTokenValidation
// Install-Package Ocelot.Provider.Consul
