using AutoMapper;
using Laixer.Identity.Dapper.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swabbr.Api;
using Swabbr.Api.Authentication;
using Swabbr.Api.Documentation;
using Swabbr.Api.Helpers;
using Swabbr.Core;
using Swabbr.Core.Extensions;
using Swabbr.Core.Interfaces.Factories;
using Swabbr.Infrastructure.Configuration;
using Swabbr.Infrastructure.Extensions;
using System.Text;

namespace Swabbr
{
    /// <summary>
    ///     Called on application startup to configure 
    ///     service container and request pipeline.
    /// </summary>
    /// <remarks>
    ///     This contains environment specific configuration
    ///     methods. Which method gets called when is explained 
    ///     in the documentation of each method.
    /// </remarks>
    public class Startup
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        ///     Create new instance.
        /// </summary>
        public Startup(IConfiguration configuration) 
            => _configuration = configuration;

        /// <summary>
        ///     This method gets called by the runtime if no environment is set
        ///     or if no mathing ConfigureEnvironment method is found. Use this 
        ///     method to configure the services container in these scenarios.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public void ConfigureServices(IServiceCollection services) => GenericConfigureServices(services);

        /// <summary>
        ///     This method gets called by the runtime if our environment is 
        ///     set to development. Use this method to add development specific
        ///     services to the services container.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            GenericConfigureServices(services);

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin();
                    policy.AllowAnyHeader();
                    policy.AllowAnyMethod();
                });
            });
        }

        /// <summary>
        ///     This method will be called regardless of the environment.
        ///     Use this method to specify which services should exist in
        ///     regardless of the environment.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public void GenericConfigureServices(IServiceCollection services)
        {
            // FUTURE: Uncouple from NewtonSoft.Json
            //         System.Text.Json isn't capable of handling all our types, an issue exists for this.
            services.AddControllers()
                .AddNewtonsoftJson();
            services.AddHealthChecks();

            // Setup authentication and authorization.
            SetupIdentity(services);
            SetupAuthentication(services);
            services.AddAuthorization(options =>
            {
                // FUTURE Add custom policies.
                // Note: This only expects the user to be authenticated, nothing else.
                //       This is the default for all endpoints, unless specified otherwise.
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
            });

            // Bind main configuration properties.
            services.Configure<SwabbrConfiguration>(_configuration.GetSection("Swabbr"));

            // Add core services.
            // Note: This also makes the AppContext injectable using the IAppContextFactory singleton.
            services.AddSwabbrCoreServices();

            // Add infrastructure services.
            services.AddSwabbrInfrastructureServices("DatabaseInternal", "BlobStorage");
            // Explicitly add Azure Notification Hub configuration.
            services.Configure<NotificationHubConfiguration>(_configuration.GetSection("AzureNotificationHub"));

            // Add API specific services
            services.AddOrReplace<IAppContextFactory, AspAppContextFactory>(ServiceLifetime.Singleton);
            services.AddTransient<UserUpdateHelper>();
            services.AddAutoMapper(mapper => MapperProfile.SetupProfile(mapper));

            // Add doc
            SetupSwagger(services);
        }

        /// <summary>
        ///     This method gets called by the runtime if no environment is set or
        ///     if no matching ConfigureEnvironment method is found. Use this method 
        ///     to configure the request pipeline.
        /// </summary>
        /// <param name="app">Application builder.</param>
        public static void Configure(IApplicationBuilder app)
        {
            app.UseHsts();
            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Swabbr Documentation");
                options.RoutePrefix = string.Empty;
            });

            app.UsePathBase(new PathString("/api"));
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health").WithMetadata(new AllowAnonymousAttribute());
            });
        }

        /// <summary>
        ///     This method gets called by the runtime if the environment is set to
        ///     development. Use this method to configure the development pipeline.
        /// </summary>
        /// <param name="app">Application builder.</param>
        public static void ConfigureDevelopment(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();
            app.UseCors();

            app.UseHsts();
            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Swabbr Documentation");
                options.RoutePrefix = string.Empty;
            });

            app.UsePathBase(new PathString("/api"));
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        /// <summary>
        ///     Adds swagger to the services.
        /// </summary>
        private static void SetupSwagger(IServiceCollection services)
            => services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Swabbr Documentation",
                    Version = "v1",
                    Description = "Swabbr REST API"
                });

                // Add custom enum name display filter
                options.SchemaFilter<EnumSchemaFilter>();
            });

        // FUTURE: Remove dapper stores
        /// <summary>
        ///     Adds identity to the services.
        /// </summary>
        /// <remarks>
        ///     This uses Dapper to use identity along
        ///     with our npgsql database.
        /// </remarks>
        private void SetupIdentity(IServiceCollection services) =>
            services.AddIdentity<SwabbrIdentityUser, SwabbrIdentityRole>(setup =>
            {
                setup.Password.RequiredLength = 8;
                setup.User.RequireUniqueEmail = true;
            })
            .AddDapperStores(options =>
            {
                options.UserTable = "user";
                options.Schema = "application";
                options.MatchWithUnderscore = true;
                options.UseNpgsql<IdentityQueryRepository>(_configuration.GetConnectionString("DatabaseInternal"));
            })
            .AddDefaultTokenProviders();

        /// <summary>
        ///     Adds authentication to the services.
        /// </summary>
        private void SetupAuthentication(IServiceCollection services)
        {
            var jwtConfigSection = _configuration.GetSection("Jwt");
            var jwtConfig = jwtConfigSection.Get<JwtConfiguration>();
            var jwtKey = Encoding.ASCII.GetBytes(jwtConfig.SignatureKey);

            // Add the token service and jwt configuration
            services.AddSingleton<TokenService>();
            services.Configure<JwtConfiguration>(jwtConfigSection);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Issuer,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtKey)
                };
            });
        }
    }
}
