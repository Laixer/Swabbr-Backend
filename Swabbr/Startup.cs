using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swabbr.Api.Extensions;
using Swabbr.Api.Options;
using Swabbr.Core.Interfaces;
using Swabbr.Infrastructure.Data;
using System;
using System.IO;
using System.Reflection;

namespace Swabbr
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
            services.AddControllers();

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            services.AddAuthentication(AzureADB2CDefaults.BearerAuthenticationScheme);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Swabbr", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            var connectionStringOptions = Configuration.GetSection("ConnectionStrings").Get<ConnectionStringOptions>();
            var cosmosDbOptions = Configuration.GetSection("CosmosDb").Get<CosmosDbOptions>();
            var connectionString = connectionStringOptions.ActiveConnectionString;
            var (databaseName, collections) = cosmosDbOptions;

            //TODO Comments
            // Verify database and collections existance.
            // Add CosmosDb. Ensure database and containers exist.
            services.AddCosmosDb(connectionString, databaseName, collections);

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IVlogRepository, VlogRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (/*env.IsDevelopment()*/Configuration.GetSection("ConnectionStrings").Get<ConnectionStringOptions>().Mode == ConnectionStringMode.Emulator)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swabbr v1");
            });
        }
    }
}